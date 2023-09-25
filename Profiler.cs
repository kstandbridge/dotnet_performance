using System.Diagnostics;

namespace DotNetPerformance;

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

    public bool IsTesting()
    {
        if(_state == ProfilerState.Testing)
        {
            if(_openBlockCount != _closeBlockCount)
            {
                Error("Unbalanced Begin/End blocks");
            }
        }

        Int64 currentTime = Stopwatch.GetTimestamp();
        
        // NOTE(kstandbridge): Error checks above could have changed the state
        if((_openBlockCount) > 0 &&
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
