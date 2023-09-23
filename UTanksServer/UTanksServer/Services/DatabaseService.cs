using System;
using System.Threading.Tasks;

using Serilog;

using UTanksServer.Database;
using UTanksServer.Extensions;
using UTanksServer.Services.Servers.Game;

namespace UTanksServer.Services {
  public interface IDatabaseService : IDisposable, IAsyncDisposable {
    public DatabaseContext Context { get; }

    public Task Init();
  }

  [UTanksServer.ECS.ECSCore.Service]
  public class DatabaseService : IDatabaseService {
    private static readonly ILogger Logger = Log.Logger.ForType<DatabaseService>();

    public DatabaseContext Context => _context ?? throw new InvalidOperationException("DatabaseContext is not initialized");

    private DatabaseContext? _context;

    private readonly IConfigService _configService;

    public DatabaseService(IConfigService configService) {
      _configService = configService;
    }

    public async Task Init() {
      _context = new DatabaseContext(_configService.DatabaseConfig);

      Logger.Information("Initialized");
    }

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if(!disposing) return;

      Context.Dispose();
    }


    public async ValueTask DisposeAsync() {
      await DisposeAsyncCore();

      Dispose(false);
      GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore() {
      await Context.DisposeAsync();
    }
  }
}
