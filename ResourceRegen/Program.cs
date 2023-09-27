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
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("");

        Random random = new Random();
        Int32 count = 65536;
        BasePlayer[] basePlayers = new BasePlayer[count];
        MonoPlayer[] monoPlayers = new MonoPlayer[count];
        DataPlayer dataPlayers = new DataPlayer(count);

        for(Int32 index = 0; index < count; ++index)
        {
            UInt64 intellect = (UInt64)random.NextInt64(Int32.MaxValue, Int64.MaxValue / 10);
            UInt64 spirit = (UInt64)random.Next(Int16.MaxValue / 100, Int32.MaxValue / 100);
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
            dataPlayers.Add(index, classType, intellect, spirit);
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

        for(Int32 index = 0; index < count; ++index)
        {
            UInt64 beforeTick = dataPlayers.Resource(index);
            dataPlayers.ResourceTick(index);
            UInt64 afterTick = dataPlayers.Resource(index);

            if(beforeTick == afterTick)
            {
                Console.WriteLine($"Error: resource tick had no effect");
                break;
            }
        }
        dataPlayers.ClearResource();

        Int32 ticksToTest = 1024;
        IPlayerTest[] playerTests = new IPlayerTest[]
        {
            // new DataPlayerTest(dataPlayers, ticksToTest, new Profiler()),
            new BasePlayerTest(basePlayers, ticksToTest, new Profiler()),
            new MonoPlayerTest(monoPlayers, ticksToTest, new Profiler()),
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