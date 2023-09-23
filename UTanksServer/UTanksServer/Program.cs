using System;
using System.Linq;
using System.Reflection;
using UTanksServer.ECS.Components.Battle.Bonus;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Extensions;
using System.Collections.Generic;
using SimpleJSON;
using UTanksServer.Database.Databases;
using Core;
using UTanksServer.Network.Simple.Net;
using UTanksServer.Network.NetworkEvents.Communications;
using UTanksServer.Network.NetworkEvents.Security;
using UTanksServer.Network.NetworkEvents.PlayerAuth;
using UTanksServer.Network.NetworkEvents.PlayerSettings;
using System.Text;
using UTanksServer.Database;
using UTanksServer.Core;
using System.Net;
using System.Threading;
using UTanksServer.Services;
using System.Reflection.Emit;
using UTanksServer.ECS.Components;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UTanksServer.ECS.Components.Notification;
using UTanksServer.ECS.DataAccessPolicy;
using UTanksServer.ECS.Components.Battle;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Garage;
using UTanksServer.ECS.Types;
using UTanksServer.ECS;
using System.Security.Cryptography;
using System.Globalization;
using System.Collections.Concurrent;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using UTanksServer.ECS.Events.User;
using UTanksServer.ECS.Templates.User;
using UTanksServer.ECS.Events.Battle;
using UTanksServer.ECS.Components.Battle.BattleComponents;
using UTanksServer.ECS.Components.Battle.BotComponent;
using UTanksServer.ECS.Systems.Battles;
using UTanksServer.ECS.Systems.User;
using UTanksServer.ECS.Systems.NetworkUpdatingSystems;
using UTanksServer.Network.NetworkEvents.FastGameEvents;

public static class GlobalProgramState
{
#if Windows //|| !RELEASE
    public static string ConfigDir = AppDomain.CurrentDomain.BaseDirectory + "Data\\";
    public static string PathSeparator = "\\";
#else
    public static string ConfigDir = AppDomain.CurrentDomain.BaseDirectory + "Data/";
    public static string PathSeparator = "/";
#endif
    public static ProgramType programType = ProgramType.Server;
    public enum ProgramType {Server, Client}
}

namespace UTanksServer
{
    class Program
    {
        public static JSONFile Config;
        public static List<byte> ConfigFilesZip = new List<byte>();
        public static long hashConfigFilesZip;

        static void Main(string[] args)
        {
            Console.Title = "Just Tanks Server";
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
      _   _____     ____    _____   ____   __     __  _____   ____  
     | | |_   _|   / ___|  | ____| |  _ \  \ \   / / | ____| |  _ \ 
  _  | |   | |     \___ \  |  _|   | |_) |  \ \ / /  |  _|   | |_) |
 | |_| |   | |      ___) | | |___  |  _ <    \ V /   | |___  |  _ < 
  \___/    |_|     |____/  |_____| |_| \_\    \_/    |_____| |_| \_\
                                                                    
");
            Console.ResetColor();
