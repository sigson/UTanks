using System;
using System.IO;
using System.Threading.Tasks;

using Serilog;

using YamlDotNet.Serialization;

using EmbedIO;
using EmbedIO.WebApi;
using EmbedIO.Routing;

using UTanksServer.Extensions;
using UTanksServer.Services.Servers.Static.Models;

namespace UTanksServer.Services.Servers.Static {
  public class ConfigController : WebApiController {
    private static readonly ILogger Logger = Log.Logger.ForType<ConfigController>();

    private readonly IConfigService _configService;
    private readonly IClientConfigService _clientConfigService;

    private readonly Uri _staticBaseUri;

    public ConfigController(IConfigService configService, IClientConfigService clientConfigService) {
      _configService = configService;
      _clientConfigService = clientConfigService;

      _staticBaseUri = new Uri($"http://{_configService.StaticServerConfig.PublicAddress}:{_configService.StaticServerConfig.Port}/");
    }

    [Route(HttpVerbs.Get, "/startup/public.yml")]
    public async Task GetConfig() {
      await using MemoryStream stream = new MemoryStream();
      await using TextWriter writer = new StreamWriter(stream);
      Serializer serializer = new Serializer();

      serializer.Serialize(writer, new StartupConfig() {
        InitUri = new Uri(_staticBaseUri, "/config/init.yml"),
        StateUri = new Uri(_staticBaseUri, "/state/tankixprod_state.yml"),

        ClientVersion = "master-48606"
      });

      await writer.FlushAsync();
      await Response.SendPlain(stream);
    }

    [Route(HttpVerbs.Get, "/config/init.yml")]
    public async Task GetInit() {
      await using MemoryStream stream = new MemoryStream();
      await using TextWriter writer = new StreamWriter(stream);
      Serializer serializer = new Serializer();

      serializer.Serialize(writer, new InitConfig() {
        ConfigVersion = "master-48606",
        BundleDbVersion = "master-48606",

        Host = _configService.GameServerConfig.PublicAddress.ToString(),
        Port = _configService.GameServerConfig.Port,

        ConfigsUri = new Uri(_staticBaseUri, "/config"),
        StateFileUri = new Uri(_staticBaseUri, "/state"),

        ResourcesUri = new Uri(_staticBaseUri, "/resources"),
        UpdateConfigUri = new Uri(_staticBaseUri, "/update/{BuildTarget}.yml")
      });

      await writer.FlushAsync();
      await Response.SendPlain(stream);
    }

    [Route(HttpVerbs.Get, "/state/tankixprod_state.yml")]
    public async Task GetState() {
      await using MemoryStream stream = new MemoryStream();
      await using TextWriter writer = new StreamWriter(stream);
      Serializer serializer = new Serializer();

      serializer.Serialize(writer, new StateConfig() {
        State = 0
      });

      await writer.FlushAsync();
      await Response.SendPlain(stream);
    }

    [Route(HttpVerbs.Get, "/update/{buildTarget}.yml")]
    public async Task GetUpdate(string buildTarget) {
      await using MemoryStream stream = new MemoryStream();
      await using TextWriter writer = new StreamWriter(stream);
      Serializer serializer = new Serializer();

      serializer.Serialize(writer, new UpdateConfig() {
        DistributionUri = new Uri(_staticBaseUri, $"/update/{buildTarget}.tar.gz"),
        ClientVersion = "master-48606",

        Executable = "tankix-update.exe"
      });

      await writer.FlushAsync();
      await Response.SendPlain(stream);
    }

    [Route(HttpVerbs.Get, "/config/{version}/{language}/config.tar.gz")]
    public async Task GetConfig(string version, string language) {
      Logger.Debug(
        "Get client config (version: {Version}, language: {Language})",
        version,
        language
      );

      await Response.SendBinary(_clientConfigService.GetArchiveStream(version));
    }
  }
}
