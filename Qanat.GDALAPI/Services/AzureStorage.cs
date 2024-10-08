using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace Qanat.GDALAPI.Services
{
    /// <summary>
    /// This is a trimmed down version of the AzureStorage implementation in the CatalogAPI.
    /// </summary>
    public class AzureStorage : IAzureStorage
    {
        #region Dependency Injection / Constructor

        private readonly string _storageConnectionString;
        private readonly ILogger<AzureStorage> _logger;

        public AzureStorage(IOptions<GDALAPIConfiguration> apiConfiguration, ILogger<AzureStorage> logger)
        {
            _storageConnectionString = apiConfiguration.Value.AzureBlobStorageConnectionString;
            _logger = logger;
        }

        #endregion

        public async Task<Response> DownloadToAsync(string containerName, string blobFilename, string location)
        {
            // Get a reference to a container named in appsettings.json
            var client = new BlobContainerClient(_storageConnectionString, containerName);

            var file = client.GetBlobClient(blobFilename);
            try
            {
                // Check if the file exists in the container
                if (await file.ExistsAsync())
                {
                    return await file.DownloadToAsync(location);
                }
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                // Log error to console
                _logger.LogError($"File {blobFilename} was not found.");
            }

            // File does not exist, return null and handle that in requesting method
            return null;
        }

        public async Task<bool> ExistsAsync(string containerName, string blobFilename)
        {
            var client = new BlobContainerClient(_storageConnectionString, containerName);
            var file = client.GetBlobClient(blobFilename);
            return await file.ExistsAsync();
        }

        public async Task<bool> UploadAsync(string containerName, string blobFilename, StreamContent stream)
        {
            // Get a reference to a container named in appsettings.json and then create it
            var container = new BlobContainerClient(_storageConnectionString, containerName);
            //await container.CreateAsync();
            try
            {
                // Get a reference to the blob just uploaded from the API in a container from configuration settings
                var client = container.GetBlobClient(blobFilename);

                // Upload the file async
                await client.UploadAsync(await stream.ReadAsStreamAsync(), overwrite: true);
                return true;
            }
            // If the file already exists, we catch the exception and do not upload it
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                _logger.LogError(
                    $"File with name {blobFilename} already exists in container. Set another name to store the file in the container: '{containerName}.'");
                return false;
            }
            // If we get an unexpected error, we catch it here and return the error message
            catch (RequestFailedException ex)
            {
                // Log error to console and create a new response we can return to the requesting method
                _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
                return false;
            }
        }
    }
}