#region read_config_resource
            Config = new JSONFile(GlobalProgramState.ConfigDir + "config.json", true).Load();
            if (Config["Logger"]["PrintDebugVerbose"].AsBool) Console.WriteLine("The server is set to display debug messages, to disable set 'Logger.PrintDebugVerbose' to false (in config)\n");
            DatabaseQuery.NewbieAccountGarage = File.ReadAllText(GlobalProgramState.ConfigDir + "newbie.json");
            if (!File.Exists(GlobalProgramState.ConfigDir + "zippedconfig.zip"))
            {
#region prepareZipTemp
                if (Directory.Exists(GlobalProgramState.ConfigDir + "ZipTemp"))
                    Directory.Delete(GlobalProgramState.ConfigDir + "ZipTemp", true);
                FileEx.CopyFilesRecursively(new DirectoryInfo(GlobalProgramState.ConfigDir + "GameConfig"), new DirectoryInfo(GlobalProgramState.ConfigDir + "ZipTemp" + GlobalProgramState.PathSeparator + "GameConfig"));
                File.Copy(GlobalProgramState.ConfigDir + "donateshop.json", GlobalProgramState.ConfigDir + "ZipTemp" + GlobalProgramState.PathSeparator + "donateshop.json");
                File.Copy(GlobalProgramState.ConfigDir + "garageshop.json", GlobalProgramState.ConfigDir + "ZipTemp" + GlobalProgramState.PathSeparator + "garageshop.json");
                File.Copy(GlobalProgramState.ConfigDir + "selectablemapdb.json", GlobalProgramState.ConfigDir + "ZipTemp" + GlobalProgramState.PathSeparator + "selectablemapdb.json");
#endregion
                ZipExt.CompressDirectory(GlobalProgramState.ConfigDir + "ZipTemp", GlobalProgramState.ConfigDir + "zippedconfig.zip", (prog) => { });
            }
            Byte[] bytes = File.ReadAllBytes(GlobalProgramState.ConfigDir + "zippedconfig.zip");
            using (MD5CryptoServiceProvider CSP = new MD5CryptoServiceProvider())
            {
                var byteHash = CSP.ComputeHash(bytes);
                string result = "";
                foreach (byte b in byteHash)
                    result += b.ToString();
                hashConfigFilesZip = long.Parse(result.Substring(0, 18));
            }
            hashConfigFilesZip = BitConverter.ToInt64(MD5.Create().ComputeHash(bytes), 0);
            ConfigFilesZip = new List<byte>(bytes);
            //ZipExt.DecompressToDirectory(@"L:\UTanksServer\UTanksServer\bin\Debug\net5.0\Data\zippedconfig.zip", @"L:\UTanksServer\UTanksServer\bin\Debug\net5.0\Data\test", (prog) => { });
#endregion
            Logger.Log("Config loaded!", "init");
            //ServerDatabase.Load();
            ConstantService.Initialize();     
            //GlobalCachingSerialization.Init();//serialization not work heh
            UserDatabase.Load();
            ManagerScope.InitManagerScope();
            InitializeDefaultDataObject.InitializeDataObjects();
            Networking.Start();
            //new UTServer();
#region ClearScreen
            Func<Task> asyncUpd = async () =>
            {
                await Task.Run(() => {
                    Thread.Sleep(5000);
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(@"
      _   _____     ____    _____   ____   __     __  _____   ____  
     | | |_   _|   / ___|  | ____| |  _ \  \ \   / / | ____| |  _ \ 
  _  | |   | |     \___ \  |  _|   | |_) |  \ \ / /  |  _|   | |_) |
 | |_| |   | |      ___) | | |___  |  _ <    \ V /   | |___  |  _ < 
  \___/    |_|     |____/  |_____| |_| \_\    \_/    |_____| |_| \_\
                                                                    
");
                    Console.ResetColor();
                });
            };
            asyncUpd();
