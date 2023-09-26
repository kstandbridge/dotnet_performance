namespace DotNetPerformance;

public class MonoPlayerTest: IPlayerTest
{
    private MonoPlayer[] _monoPlayers;
    private Int32 _ticksToTest;

    public string Label => "MonoPlayer";
    private Profiler _profiler;

    public MonoPlayerTest(MonoPlayer[] monoPlayers, Int32 ticksToTest, Profiler profiler)
    {
        _monoPlayers = monoPlayers;
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
                for(Int32 playerIndex = 0; playerIndex < _monoPlayers.Length; ++playerIndex)
                {
                    _monoPlayers[playerIndex].ResourceTick();
                }
            }
            for(Int32 playerIndex = 0; playerIndex < _monoPlayers.Length; ++playerIndex)
            {
                _monoPlayers[playerIndex].ClearResource();
            }
            _profiler.End();
        }
    }
}
