using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace UTanksServer.Services.Servers.Game {
  public class AsyncQueue<T> : IAsyncEnumerable<T> {
    private readonly SemaphoreSlim _enumerationSemaphore = new SemaphoreSlim(1);
    private readonly BufferBlock<T> _bufferBlock = new BufferBlock<T>();

    public void Enqueue(T item) => _bufferBlock.Post(item);

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default) {
      // We lock this so we only ever enumerate once at a time.
      // That way we ensure all items are returned in a continuous
      // fashion with no 'holes' in the data when two foreach compete.
      await _enumerationSemaphore.WaitAsync(token);
      try {
        // Return new elements until cancellationToken is triggered.
        while(true) {
          // Make sure to throw on cancellation so the Task will transfer into a canceled state
          token.ThrowIfCancellationRequested();
          yield return await _bufferBlock.ReceiveAsync(token);
        }
      } finally {
        _enumerationSemaphore.Release();
      }
    }
  }
}
