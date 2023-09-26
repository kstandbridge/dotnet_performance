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



        Profiler baseForEachProfiler = new Profiler();
        Profiler monoForEachProfiler = new Profiler();
        Profiler baseForProfiler = new Profiler();
        Profiler monoForProfiler = new Profiler();
        Int32 ticksToTest = 100;
        for(;;)
        {

            {
                Console.Write("\n--- BasePlayerForeach ---\n");

                baseForEachProfiler.NewTestWave(0);

                while(baseForEachProfiler.IsTesting())
                {
                    baseForEachProfiler.Begin();
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
                    baseForEachProfiler.End();
                }
            }

            {
                Console.Write("\n--- MonoPlayerForeach ---\n");

                monoForEachProfiler.NewTestWave(0);

                while(monoForEachProfiler.IsTesting())
                {
                    monoForEachProfiler.Begin();
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
                    monoForEachProfiler.End();
                }
            }

            {
                Console.Write("\n--- BasePlayerFor ---\n");

                baseForProfiler.NewTestWave(0);

                while(baseForProfiler.IsTesting())
                {
                    baseForProfiler.Begin();
                    for(Int32 tickIndex = 0; tickIndex < ticksToTest; ++tickIndex)
                    {
                        for(Int32 playerIndex = 0; playerIndex < count; playerIndex += 4)
                        {
                            basePlayers[playerIndex].ResourceTick();
                        }
                    }
                    for(Int32 playerIndex = 0; playerIndex < count; playerIndex += 4)
                    {
                        basePlayers[playerIndex].ClearResource();
                    }
                    baseForProfiler.End();
                }
            }

            {
                Console.Write("\n--- MonoPlayerFor ---\n");

                monoForProfiler.NewTestWave(0);

                while(monoForProfiler.IsTesting())
                {
                    monoForProfiler.Begin();
                    for(Int32 tickIndex = 0; tickIndex < ticksToTest; ++tickIndex)
                    {
                        for(Int32 playerIndex = 0; playerIndex < count; playerIndex += 4)
                        {
                            monoPlayers[playerIndex].ResourceTick();
                        }
                    }
                    for(Int32 playerIndex = 0; playerIndex < count; playerIndex += 4)
                    {
                        monoPlayers[playerIndex].ClearResource();
                    }
                    monoForProfiler.End();
                }
            }

        }
    }
}