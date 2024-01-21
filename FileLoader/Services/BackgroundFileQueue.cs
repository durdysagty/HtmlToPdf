using FileLoader.Services.Interfaces;
using Models;
using System.Collections.Concurrent;

namespace FileLoader.Services
{
    public class BackgroundFileQueue: IBackgroundFileQueue
    {
        private readonly ConcurrentQueue<string> _paths = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void EnqueueFile(Signal signal)
        {
            _paths.Enqueue(signal.ToString());
            _signal.Release();
        }

        public async Task<string?> DequeueFileAsync(CancellationToken token)
        {
            await _signal.WaitAsync(token);
            _paths.TryDequeue(out string? path);
            return path;
        }
    }
}
