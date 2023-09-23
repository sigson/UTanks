using System;
using System.Net;
using System.Threading.Tasks;

using Serilog;

using EmbedIO;
using EmbedIO.Files;
using EmbedIO.WebApi;
using EmbedIO.Actions;

using UTanksServer.Extensions;

namespace UTanksServer.Services.Servers.Static {
  public interface IStaticServer : IDisposable {
    public Task Start();
  }

  [UTanksServer.ECS.ECSCore.Service]
  public class StaticServer : IStaticServer {
    private static readonly ILogger Logger = Log.Logger.ForType<StaticServer>();

    private readonly WebServer _host;

    private readonly IConfigService _configService;
    private readonly IClientConfigService _clientConfigService;

    public StaticServer(IConfigService configService, IClientConfigService clientConfigService) {
      _configService = configService;
      _clientConfigService = clientConfigService;

      _host = new WebServer((options) => {
          IPAddress address = _configService.StaticServerConfig.Address;
          int port = _configService.StaticServerConfig.Port;

          options
            .WithUrlPrefix($"http://{(address.Equals(IPAddress.Any) ? "*" : address)}:{port}/")
            .WithMode(HttpListenerMode.EmbedIO);
        })
        .WithLocalSessionManager()
        .WithWebApi("/", (module) => {
          module.WithController(() => new ConfigController(_configService, _clientConfigService));
        })
        .WithStaticFolder("/", "Static/", true, (module) => {
          module.WithContentCaching();
        })
        .WithModule(new ActionModule("/", HttpVerbs.Any, (context) => context.SendDataAsync(new {
          Message = "Error"
        })));

      _host.StateChanged += (_, args) => {
        Logger.Verbose("WebServer new state: {State}", args.NewState);

        switch(args.NewState) {
          case WebServerState.Loading: {
            Log.Information("Starting...");
            break;
          }

          case WebServerState.Listening: {
            Logger.Information("Started with URL prefixes: {@Prefixes}", _host.Options.UrlPrefixes);
            break;
          }
        }
      };
    }

    public async Task Start() {
      _ = _host.RunAsync();

      Logger.Information("Initialized");
    }

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      _host.Dispose();
    }
  }
}
