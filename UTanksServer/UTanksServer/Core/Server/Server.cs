using System;
using System.Collections.Generic;
using System.Linq;
using UTanksServer.Core.Database;
using UTanksServer.Core.Logging;
using UTanksServer.ECS.Entities;
using UTanksServer.Services.Servers.Game;

namespace UTanksServer.Core
{
    public class Server
    {
        public static Server Instance { get; set; }

        public ServerConnection Connection { get; private set; }

        public ServerSettings Settings { get; init; }
        //public IDatabase Database { get; init; }
        public ServerData ServerData { get; private set; }
        public Action UserErrorHandler { get; init; }
        //public ModuleRegistry ModuleRegistry { get; }
        public List<PlayerData> StoredPlayerData { get; } = new();


        public static ServerConfig Config => ServerConfig.Instance;
        public static DatabaseNetwork DatabaseNetwork => DatabaseNetwork.Instance;

        public Server()
        {
            //ModuleRegistry = new ModuleRegistry();
            //ModuleRegistry.Register(Modules.ModuleToType);
        }

        public void Start()
        {
            Logger.Log("Starting server...");

            // The database needs the config so load it
            ServerConfig.Load("DBConfig.json");
            // Connect to the database only if there is no current connection or if it is not in a ready state
            if (DatabaseNetwork.Instance == null ||
                !DatabaseNetwork.Instance.IsReady)
                new DatabaseNetwork().Connect(null);

            ServerData = new ServerData();
            Connection = new ServerConnection(this);
            Connection.Start(Settings.IPAddress, Settings.Port, Settings.MaxPlayers);
            //Database.Startup();
        }

        public Player FindPlayerByUid(long uid) =>
            Connection.Pool.FirstOrDefault(player => player.IsLoggedIn && player.UserEntity.instanceId == uid);

        //public PlayerEntity FindPlayerByUsername(string username) =>
        //    Connection.Pool.FirstOrDefault(controlledPlayer =>
        //        controlledPlayer.User != null && controlledPlayer.Data.Username == username);

        //public BattleEntity FindBattleById(long? lobbyId = null, long? battleId = null)
        //{
        //    if (lobbyId == null && battleId == null)
        //        throw new ArgumentNullException("No valid argument is specified.");

        //    foreach (BattleEntity battle in ServerConnection.BattlePool)
        //    {
        //        if (battle.BattleLobbyEntity.EntityId == lobbyId || battle.BattleEntity.EntityId == battleId)
        //            return battle;
        //    }

        //    return null;
        //}

        public void Stop()
        {
            if (!Connection.IsStarted) return;

            Logger.Log("Stopping server...");

            Connection.StopServer();
            //Database.Shutdown();

            Logger.Log("Server is stopped.");
        }

        internal void HandleError(Exception exception)
        {
            Logger.Error($"Fatal error: {exception}");
            Stop();
            UserErrorHandler?.Invoke();
        }
    }
}
