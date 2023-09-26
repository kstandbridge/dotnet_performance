namespace DotNetPerformance;

public class DataPlayerTest : IPlayerTest
{
    private DataPlayer _dataPlayers;
    private Int32 _ticksToTest;

    public string Label => "DataPlayer";
    private Profiler _profiler;

    public DataPlayerTest(DataPlayer dataPlayers, Int32 ticksToTest, Profiler profiler)
    {
        _dataPlayers = dataPlayers;
        _ticksToTest = ticksToTest;
        _profiler = profiler;
    }
    public void RunTest()
    {
        _profiler.NewTestWave();
        while(_profiler.IsTesting())
        {
            _profiler.Begin();
            for(Int32 tickIndex = 0; tickIndex < _ticksToTest; ++tickIndex)
            {
                _dataPlayers.ResourceTick();
            }
            _dataPlayers.ClearResource();
            _profiler.End();
        }
    }
}
