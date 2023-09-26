namespace DotNetPerformance;

public class BasePlayerTest: IPlayerTest
{
    private BasePlayer[] _basePlayers;
    private Int32 _ticksToTest;

    public string Label => "BasePlayer";
    private Profiler _profiler;

    public BasePlayerTest(BasePlayer[] basePlayers, Int32 ticksToTest, Profiler profiler)
    {
        _basePlayers = basePlayers;
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
                for(Int32 playerIndex = 0; playerIndex < _basePlayers.Length; ++playerIndex)
                {
                    _basePlayers[playerIndex].ResourceTick();
                }
            }
            for(Int32 playerIndex = 0; playerIndex < _basePlayers.Length; ++playerIndex)
            {
                _basePlayers[playerIndex].ClearResource();
            }
            _profiler.End();
        }
    }
}
