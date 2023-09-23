using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UTanksServer.Core.Logging;
using UTanksServer.Services.Servers.Game;

namespace UTanksServer.Core
{
    public class ServerConnection
    {
        public Server Server { get; }

        public ServerConnection(Server server)
        {
            Server = server;
        }

        public void Start(IPAddress ip, short port, int poolSize)
        {
            if (IsStarted) return;
            IsStarted = true;

            //ServerMapInfo = JsonSerializer.Deserialize<Dictionary<string, MapInfo>>(
            //    File.ReadAllText(ServerMapInfoLocation), new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //    IncludeFields = true
            //});
            //ContainerInfos = JsonSerializer.Deserialize<Dictionary<string, ContainerInfo.ContainerInfo>>(
            //    File.ReadAllText(ContainerInfoLocation), new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //    IncludeFields = true
            //});

            //if (!Server.Instance.Settings.DisableHeightMaps)
            //{
            //    Logger.Log("Loading height maps...");
            //    HeightMaps = JsonSerializer.Deserialize<Dictionary<string, HeightMap>>(
            //        File.ReadAllText(HeightMapInfoLocation), new JsonSerializerOptions
            //    {
            //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //        IncludeFields = true
            //    });

            //    foreach (HeightMapLayer layer in HeightMaps.Values.SelectMany(map => map.Layers))
            //    {
            //        Logger.Trace($"Reading {layer.Path}...");
            //        layer.Image = Image.Load<Rgb24>(Path.Combine(Directory.GetCurrentDirectory(), "Library", layer.Path));
            //    }
            //    Logger.Log("Height maps loaded");
            //}

            //MaxPoolSize = poolSize;

            //Config.Init();


            //new Thread(() => StateServer(ip)) { Name = "State Server" }.Start();

            //            new Thread(BattleLoop) { Name = "Battle Thread" }.Start();
            //            new Thread(PlayerLoop) { Name = "Player Thread" }.Start();

            //#if DEBUG
            //            if (!Server.Instance.Settings.DisablePingMessages)
            //#endif
            //                new Thread(PingChecker) { Name = "Ping Checker" }.Start();

            //new Thread(() => AcceptPlayers(ip, port, MaxPoolSize)) { Name = "Acceptor" }.Start();//wtf is going oooon
            Logger.Log("Server is started.");
        }

        /// <summary>
        /// Stops server.
        /// </summary>
        public void StopServer()
        {
            if (!IsStarted) return;
            IsStarted = false;

            acceptorSocket.Close();
            httpListener.Close();

            Pool.ForEach(player => player.Dispose());
            Pool.Clear();
        }

        /// <summary>
        /// Adds player to pool.
        /// </summary>
        /// <param name="socket"></param>
        private void AddPlayer(Socket socket)
        {
            int freeIndex = Pool.FindIndex(player => !player.IsActive);

            if (freeIndex != -1)
            {
                Pool[freeIndex] = new Player(Server, socket);
            }
            else if (PlayerCount < MaxPoolSize)
            {
                Pool.Add(new Player(Server, socket));
            }
            else
            {
                socket.Close();
                Logger.Warn("Server is full!");
            }
        }

        /// <summary>
        /// Waits for new clients.
        /// </summary>
        private void AcceptPlayers(IPAddress ip, short port, int PoolSize)
        {
            using (acceptorSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    acceptorSocket.Bind(new IPEndPoint(ip, port));
                    acceptorSocket.Listen(PoolSize);
                }
                catch (SocketException e)
                {
                    HandleError(e);
                    return;
                }

                while (true)
                {
                    if (!IsStarted) return;

                    Socket socket = null;
                    bool accepted = false;
                    try
                    {
                        socket = acceptorSocket.Accept();
                        accepted = true;

                        AddPlayer(socket);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);

                        if (accepted) socket.Close();
                    }
                }
            }
        }

        /// <summary>
        /// HTTP state server.
        /// </summary>
        /// <param name="ip">IP address.</param>
        private void StateServer(IPAddress ip)
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/StateServer";

            using (httpListener = new HttpListener())
            {
                httpListener.Prefixes.Add($"http://{(Equals(ip, IPAddress.Any) ? "+" : ip.ToString())}:8080/");

                try
                {
                    httpListener.Start();
                }
                catch (HttpListenerException e)
                {
                    HandleError(e);
                    return;
                }

                while (true)
                {
                    if (!IsStarted) return;
                    HttpListenerContext context;

                    try
                    {
                        context = httpListener.GetContext();
                    }
                    catch (HttpListenerException e)
                    {
                        Logger.Error(e);
                        return;
                    }

                    new Task(() =>
                    {
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        Logger.Debug($"Request from {request.RemoteEndPoint}: {request.HttpMethod} {request.Url.PathAndQuery}");
                        if (request.HttpMethod != "GET")
                        {
                            response.StatusCode = 405;
                            response.OutputStream.Close();
                            return;
                        }

                        byte[] data;
                        try
                        {
                            if (Path.GetExtension(request.Url.LocalPath) == ".yml")
                            {
                                string unformatted = File.ReadAllText(rootPath + request.Url.LocalPath);
                                data = Encoding.UTF8.GetBytes(unformatted.Replace("*ip*", request.Url.Host));
                            }
                            else
                            {
                                data = File.ReadAllBytes(rootPath + request.Url.LocalPath);
                            }
                        }
                        catch (Exception e)
                        {
                            data = Array.Empty<byte>();

                            response.StatusCode = e switch
                            {
                                FileNotFoundException or DirectoryNotFoundException => 404,
                                UnauthorizedAccessException => 403,
                                _ => 500
                            };

                            if (response.StatusCode == 500)
                                Logger.Error(e);
                        }

                        response.ContentLength64 = data.Length;

                        Stream output = response.OutputStream;
                        output.Write(data, 0, data.Length);

                        output.Close();
                    }).Start();
                }
            }
        }

        //private void PingChecker()
        //{
        //    sbyte id = 0;

        //    while (true)
        //    {
        //        if (!IsStarted) return;

        //        foreach (Player player in Pool)
        //        {
        //            player.Connection.PingSendTime = DateTimeOffset.UtcNow;
        //            player.SendEvent(new PingEvent(player.Connection.PingSendTime.ToUnixTimeMilliseconds(), id), player.ClientSession);
        //        }

        //        Thread.Sleep(10000);
        //    }
        //}