#endregion
            RunInput(args);
        }

        static async void RunInput(string[] args)
        {
            while (true)
            {
                string[] input = CreateArgs(Console.ReadLine());
                if (input.Length == 0) input = new string[] { string.Empty };
                if(args.Length > 0)
                {
                    ServerInstanceStart.MainServerInstanceStart(args);
                }
                switch (input[0].ToLower())
                {
                    case "exit":
                        Networking.Stop();
                        return;              
                    case "jtserver":
                        if (input.Length >= 2)
                        {
                            switch (input[1].ToLower())
                            {
                                case "kill":
                                case "exit":
                                case "stop":
                                    if (UTServer.Instance.IsRunning)
                                        UTServer.Instance?.Destroy(false);
                                    else Console.WriteLine("JTServer is not running");
                                    break;
                                case "start":
                                case "run":
                                    if (!UTServer.Instance.IsRunning)
                                        new UTServer();
                                    else Console.WriteLine("JTServer is already running");
                                    break;
                                case "restart":
                                case "reboot":
                                    if (UTServer.Instance.IsRunning)
                                        UTServer.Instance.Destroy(true);
                                    else new UTServer();
                                    break;
                                case "enable":
                                    UTServer.Config["EnableModule"] = true;
                                    Console.WriteLine("JTServer Enabled");
                                    break;
                                case "disable":
                                    UTServer.Config["EnableModule"] = false;
                                    Console.WriteLine("JTServer Disabled");
                                    if (UTServer.Instance.IsRunning)
                                        UTServer.Instance.Destroy(false);
                                    break;
                                default:
                                    Console.WriteLine($"Unknown command '{input[1]}', to view a list of options, use 'JTServer'");
                                    break;
                            }
                        }
                        else
                            Console.WriteLine("JTServer Commands:" +
                                "\n  kill - Stops the server (alias: exit, stop)" +
                                "\n  start - Starts the server (alias: run)" +
                                "\n  enable - Enable the excecution of the server (session-only)" +
                                "\n  disable - Disabled the server and kills the process (if any, session-only)"
                            );
                        break;
                    case "addserver":
                        if (input.Length < 5)
                        {
                            Console.WriteLine($"Not enough parameters! (Need: 5 got {input.Length})\nSyntax: addserver <string: name> <string: key> <string: token> <string: addresses>");
                            break;
                        }
                        Console.WriteLine("Validating and creating credentials...");
                        ServerRow serverRow = new ServerRow
                        {
                            name = input[1],
                            key = input[2],
                            token = input[3],
                            addresses = new List<string>(input[4].Split(' '))
                        };
                        ServerRow otherServer = await ServerDatabase.Servers.Get(HashUtil.MD5(serverRow.key));
                        if (otherServer != ServerRow.Empty)
                        {
                            Console.WriteLine($"Unable to create login credentials, key is used by '{otherServer.name}' (sid: {otherServer.uid})");
                            break;
                        }
                        Console.WriteLine($"Confirm Info (ignore uid): {serverRow}");

                        bool createAccount = false;
                        while (true)
                        {
                            Console.Write("\nConfirm (Y/n): ");
                            ConsoleKeyInfo response = Console.ReadKey();
                            if (response.Key == ConsoleKey.Y || response.Key == ConsoleKey.N || response.Key == ConsoleKey.Enter)
                            {
                                createAccount = response.Key == ConsoleKey.Enter ? true : response.Key == ConsoleKey.Y;
                                if (response.Key != ConsoleKey.Enter) Console.WriteLine();
                                break;
                            }
                        }
                        serverRow.key = HashUtil.MD5(serverRow.key);
                        serverRow.token = HashUtil.Compute(serverRow.token);
                        if (!createAccount)
                            Console.WriteLine("Canceled");
                        else if (await ServerDatabase.Servers.Create(serverRow))
                            Console.WriteLine($"Created login creds for '{serverRow.name}'. Now enter them in your server config, and boot it up!");
                        else Console.WriteLine("Failed to create login creds. Database error");
                        break;
                    case "help":
                        Console.WriteLine("Useful Commands" +
                            "\n  addserver - Create login credentials to the DB" +
                            "\n  ld - loadbots" +
                            "\n  lda - loadbotsall" +
                            "\n  lda - loadbotsall" +
                            "\n  tb - testbattle" +
                            "\n  JTServer - Modify the JTServer process");
                        break;
                    case "drops":
                        Console.WriteLine("not realized");
                        break;
                    case "battles":
                        ManagerScope.entityManager.EntityStorage.Values.Where((x) => x.HasComponent(BattleComponent.Id)).ToList().ForEach(x => {
                            Console.WriteLine(x.instanceId.ToString() + "   " + x.GetComponent< BattleComponent>().BattleCustomName + "   " + x.GetComponent<BattleComponent>().BattleRealName);
                        });
                        break;
                    case "players":
                        int inbattle = 0;
                        int all = 0;
                        ManagerScope.entityManager.EntityStorage.Values.Where((x) => x.HasComponent(UserOnlineComponent.Id)).ToList().ForEach(x => {
                            if (x.HasComponent(BattleOwnerComponent.Id))
                                inbattle++;
                            all++;
                        });
                        Console.WriteLine("All online" + all.ToString() + "\nIn battles: " + inbattle.ToString());
                        break;
                    case "ld":
                        if (true)
                        {
                            var listOfBattles = ManagerScope.entityManager.EntityStorage.Values.Where((x) => x.HasComponent(BattleComponent.Id)).ToList();
                            listOfBattles.ForEach(x => {
                                Console.WriteLine(x.instanceId.ToString() + "   " + x.GetComponent<BattleComponent>().BattleCustomName + "   " + x.GetComponent<BattleComponent>().BattleRealName);
                            });
                            Console.WriteLine("enter num of battle");
                            var battleId = Console.ReadLine();
                            try
                            {
                                var selectedBattle = listOfBattles[int.Parse(battleId)];
                                Console.WriteLine("enter num of bots");
                                var botsCount = Console.ReadLine();

                                foreach (var battleteam in selectedBattle.GetComponent<BattleTeamsComponent>().teams.Values.ToList())
                                {
                                    for (int i = 0; i < int.Parse(botsCount); i++)
                                    {
                                        var userNetwork = new Network.Simple.Net.Server.User() { PlayerEntityId = Guid.NewGuid().GuidToLong() };
                                        var userrow = new UserRow()
                                        {
                                            ActiveChatBanEndTime = 0,
                                            Clan = -1,
                                            Crystalls = 100000,
                                            Email = new Random().RandomString(10),
                                            EmailVerified = true,
                                            GarageJSONData = DatabaseQuery.NewbieAccountGarage,
                                            GlobalScore = 193000,
                                            HardwareId = new Random().RandomString(10),
                                            HardwareToken = new Random().RandomString(10),
                                            id = -1,
                                            Karma = 0,
                                            LastDatetimeGetDailyBonus = 0,
                                            LastIp = "127.0.0.1",
                                            Password = "c20ad4d76fe97759aa27a0c99bff6710",
                                            Rank = 14,
                                            RankScore = 1000,
                                            RegistrationDate = -1,
                                            TermlessBan = false,
                                            UserGroup = "admin",
                                            UserLocation = "en",
                                            Username = "bot_" + new Random().RandomString(8)
                                        };

                                        var entityUser = new UserTemplate().CreateEntity(userrow, userNetwork.PlayerEntityId);

                                        ManagerScope.eventManager.OnEventAdd(new UserLogged()
                                        {
                                            networkSocket = userNetwork,
                                            EntityOwnerId = userNetwork.PlayerEntityId,
                                            userRelogin = false,
                                            userEntity = entityUser,
                                            actionAfterLoggin = (entity) =>
                                            {
                                                ManagerScope.eventManager.OnEventAdd(new EnterToBattleEvent()
                                                {
                                                    BattleId = selectedBattle.instanceId,
                                                    TeamInstanceId = battleteam.instanceId,
                                                    EntityOwnerId = entity.instanceId
                                                });

                                                Thread.Sleep(100);
                                                ManagerScope.eventManager.OnEventAdd(new BattleLoadedEvent()
                                                {
                                                    BattleId = selectedBattle.instanceId,
                                                    TeamInstanceId = battleteam.instanceId,
                                                    EntityOwnerId = entity.instanceId
                                                });
                                                Thread.Sleep(100);
                                                entity.AddComponent(new AutoSmokyComponent(5f).SetGlobalComponentGroup());
                                            }
                                        });
                                    }
                                }
                            }
                            catch { }
                        }
                        break;
                    case "lda":
                        if (true)
                        {
                            ManagerScope.systemManager.SystemsInterestedEntityDatabase.Keys.ForEach(x =>  {
                                if(x.GetType() == typeof(BattleUserUpdaterSystem))
                                {
                                    x.Enabled = false;
                                }
                                if (x.GetType() == typeof(UserUpdaterSystem))
                                {
                                    x.Enabled = false;
                                }
                                if (x.GetType() == typeof(BattleUpdaterSystem))
                                {
                                    x.Enabled = false;
                                }
                                if (x.GetType() == typeof(BattleInfoUpdaterSystem))
                                {
                                    x.Enabled = false;
                                }
                                if (x.GetType() == typeof(UserUpdaterSystem))
                                {
                                    x.Enabled = false;
                                }
                            });
                            var listOfBattles = ManagerScope.entityManager.EntityStorage.Values.Where((x) => x.HasComponent(BattleComponent.Id) && x.GetComponent<BattlePlayersComponent>().players.Count == 0).ToList();
                            listOfBattles.ForEach(x => {
                                Console.WriteLine(x.instanceId.ToString() + "   " + x.GetComponent<BattleComponent>().BattleCustomName + "   " + x.GetComponent<BattleComponent>().BattleRealName);
                            });
                            Console.WriteLine("enter num of bots");
                            var botsCount = Console.ReadLine();
                            foreach (var selectedBattle in listOfBattles)
                            {
                                try
                                {
                                    foreach(var battleteam in selectedBattle.GetComponent<BattleTeamsComponent>().teams.Values.ToList())
                                    {
                                        for (int i = 0; i < int.Parse(botsCount); i++)
                                        {
                                            var userNetwork = new Network.Simple.Net.Server.User() { PlayerEntityId = Guid.NewGuid().GuidToLong() };
                                            var userrow = new UserRow()
                                            {
                                                ActiveChatBanEndTime = 0,
                                                Clan = -1,
                                                Crystalls = 100000,
                                                Email = new Random().RandomString(10),
                                                EmailVerified = true,
                                                GarageJSONData = DatabaseQuery.NewbieAccountGarage,
                                                GlobalScore = 193000,
                                                HardwareId = new Random().RandomString(10),
                                                HardwareToken = new Random().RandomString(10),
                                                id = -1,
                                                Karma = 0,
                                                LastDatetimeGetDailyBonus = 0,
                                                LastIp = "127.0.0.1",
                                                Password = "c20ad4d76fe97759aa27a0c99bff6710",
                                                Rank = 14,
                                                RankScore = 1000,
                                                RegistrationDate = -1,
                                                TermlessBan = false,
                                                UserGroup = "admin",
                                                UserLocation = "en",
                                                Username = "bot_" + new Random().RandomString(8)
                                            };

                                            var entityUser = new UserTemplate().CreateEntity(userrow, userNetwork.PlayerEntityId);

                                            ManagerScope.eventManager.OnEventAdd(new UserLogged()
                                            {
                                                networkSocket = userNetwork,
                                                EntityOwnerId = userNetwork.PlayerEntityId,
                                                userRelogin = false,
                                                userEntity = entityUser,
                                                actionAfterLoggin = (entity) =>
                                                {
                                                    ManagerScope.eventManager.OnEventAdd(new EnterToBattleEvent()
                                                    {
                                                        BattleId = selectedBattle.instanceId,
                                                        TeamInstanceId = battleteam.instanceId,
                                                        EntityOwnerId = entity.instanceId
                                                    });

                                                    Task.Delay(20);
                                                    ManagerScope.eventManager.OnEventAdd(new BattleLoadedEvent()
                                                    {
                                                        BattleId = selectedBattle.instanceId,
                                                        TeamInstanceId = battleteam.instanceId,
                                                        EntityOwnerId = entity.instanceId
                                                    });
                                                    Task.Delay(20);
                                                    entity.AddComponent(new AutoSmokyComponent(3f).SetGlobalComponentGroup());
                                                }
                                            });
                                        }
                                    }
                                    
                                }
                                catch { }
                            }
                            ManagerScope.systemManager.SystemsInterestedEntityDatabase.Keys.ForEach(x => {
                                if (x.GetType() == typeof(BattleUserUpdaterSystem))
                                {
                                    x.Enabled = true;
                                    x.InWork = false;
                                    x.LastEndExecutionTimestamp = DateTime.Now.Ticks;
                                }
                                if (x.GetType() == typeof(UserUpdaterSystem))
                                {
                                    x.Enabled = true;
                                    x.InWork = false;
                                    x.LastEndExecutionTimestamp = DateTime.Now.Ticks;
                                }
                                if (x.GetType() == typeof(BattleUpdaterSystem))
                                {
                                    x.Enabled = true;
                                    x.InWork = false;
                                    x.LastEndExecutionTimestamp = DateTime.Now.Ticks;
                                }
                                if (x.GetType() == typeof(BattleInfoUpdaterSystem))
                                {
                                    x.Enabled = true;
                                    x.InWork = false;
                                    x.LastEndExecutionTimestamp = DateTime.Now.Ticks;
                                }
                                if (x.GetType() == typeof(UserUpdaterSystem))
                                {
                                    x.Enabled = true;
                                    x.InWork = false;
                                    x.LastEndExecutionTimestamp = DateTime.Now.Ticks;
                                }
                            });
                        }
                        break;
                    case "tb":
                        if(true)
                        {
                            var userNetwork = new Network.Simple.Net.Server.User() { PlayerEntityId = Guid.NewGuid().GuidToLong() };
                            var userrow = new UserRow()
                            {
                                ActiveChatBanEndTime = 0,
                                Clan = -1,
                                Crystalls = 100000,
                                Email = new Random().RandomString(10),
                                EmailVerified = true,
                                GarageJSONData = DatabaseQuery.NewbieAccountGarage,
                                GlobalScore = 193000,
                                HardwareId = new Random().RandomString(10),
                                HardwareToken = new Random().RandomString(10),
                                id = -1,
                                Karma = 0,
                                LastDatetimeGetDailyBonus = 0,
                                LastIp = "127.0.0.1",
                                Password = "c20ad4d76fe97759aa27a0c99bff6710",
                                Rank = 14,
                                RankScore = 1000,
                                RegistrationDate = -1,
                                TermlessBan = false,
                                UserGroup = "admin",
                                UserLocation = "en",
                                Username = "bot_" + new Random().RandomString(8)
                            };

                            Console.WriteLine("enter count of battles");
                            var battlesCount = Console.ReadLine();

                            var entityUser = new UserTemplate().CreateEntity(userrow, userNetwork.PlayerEntityId);

                            ManagerScope.eventManager.OnEventAdd(new UserLogged()
                            {
                                networkSocket = userNetwork,
                                EntityOwnerId = userNetwork.PlayerEntityId,
                                userRelogin = false,
                                userEntity = entityUser,
                                actionAfterLoggin = (entity) =>
                                {
                                    for(int i = 0; i < int.Parse(battlesCount); i++)
                                    {
                                        ManagerScope.eventManager.OnEventAdd(new CreateBattleEvent()
                                        {
                                            BattleCustomName = "autotest",
                                            BattleMode = "dm",
                                            BattleRealName = "Дуэль",
                                            BattleTimeMinutes = 222,
                                            BattleWinGoalValue = 5,
                                            DamageScalingCoeficient = 1,
                                            dressingUpTimeoutSeconds = 240,
                                            enableAutoPeaceOnSuperDrop = false,
                                            enableBattleAutoEnding = true,
                                            enableCrystalDrop = true,
                                            enableDressingUp = true,
                                            enableMicroUpgrade = true,
                                            enablePlayerAutoBalance = true,
                                            enablePlayerSupplies = true,
                                            enableResists = true,
                                            enableSuperDrop = true,
                                            enableSupplyDrop = true,
                                            enableTeamKilling = false,
                                            enableUnlimitedUserSupply = false,
                                            GameMapGroupName = "Дуэль",
                                            EntityOwnerId = entity.instanceId,
                                            GravityScaling = 1,
                                            HealthScalingCoeficient = 1,
                                            isCheatersBattle = false,
                                            isClosedBattle = false,
                                            isParkourBattle = false,
                                            isProBattle = false,
                                            isTestBoxBattle = false,
                                            isTournamentBattle = false,
                                            ListOfAcceptedConfigPathHull = new List<string>(),
                                            ListOfAcceptedConfigPathWeapon = new List<string>(),
                                            LuminosityStrength = 0,
                                            MapPath = "Data\\Maps\\Дуэль.xml",
                                            MassScaling = 1,
                                            MaximalPlayerRankValue = 30,
                                            MaxPlayers = 222,
                                            MinimalPlayerRankValue = 0
                                        });
                                    }
                                    
                                }
                            });
                        }
                        
                        break;
                    default:
                        Console.WriteLine($"Unknown Command '{input[0]}'");
                        break;
                }
            }
        }

        public static string[] CreateArgs(string commandLine)
        {
            StringBuilder argsBuilder = new StringBuilder(commandLine);
            bool inQuote = false;

            // Convert the spaces to a newline sign so we can split at newline later on
            // Only convert spaces which are outside the boundries of quoted text
            for (int i = 0; i < argsBuilder.Length; i++)
            {
                if (argsBuilder[i].Equals('"'))
                {
                    inQuote = !inQuote;
                }

                if (argsBuilder[i].Equals(' ') && !inQuote)
                {
                    argsBuilder[i] = '\n';
                }
            }

            // Split to args array
            string[] args = argsBuilder.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Clean the '"' signs from the args as needed.
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = ClearQuotes(args[i]);
            }

            return args;
        }
        static string ClearQuotes(string stringWithQuotes)
        {
            int quoteIndex;
            if ((quoteIndex = stringWithQuotes.IndexOf('"')) == -1)
            {
                // String is without quotes..
                return stringWithQuotes;
            }

            // Linear sb scan is faster than string assignemnt if quote count is 2 or more (=always)
            StringBuilder sb = new StringBuilder(stringWithQuotes);
            for (int i = quoteIndex; i < sb.Length; i++)
            {
                if (sb[i].Equals('"'))
                {
                    // If we are not at the last index and the next one is '"', we need to jump one to preserve one
                    if (i != sb.Length - 1 && sb[i + 1].Equals('"'))
                    {
                        i++;
                    }

                    // We remove and then set index one backwards.
                    // This is because the remove itself is going to shift everything left by 1.
                    sb.Remove(i--, 1);
                }
            }

            return sb.ToString();
        }

        public static void ConfigChecker()
        {

        }
    }

    public static class ServerInstanceStart
    {
        static bool CheckParamCount(string name, int need, int actual)
        {
            if (actual != need)
            {
                Console.WriteLine($"-{name}: Parameter count ({actual}) is {(actual < need ? "less" : "more")} than expected ({need}).");
                return false;
            }
            return true;
        }

        static void Help()
        {
            Console.WriteLine("-r,   --run                 ip, port, maxPlayers Start server.\n" +
                              "-nhm, --disable-height-maps                      Disable loading of height maps.\n" +
                              "-np,  --disable-ping                             Disable sending of ping messages.\n" +
                              "-t,   --enable-tracing                           Enable packet tracing (works only in debug builds).\n" +
                              "-st,  --enable-stack-trace                       Enable outputting command stack trace of commands (works only with packet tracing enabled).\n" +
                              "-h,   --help                                     Display this help.");
        }

        public static void MainServerInstanceStart(string[] args)
        {
            if (args.Length == 0)
            {
                Help();
                return;
            }

            var additionalArgs = CommandLine.Parse(args);

            if (additionalArgs == null)
            {
                Console.WriteLine("Parameters are not valid.");
                return;
            }

            ServerSettings settings = new();

            HashSet<string> uniqueArgs = new();

            try
            {
                foreach (var pair in additionalArgs)
                {
                    if (!uniqueArgs.Add(pair.Key))
                    {
                        Console.WriteLine($"Duplicate parameter: {pair.Key}");
                        return;
                    }

                    switch (pair.Key)
                    {
                        case "r":
                        case "run":
                            if (!CheckParamCount(pair.Key, 3, pair.Value.Length)) return;
                            settings.IPAddress = IPAddress.Parse(pair.Value[0]);
                            settings.Port = Int16.Parse(pair.Value[1]);
                            settings.MaxPlayers = Int32.Parse(pair.Value[2]);
                            break;
                        case "nhm":
                        //case "-disable-height-maps":
                        //    if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                        //    settings.DisableHeightMaps = true;
                        //    break;
                        case "nhb":
                        case "np":
                        case "disable-ping":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.DisablePingMessages = true;
                            break;
                        case "t":
                        //case "-enable-tracing":
                        //    if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                        //    settings.EnableTracing = true;
                        //    break;
                        case "st":
                        case "enable-stack-trace":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.EnableCommandStackTrace = true;
                            break;
                        case "disable-map-bounds":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.MapBoundsInactive = true;
                            break;
                        case "super-cool-container-active":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.SuperMegaCoolContainerActive = true;
                            break;
                        case "test-server":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.TestServer = true;
                            break;
                        case "h":
                        case "help":
                            Help();
                            //return;
                            break;
                        default:
                            Console.WriteLine($"Unknown parameter: {pair.Key}");
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (settings.IPAddress == null)
            {
                Console.WriteLine("No run parameters specified.");
                return;
            }

            Server.Instance = new Server
            {
                Settings = settings,
                //Database = new LocalDatabase()
            };
            Server.Instance.Start();
        }
    }

    internal static class CommandLine
    {
        public static Dictionary<string, string[]> Parse(string[] args)
        {
            Dictionary<string, string[]> parameters = new();

            if (args.Length == 0)
                return parameters;

            string key = null;
            List<string> values = new();

            for (int i = 0; i < args.Length; i++)
            {
                string current = args[i];

                if (current[0] == '-')
                {
                    if (key != null)
                    {
                        parameters.Add(key, values.ToArray());
                        values.Clear();
                    }

                    key = current[1..];
                    continue;
                }
                else if (key == null)
                {
                    return null;
                }

                values.Add(current);
            }

            parameters.Add(key, values.ToArray());

            return parameters;
        }
    }
}
