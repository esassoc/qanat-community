using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;
using Qanat.Models.Security;
using System.Net;

namespace Qanat.API.Controllers
{
    [ApiController]
    [RightsChecker]

    public class FileResourceController : SitkaController<FileResourceController>
    {
        private readonly FileService _fileService;

        public FileResourceController(QanatDbContext dbContext, ILogger<FileResourceController> logger, IOptions<QanatConfiguration> qanatConfiguration, FileService fileService) : base(dbContext, logger, qanatConfiguration)
        {
            _fileService = fileService;
        }

        [HttpGet("fileResources/{fileResourceGuidAsString}")]
        [WithRolePermission(PermissionEnum.FileResourceRights, RightsEnum.Read)]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DownloadFileResource(string fileResourceGuidAsString)
        {
            var fileResource = FileResources.GetByGuidString(_dbContext, fileResourceGuidAsString);

            if (fileResource != null)
            {
                var fileStream = await _fileService.GetFileStreamFromBlobStorage(fileResource.FileResourceCanonicalName);
                if (fileStream != null)
                {
                    var fileName = fileResource.OriginalBaseFilename;
                    var fileExtension = fileResource.OriginalFileExtension;
                    return DisplayFile(fileName, fileExtension, fileStream);
                }
            }

            // Unhappy path - return an HTTP 404
            // ---------------------------------
            var message = $"File resource not found in database. It may have been deleted.";
            _logger.LogError(message);
            return NotFound(message);
        }

        private IActionResult DisplayFile(string fileName, string fileExtension, Stream fileStream)
        {
            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = $"{fileName}.{fileExtension}",
                Inline = false
            };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(contentDisposition.FileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(fileStream, contentType, contentDisposition.FileName);
        }
    }
}
