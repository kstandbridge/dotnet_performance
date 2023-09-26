namespace DotNetPerformance;

public enum PlayerClassType
{
    Priest,
    Mage,
    Rogue,
    Warrior,

    Count
}

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("");

        Random random = new Random();
        List<BasePlayer> basePlayers = new List<BasePlayer>();
        List<MonoPlayer> monoPlayers = new List<MonoPlayer>();
        Int32 count = 100000;
        for(Int32 index = 0; index < count; ++index)
        {
            UInt64 intellect = (UInt64)random.Next(500, 2500);
            UInt64 spirit = (UInt64)random.Next(500, 2500);
            PlayerClassType classType = (PlayerClassType)random.Next(0, (Int32)PlayerClassType.Count);
            switch (classType)
            {
                case PlayerClassType.Priest:  basePlayers.Add(new PriestPlayer(intellect, spirit)); break;
                case PlayerClassType.Mage:    basePlayers.Add(new MagePlayer(intellect, spirit)); break;
                case PlayerClassType.Rogue:   basePlayers.Add(new RoguePlayer(intellect, spirit)); break;
                case PlayerClassType.Warrior: basePlayers.Add(new WarriorPlayer(intellect, spirit)); break;
                default:
                {
                    Console.WriteLine($"Error: class type {classType} out of range!");
                } break;
            }
            monoPlayers.Add(new MonoPlayer(classType, intellect, spirit));
        }

        if(basePlayers.Count != monoPlayers.Count)
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



        Profiler baseProfiler = new Profiler();
        Profiler monoProfiler = new Profiler();
        Int32 ticksToTest = 1000;
        for(;;)
        {
            {
                Console.Write("\n--- BasePlayer ---\n");

                baseProfiler.NewTestWave(0);

                while(baseProfiler.IsTesting())
                {
                    baseProfiler.Begin();
                    for(Int32 index = 0; index < ticksToTest; ++index)
                    {
                        foreach(BasePlayer player in basePlayers)
                        {
                            player.ResourceTick();
                        }
                    }
                    foreach(BasePlayer player in basePlayers)
                    {
                        player.ClearResource();
                    }
                    baseProfiler.End();
                }
            }

            {
                Console.Write("\n--- MonoPlayer ---\n");

                monoProfiler.NewTestWave(0);

                while(monoProfiler.IsTesting())
                {
                    monoProfiler.Begin();
                    for(Int32 index = 0; index < ticksToTest; ++index)
                    {
                        foreach(MonoPlayer player in monoPlayers)
                        {
                            player.ResourceTick();
                        }
                    }
                    foreach(MonoPlayer player in monoPlayers)
                    {
                        player.ClearResource();
                    }
                    monoProfiler.End();
                }
            }
        }
    }
}