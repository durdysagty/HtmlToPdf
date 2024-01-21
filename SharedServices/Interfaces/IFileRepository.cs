namespace SharedServices.Interfaces
{
    public interface IFileRepository
    {
        Task<string> GetFile(string folder, string dir, string fileName, CancellationToken cancellationToken); 
        Task UploadAsync(Stream stream, string dirName, string fileName, CancellationToken cancellationToken);
        Task DeleteFile(string folder, string dir, string fileName, CancellationToken cancellationToken);
    }
}
