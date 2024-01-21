using Models;

namespace FileLoader.Services.Interfaces
{
    public interface IBackgroundFileQueue
    {
        void EnqueueFile(Signal path);
        Task<string?> DequeueFileAsync(CancellationToken token);
    }
}
