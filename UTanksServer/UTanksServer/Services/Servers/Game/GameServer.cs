using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

using Serilog;

using UTanksServer.Extensions;
using UTanksServer.Services.Servers.Game.Connection;

namespace UTanksServer.Services.Servers.Game {
  public interface IGameServer {
    public Task Start();
  }

  public interface ICommand {
    public Task OnReceive(Player player);
  }

  [UTanksServer.ECS.ECSCore.Service]
  public class GameServer : IGameServer {
    private static readonly ILogger Logger = Log.Logger.ForType<GameServer>();

    public IPEndPoint Endpoint { get; }

    private readonly Socket _socket;
    private readonly List<IPlayerConnection> _clients;

    private readonly IConfigService _configService;

    public GameServer(IConfigService configService) {
      _configService = configService;

      Endpoint = new IPEndPoint(_configService.GameServerConfig.Address, _configService.GameServerConfig.Port);

      _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      _clients = new List<IPlayerConnection>();
    }

    public async Task Start() {
      _socket.Bind(Endpoint);
      _socket.Listen();

      ThreadPool.SetMinThreads(50, 50);

      _ = Task.Factory.StartNew(async () => {
        while(true) {
          IPlayerConnection connection = await AcceptClient();

          _ = Task.Run(async () => await connection.ReceivePackets());
          _ = Task.Run(async () => await connection.SendPackets());

          _ = Task.Run(async () => {
            await Task.Delay(1000); // TODO(Assasans): Find a better way to detect if it is HTTP request
            await connection.Init();
          });
        }
      }, TaskCreationOptions.LongRunning);

      Logger.Information("Started game server: {Address}:{Port}", Endpoint.Address, Endpoint.Port);
      Logger.Information("Initialized");
    }

    private async Task<IPlayerConnection> AcceptClient() {
      Logger.Verbose("Waiting for socket...");

      Socket clientSocket = await _socket.AcceptAsync();

      Player player = new Player();
            //PlayerSocketConnection connection = new PlayerSocketConnection(clientSocket);

            //player.Connection = connection;
            //connection.Player = player;

            //_clients.Add(connection);

            //Logger.WithPlayer(connection.Player).Verbose(
            //  "Accepted socket"
            //);

            //return connection;
            return null;
    }
  }
}
