using FileLoader.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using SharedServices.Interfaces;
using System.Net;

namespace FileLoader.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileLoadController : ControllerBase
    {

        private readonly ILogger<FileLoadController> _logger;
        private readonly IFileRepository _fileRepository;
        private readonly IBackgroundFileQueue _backgroundFileQueue;

        public FileLoadController(ILogger<FileLoadController> logger, IFileRepository fileRepository, IBackgroundFileQueue backgroundFileQueue)
        {
            _logger = logger;
            _fileRepository = fileRepository;
            _backgroundFileQueue = backgroundFileQueue;
        }
        [HttpPost]
        public async Task<IActionResult> Post(string signalR, string clientId, IFormFile file, CancellationToken cancellationToken)
        {
            //TODO: handle large files
            if (file == null || file.Length == 0)
                return BadRequest("Uploaded file is empty or null.");

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".html" && extension != ".htm")
                return BadRequest("Only HTML files are allowed.");
            //if (!HttpContext.Request.Headers.TryGetValue("X-User-Folder", out StringValues headers))
            //    return Ok("REQUEST_A_FOLDER");
            try
            {
                //string folder = headers[0]!.ToString();
                string trustedFileName = WebUtility.HtmlEncode(file.FileName);
                await _fileRepository.UploadAsync(file.OpenReadStream(), clientId, trustedFileName, cancellationToken);
                Signal signal = new(signalR, clientId, trustedFileName);
                _backgroundFileQueue.EnqueueFile(signal);
                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error: {message}", ex.Message);
#if !DEBUG
                return StatusCode(500, "Internal server error");
#else
                return StatusCode(500, $"{ex.Message}");
#endif
            }
        }
    }
}
