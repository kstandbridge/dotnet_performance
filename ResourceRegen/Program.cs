namespace DotNetPerformance;

public enum PlayerClassType
{
    Priest,
    Mage,
    Rogue,
    Warrior,

    Count
}

public interface IPlayerTest
{
    public string Label { get; }
    public void RunTest();
}

public class MonoPlayerForeach : IPlayerTest
{
    private MonoPlayer[] _monoPlayers;
    private Int32 _ticksToTest;

    public string Label => "MonoPlayerForeach";
    private Profiler _profiler;

    public MonoPlayerForeach(MonoPlayer[] monoPlayers, Int32 ticksToTest, Profiler profiler)
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
            for(Int32 index = 0; index < _ticksToTest; ++index)
            {
                foreach(MonoPlayer player in _monoPlayers)
                {
                    player.ResourceTick();
                }
            }
            foreach(MonoPlayer player in _monoPlayers)
            {
                player.ClearResource();
            }
            _profiler.End();
        }
    }
}


public class MonoPlayerFor: IPlayerTest
{
    private MonoPlayer[] _monoPlayers;
    private Int32 _ticksToTest;

    public string Label => "MonoPlayerFor";
    private Profiler _profiler;

    public MonoPlayerFor(MonoPlayer[] monoPlayers, Int32 ticksToTest, Profiler profiler)
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

public class BasePlayerForeach : IPlayerTest
{
    private BasePlayer[] _basePlayers;
    private Int32 _ticksToTest;

    public string Label => "BasePlayerForeach";
    private Profiler _profiler;

    public BasePlayerForeach(BasePlayer[] basePlayers, Int32 ticksToTest, Profiler profiler)
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
            for(Int32 index = 0; index < _ticksToTest; ++index)
            {
                foreach(BasePlayer player in _basePlayers)
                {
                    player.ResourceTick();
                }
            }
            foreach(BasePlayer player in _basePlayers)
            {
                player.ClearResource();
            }
            _profiler.End();
        }
    }
}

public class BasePlayerFor: IPlayerTest
{
    private BasePlayer[] _basePlayers;
    private Int32 _ticksToTest;

    public string Label => "BasePlayerFor";
    private Profiler _profiler;

    public BasePlayerFor(BasePlayer[] basePlayers, Int32 ticksToTest, Profiler profiler)
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

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("");

        Random random = new Random();
        Int32 count = 65536;
        BasePlayer[] basePlayers = new BasePlayer[count];
        MonoPlayer[] monoPlayers = new MonoPlayer[count];
        for(Int32 index = 0; index < count; ++index)
        {
            UInt64 intellect = (UInt64)random.Next(500, 2500);
            UInt64 spirit = (UInt64)random.Next(500, 2500);
            PlayerClassType classType = (PlayerClassType)random.Next(0, (Int32)PlayerClassType.Count);
            switch (classType)
            {
                case PlayerClassType.Priest:  basePlayers[index] = new PriestPlayer(intellect, spirit); break;
                case PlayerClassType.Mage:    basePlayers[index] = new MagePlayer(intellect, spirit); break;
                case PlayerClassType.Rogue:   basePlayers[index] = new RoguePlayer(intellect, spirit); break;
                case PlayerClassType.Warrior: basePlayers[index] = new WarriorPlayer(intellect, spirit); break;
                default:
                {
                    Console.WriteLine($"Error: class type {classType} out of range!");
                } break;
            }
            monoPlayers[index] = new MonoPlayer(classType, intellect, spirit);
        }

        if(basePlayers.Length != monoPlayers.Length)
        {
            Console.WriteLine($"Error: player count mismatch");
        }

        foreach(BasePlayer player in basePlayers)
        {
            UInt64 beforeTick = player.Resource;
            player.ResourceTick();
            UInt64 afterTick = player.Resource;

            if(beforeTick == afterTick)
            {
                Console.WriteLine($"Error: resource tick had no effect");
                break;
            }
        }

        foreach(BasePlayer player in basePlayers)
        {
            player.ClearResource();
        }
        
        foreach(MonoPlayer player in monoPlayers)
        {
            UInt64 beforeTick = player.Resource;
            player.ResourceTick();
            UInt64 afterTick = player.Resource;

            if(beforeTick == afterTick)
            {
                Console.WriteLine($"Error: resource tick had no effect");
                break;
            }
        }

        foreach(MonoPlayer player in monoPlayers)
        {
            player.ClearResource();
        }

        Int32 ticksToTest = 1000;
        IPlayerTest[] playerTests = new IPlayerTest[]
        {
            new BasePlayerForeach(basePlayers, ticksToTest, new Profiler()),
            new MonoPlayerForeach(monoPlayers, ticksToTest, new Profiler()),
            new BasePlayerFor(basePlayers, ticksToTest, new Profiler()),
            new MonoPlayerFor(monoPlayers, ticksToTest, new Profiler()),
        };

        for(;;)
        {
            foreach(IPlayerTest playerTest in playerTests)
            {
                Console.Write($"\n--- {playerTest.Label} ---\n");
                playerTest.RunTest();
            }
        }

    }
}