//        private void BattleLoop()
//        {
//            Stopwatch stopwatch = new();

//            try
//            {
//                while (true)
//                {
//                    if (!IsStarted) return;

//                    stopwatch.Restart();

//                    foreach (Battle battle in BattlePool.ToArray())
//                        battle.Tick(LastBattleTickDuration);
//                    MatchMaking.Tick();

//                    stopwatch.Stop();

//                    TimeSpan spentOnBattles = stopwatch.Elapsed;

//                    stopwatch.Start();
//                    if (spentOnBattles.TotalSeconds < BattleTickDuration)
//                        Thread.Sleep(TimeSpan.FromSeconds(BattleTickDuration) - spentOnBattles);

//                    stopwatch.Stop();
//                    LastBattleTickDuration = stopwatch.Elapsed.TotalSeconds;
//                }
//            }
//            catch (Exception e)
//            {
//#if DEBUG
//                Debugger.Launch();
//#endif
//                HandleError(e);
//            }
//        }

//        private void PlayerLoop()
//        {
//            Stopwatch stopwatch = new();

//            try
//            {
//                while (true)
//                {
//                    if (!IsStarted) return;

//                    stopwatch.Restart();
//                    foreach (Player player in Pool.ToArray())
//                        if (player is {IsActive: true})
//                            player.Tick();

//                    stopwatch.Stop();

//                    TimeSpan spentOnPlayers = stopwatch.Elapsed;

//                    stopwatch.Start();
//                    if (spentOnPlayers.TotalSeconds < PlayerTickDuration)
//                        Thread.Sleep(TimeSpan.FromSeconds(PlayerTickDuration) - spentOnPlayers);

//                    stopwatch.Stop();
//                    LastPlayerTickDuration = stopwatch.Elapsed.TotalSeconds;
//                }
//            }
//            catch (Exception e)
//            {
//#if DEBUG
//                Debugger.Launch();
//#endif
//                HandleError(e);
//            }
//        }

        private static void HandleError(Exception exception) => Server.Instance.HandleError(exception);

        // Player pool.
        public List<Player> Pool { get; } = new();
        private int MaxPoolSize;

        // Client accept thread.
        private Socket acceptorSocket;

        // HTTP state server thread.
        private HttpListener httpListener;

        //public static List<Battle> BattlePool { get; } = new List<Battle>();

        public static double LastBattleTickDuration { get; private set; }
        private const int BattleTickRate = 100;
        private const double BattleTickDuration = 1.0 / BattleTickRate;

        public static double LastPlayerTickDuration { get; set; }
        private const int PlayerTickRate = 1000;
        private const double PlayerTickDuration = 1.0 / PlayerTickRate;

        // Server state.
        public bool IsStarted { get; private set; }

        public int PlayerCount = 0;

        //private static string ServerMapInfoLocation => "Library/ServerMapInfo.json";
        //private static string ContainerInfoLocation => "Library/BlueprintContainers.json";
        //private static string HeightMapInfoLocation => "Library/HeightMaps.json";
        //public static Dictionary<string, MapInfo> ServerMapInfo { get; private set; }
        //public static Dictionary<string, ContainerInfo.ContainerInfo> ContainerInfos { get; private set; }
        //public static Dictionary<string, HeightMap> HeightMaps { get; private set; }
    }
}
