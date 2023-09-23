using System.Threading;
using System.Threading.Tasks;

using Serilog;

using UTanksServer.Extensions;

namespace UTanksServer.Services {
  public interface IExitWaitService {
    public void Exit();
    public Task WaitAsync();
  }

  [UTanksServer.ECS.ECSCore.Service]
  public class ExitWaitService : IExitWaitService {
    private static readonly ILogger Logger = Log.Logger.ForType<ExitWaitService>();

    private readonly SemaphoreSlim _semaphore;

    public ExitWaitService() {
      _semaphore = new SemaphoreSlim(0);
    }

    public void Exit() {
      Logger.Information("Exiting...");
      _semaphore.Release();
    }

    public async Task WaitAsync() => await _semaphore.WaitAsync(Timeout.InfiniteTimeSpan);
  }
}
