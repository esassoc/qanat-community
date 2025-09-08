using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Qanat.Common;
using Qanat.EFModels.Entities;

namespace Qanat.API.Services
{
    public class FileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly IAzureStorage _azureStorage;

        public const string FileContainerName = "files";

        public FileService(ILogger<FileService> logger, IAzureStorage azureStorage)
        {
            _logger = logger;
            _azureStorage = azureStorage;
        }

        public string MakePrettyFileName(string filename)
        {
            var replacedFileName = filename
                .Replace("(", "")
                .Replace(")", "")
                .Replace(" ", "")
                .Replace(",", "")
                .Replace(":", "")
                .Replace(";", "")
                .Replace("\"", "")
                .Replace("&", "")
                .Replace("#", "")
                .Replace("'", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(" ", "");

            return replacedFileName;
        }

        public async Task<Stream> GetFileStreamFromBlobStorage(string canonicalName)
        {
            try
            {
                var blobDto = await _azureStorage.DownloadAsync(FileContainerName, canonicalName);
                return blobDto.Content;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return null;
        }

        public async Task SaveFileStreamToAzureBlobStorage(string canonicalName, Stream stream)
        {
            _logger.LogInformation($"Saving file stream {canonicalName} to {FileContainerName}");
            stream.Seek(0, SeekOrigin.Begin);
            await _azureStorage.UploadAsync(FileContainerName, canonicalName, stream);
        }

        public async Task<FileResource> CreateFileResource(QanatDbContext dbContext, IFormFile file, int createUserID)
        {
            _logger.LogInformation($"Creating new File Resource from IFormFile {file.Name}");
            var canonicalName = Guid.NewGuid();
            await using var stream = file.OpenReadStream();
            stream.Seek(0, SeekOrigin.Begin);
            var uploadedFile = await _azureStorage.UploadAsync(FileContainerName, canonicalName.ToString(), stream);

            if (uploadedFile.Error)
            {
                throw new Exception(
                    $"There was an error uploading the FormFile \"{file.FileName}\" to blob storage with the canonical name \"{canonicalName}\". Error Details: {uploadedFile.Status}");
            }

            var fileNameSegments = file.FileName.Split(".");
            var newFileResource = new FileResource()
            {
                CreateDate = DateTime.UtcNow,
                CreateUserID = createUserID,
                FileResourceCanonicalName = canonicalName.ToString(),
                FileResourceGUID = canonicalName,
                OriginalFileExtension = fileNameSegments.Last(),
                OriginalBaseFilename = String.Join(".", fileNameSegments.Take(fileNameSegments.Length - 1)),
            };

            dbContext.FileResources.Add(newFileResource);
            await dbContext.SaveChangesAsync();
            await dbContext.Entry(newFileResource).ReloadAsync();
            return newFileResource;
        }

        public async Task<FileResource> CreateFileResource(QanatDbContext dbContext, Stream stream, string fullFileName, int createUserID)
        {
            var canonicalName = Guid.NewGuid();
            stream.Seek(0, SeekOrigin.Begin);
            var uploadedFile = await _azureStorage.UploadAsync(FileContainerName, canonicalName.ToString(), stream);

            if (uploadedFile.Error)
            {
                throw new Exception($"There was an error uploading the FormFile \"{fullFileName}\" to blob storage with the canonical name \"{canonicalName}\". Error Details: {uploadedFile.Status}");
            }

            var fileNameSegments = fullFileName.Split(".");
            var newFileResource = new FileResource()
            {
                CreateDate = DateTime.UtcNow,
                CreateUserID = createUserID,
                FileResourceCanonicalName = canonicalName.ToString(),
                FileResourceGUID = canonicalName,
                OriginalFileExtension = fileNameSegments.Last(),
                OriginalBaseFilename = String.Join(".", fileNameSegments.Take(fileNameSegments.Length - 1)),
            };

            dbContext.FileResources.Add(newFileResource);
            await dbContext.SaveChangesAsync();
            await dbContext.Entry(newFileResource).ReloadAsync();
            return newFileResource;
        }

        public async Task<FileStream> CreateZipFileFromFileResources(List<FileResource> fileResources)
        {
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileResource in fileResources)
                {
                    var fileInZip = archive.CreateEntry($"{fileResource.OriginalBaseFilename}.{fileResource.OriginalFileExtension}");
                    var blobStream = await GetFileStreamFromBlobStorage(fileResource.FileResourceCanonicalName);

                    await using var s = fileInZip.Open();
                    await blobStream.CopyToAsync(s);
                }
            }

            var disposableTempFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".zip");
            var fileStream = new FileStream(disposableTempFile.FileInfo.FullName, FileMode.Create);
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(fileStream);
            fileStream.Seek(0, SeekOrigin.Begin);
            return fileStream;
        }

        public async void DeleteFileStreamFromBlobStorage(string canonicalName)
        {
            try
            {
                await _azureStorage.DeleteAsync(FileContainerName, canonicalName);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}