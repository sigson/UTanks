using System.IO;
using System.Net;
using System.Threading.Tasks;

using UTanksServer.Database;
using UTanksServer.Extensions;
using UTanksServer.Services.Servers.Game;
using UTanksServer.Services.Servers.Static;

using Serilog;

namespace UTanksServer.Services {
  public interface IConfigService {
    public FileInfo StaticServerFile { get; }
    public FileInfo GameServerFile { get; }
    public FileInfo DatabaseFile { get; }

    public StaticServerConfig StaticServerConfig { get; }
    public GameServerConfig GameServerConfig { get; }
    public DatabaseConfig DatabaseConfig { get; }

    public Task Init();
  }

  [UTanksServer.ECS.ECSCore.Service]
  public class ConfigService : IConfigService {
    private static readonly ILogger Logger = Log.Logger.ForType<ConfigService>();

    public FileInfo StaticServerFile { get; } = new FileInfo("Config/StaticServer.json");
    public FileInfo GameServerFile { get; } = new FileInfo("Config/GameServer.json");
    public FileInfo DatabaseFile { get; } = new FileInfo("Config/Database.json");

    public StaticServerConfig StaticServerConfig { get; private set; } = null!;
    public GameServerConfig GameServerConfig { get; private set; } = null!;
    public DatabaseConfig DatabaseConfig { get; private set; } = null!;

    private readonly IJsonSerializerService _jsonSerializerService;
    private readonly IIpAddressUtilsService _ipAddressUtilsService;

    public ConfigService(IJsonSerializerService jsonSerializerService, IIpAddressUtilsService ipAddressUtilsService) {
      _jsonSerializerService = jsonSerializerService;
      _ipAddressUtilsService = ipAddressUtilsService;
    }

    public async Task Init() {
      Logger.Information("Loading configuration...");

      StaticServerConfig = await _jsonSerializerService.DeserializeJsonAsync<StaticServerConfig>(await StaticServerFile.ReadAsync());
      GameServerConfig = await _jsonSerializerService.DeserializeJsonAsync<GameServerConfig>(await GameServerFile.ReadAsync());
      DatabaseConfig = await _jsonSerializerService.DeserializeJsonAsync<DatabaseConfig>(await DatabaseFile.ReadAsync());

      IPAddress publicAddress = await _ipAddressUtilsService.GetPublicAddress();
      StaticServerConfig.PublicAddress = StaticServerConfig.Address.Equals(IPAddress.Any) ? publicAddress : StaticServerConfig.Address;
      GameServerConfig.PublicAddress = GameServerConfig.Address.Equals(IPAddress.Any) ? publicAddress : GameServerConfig.Address;

      Logger.Information("Public IP address: {PublicAddress}", publicAddress);
      Logger.Information("Static server endpoint: {Address}:{Port}", StaticServerConfig.Address, StaticServerConfig.Port);
      Logger.Information("Game server endpoint: {Address}:{Port}", GameServerConfig.Address, GameServerConfig.Port);

      Logger.Information("Initialized");
    }
  }
}
