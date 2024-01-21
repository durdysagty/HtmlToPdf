using Renci.SshNet;
using Renci.SshNet.Sftp;
using SharedServices.Interfaces;
using System.Text;

namespace SharedServices
{
    public class FileRepository : IFileRepository, IDisposable
    {
        private SftpClient? _sftpClient;
        private readonly string _folder;

        public FileRepository(SftpClient sftpClient, string folder)
        {
            _sftpClient = sftpClient;
            _folder = folder;
        }

        public async Task<string> GetFile(string folder, string dir, string fileName, CancellationToken cancellationToken)
        {
            if (!_sftpClient!.IsConnected)
                await _sftpClient!.ConnectAsync(cancellationToken);
            string path = $"{folder}{dir}/{fileName}";
            SftpFileStream fileStream = _sftpClient.OpenRead(path);
            using StreamReader reader = new(fileStream, Encoding.UTF8);
            string htmlFile = await reader.ReadToEndAsync(cancellationToken);
            return htmlFile;
        }

        public async Task UploadAsync(Stream stream, string dirName, string fileName, CancellationToken cancellationToken)
        {
            if (!_sftpClient!.IsConnected)
                await _sftpClient!.ConnectAsync(cancellationToken);
            string dir = $"{_folder}{dirName}";
            if (!_sftpClient.Exists($"{dir}"))
                _sftpClient.CreateDirectory(dir);
            //TODO: check if file exists create new fileName
            await Task.Run(() => _sftpClient.UploadFile(stream, $"{dir}/{fileName}"), cancellationToken);
        }

        public async Task DeleteFile(string folder, string dir, string fileName, CancellationToken cancellationToken)
        {
            if (!_sftpClient!.IsConnected)
                await _sftpClient!.ConnectAsync(cancellationToken);
            string path = $"{folder}{dir}/{fileName}";
            await _sftpClient.DeleteFileAsync(path, cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_sftpClient != null)
                {
                    _sftpClient.Disconnect();
                    _sftpClient.Dispose();
                    _sftpClient = null;
                }
            }
        }
    }
}
