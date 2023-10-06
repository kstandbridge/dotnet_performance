using System.Diagnostics;

namespace DotNetPerformance;

public static class ProfilerExtensions
{
    public static Profiler WithTargetBytes(this Profiler profiler, Int64 targetBytes)
    {
        profiler._targetBytes = targetBytes;
        return profiler;
    }


    public static Profiler WithTargetEntities(this Profiler profiler, Int64 targetEntities)
    {
        profiler._targetEntities = targetEntities;
        return profiler;
    }
}

public class Profiler
{
    private enum ProfilerState
    {
        Uninitialized,
        Testing,
        Completed,
        Error,
    }

    private enum ProfilerValueType
    {
        TestCount,

        Ticks,
        Bytes,
        Entities,

        Count,
    }

    private class ProfilerValue
    {
        public Int64[] E = new Int64[(Int32)ProfilerValueType.Count];
    }

    private class ProfilerResults
    {
        public ProfilerValue Total { get; } = new();
        public ProfilerValue Min { get; set; } = new();
        public ProfilerValue Max { get; set; } = new();
    }
    
    private ProfilerState _state = ProfilerState.Uninitialized;
    private ProfilerResults _results = new();
    private ProfilerValue _currentTest = new();

    private Int64 _testTime;
    private Int64 _startTime;

    private Int32 _openBlockCount;
    private Int32 _closeBlockCount;

    public Int64 _targetBytes;
    public Int64 _targetEntities;

    private void Error(string message)
    {
        _state = ProfilerState.Error;
        Console.WriteLine($"Error: {message}");
    }

    private void Print(string label, ProfilerValue value)
    {
        Int64 count = value.E[(Int32)ProfilerValueType.TestCount];
        Double divisor = (count > 0) ? (Double)count : 1.0d;

        Int64[] e = new Int64[(Int32)ProfilerValueType.Count]; 
        for(Int32 eIndex = 0; eIndex < (Int32)ProfilerValueType.Count; ++eIndex)
        {
            e[eIndex] = (Int64)((Double)value.E[eIndex] / divisor);
        }

        Console.Write($"{label} : {e[(Int32)ProfilerValueType.Ticks]}");
        Double seconds = (Double)e[(Int32)ProfilerValueType.Ticks]/(Double)Stopwatch.Frequency;
        Console.Write($" ({seconds*1000.0d:F2}ms)");
        
        if(e[(Int32)ProfilerValueType.Bytes] > 0)
        {
            Double gigabyte = (1024.0d*1024.0d*1024.0d);
            Double bandwidth = (Double)e[(Int32)ProfilerValueType.Bytes] / (gigabyte * seconds);
            Console.Write($" {bandwidth:F2}gb/s");
        }

        if(e[(Int32)ProfilerValueType.Entities] > 0)
        {
            Double bandwidth = (Double)e[(Int32)ProfilerValueType.Entities] / seconds / 1000.0d;
            Console.Write($" ({bandwidth:F0} entities/ms)");
             
        }
    }

    public void NewTestWave(Int64 lengthInSeconds = 10)
    {
        switch(_state)
        {
            case ProfilerState.Uninitialized:
            {
                _state = ProfilerState.Testing;
                _results.Min.E[(Int32)ProfilerValueType.Ticks] = Int64.MaxValue;
            } break;
            
            case ProfilerState.Completed:
            {
                _state = ProfilerState.Testing;
            } break;
        }

        _testTime = lengthInSeconds*Stopwatch.Frequency;
        _startTime = Stopwatch.GetTimestamp();
    }

    public void Begin()
    {
        ++_openBlockCount;

        _currentTest.E[(Int32)ProfilerValueType.Ticks] -= Stopwatch.GetTimestamp();
    }

    public void End()
    {
        _currentTest.E[(Int32)ProfilerValueType.Ticks] += Stopwatch.GetTimestamp();

        ++_closeBlockCount;
    }

    public void CountBytes(Int64 bytes)
    {
        _currentTest.E[(Int32)ProfilerValueType.Bytes] += bytes;
    }

    public void CountEntities(Int64 entities)
    {
        _currentTest.E[(Int32)ProfilerValueType.Entities] += entities;
    }

    public bool IsTesting()
    {
        if(_state == ProfilerState.Testing)
        {
            if(_openBlockCount != _closeBlockCount)
            {
                Error("Unbalanced Begin/End blocks");
            }

            if(_openBlockCount > 0) 
            {
                if(_currentTest.E[(Int32)ProfilerValueType.Bytes] != _targetBytes)
                {
                    Error("Processed byte count mismatch");
                }

                if(_currentTest.E[(Int32)ProfilerValueType.Entities] != _targetEntities)
                {
                    Error("Processed entity count mismatch");
                }
            }
        }

        Int64 currentTime = Stopwatch.GetTimestamp();
        
        // NOTE(kstandbridge): Error checks above could have changed the state
        if((_openBlockCount > 0) &&
           (_state == ProfilerState.Testing))
        {
            _currentTest.E[(Int32)ProfilerValueType.TestCount] = 1;
            for(Int32 eIndex = 0; eIndex < (Int32)ProfilerValueType.Count; ++eIndex)
            {
                _results.Total.E[eIndex] += _currentTest.E[eIndex];
            }

            if(_results.Max.E[(Int32)ProfilerValueType.Ticks] < _currentTest.E[(Int32)ProfilerValueType.Ticks])
            {
                _results.Max = _currentTest;
            }

            if(_results.Min.E[(Int32)ProfilerValueType.Ticks] > _currentTest.E[(Int32)ProfilerValueType.Ticks])
            {
                _results.Min = _currentTest;

                _startTime = currentTime;

                Print("Min", _results.Min);
                Console.Write("                                                          \r");
            }

            _openBlockCount = 0;
            _closeBlockCount = 0;
            _currentTest = new ProfilerValue();
        }

        if((currentTime - _startTime) > _testTime)
        {
            _state = ProfilerState.Completed;
            Console.Write("                                                          \r");
            Print("Min", _results.Min); Console.WriteLine("");
            Print("Max", _results.Max); Console.WriteLine("");
            Print("Avg", _results.Total); Console.WriteLine("");
        }

        bool result = (_state == ProfilerState.Testing);
        return result;
    }
}
