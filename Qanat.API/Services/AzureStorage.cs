using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Qanat.Models.DataTransferObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Qanat.API.Services;

public class AzureStorage : IAzureStorage
{
    #region Dependency Injection / Constructor

    private readonly string _storageConnectionString;
    private readonly ILogger<AzureStorage> _logger;

    public AzureStorage(IConfiguration configuration, IOptions<QanatConfiguration> apiConfiguration, ILogger<AzureStorage> logger)
    {
        _storageConnectionString = apiConfiguration.Value.AzureBlobStorageConnectionString;
        _logger = logger;
    }

    #endregion

    public async Task<BlobResponseDto> DeleteAsync(string containerName, string blobFilename)
    {
        var client = new BlobContainerClient(_storageConnectionString, containerName);

        var file = client.GetBlobClient(blobFilename);

        try
        {
            // Delete the file
            await file.DeleteAsync();
        }
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            // File did not exist, log to console and return new response to requesting method
            _logger.LogError($"File {blobFilename} was not found.");
            return new BlobResponseDto { Error = true, Status = $"File with name {blobFilename} not found." };
        }

        // Return a new BlobResponseDto to the requesting method
        return new BlobResponseDto
        { Error = false, Status = $"File: {blobFilename} has been successfully deleted." };

    }

    public async Task<BlobDto> DownloadAsync(string containerName, string blobFilename)
    {
        // Get a reference to a container named in appsettings.json
        var client = new BlobContainerClient(_storageConnectionString, containerName);

        var file = client.GetBlobClient(blobFilename);
        try
        {
            // Get a reference to the blob uploaded earlier from the API in the container from configuration settings

            // Check if the file exists in the container
            if (await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();

                // Download the file details async
                var content = await file.DownloadContentAsync();

                // Add data to variables in order to return a BlobDto
                var contentType = content.Value.Details.ContentType;

                // Create new BlobDto with blob data from variables
                return new BlobDto { Content = data, Name = blobFilename, ContentType = contentType };
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

    public async Task<List<BlobDto>> ListAsync(string containerName)
    {
        // Get a reference to a container named in appsettings.json
        var container = new BlobContainerClient(_storageConnectionString, containerName);

        // Create a new list object for 
        var files = new List<BlobDto>();

        await foreach (var file in container.GetBlobsAsync())
        {
            // Add each file retrieved from the storage container to the files list by creating a BlobDto object
            var uri = container.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            files.Add(new BlobDto
            {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType
            });
        }

        // Return all files to the requesting method
        return files;
    }

    public async Task<BlobResponseDto> UploadAsync(string containerName, string blobFilename, Stream data)
    {
        // Create new upload response object that we can return to the requesting method
        BlobResponseDto response = new();

        // Get a reference to a container named in appsettings.json and then create it
        var container = new BlobContainerClient(_storageConnectionString, containerName);
        //await container.CreateAsync();
        try
        {
            // Get a reference to the blob just uploaded from the API in a container from configuration settings
            var client = container.GetBlobClient(blobFilename);

            // Upload the file async
            await client.UploadAsync(data);

            // Everything is OK and file got uploaded
            response.Status = $"File {blobFilename} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

        }
        // If the file already exists, we catch the exception and do not upload it
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            _logger.LogError(
                $"File with name {blobFilename} already exists in container. Set another name to store the file in the container: '{containerName}.'");
            response.Status =
                $"File with name {blobFilename} already exists. Please use another name to store your file.";
            response.Error = true;
            return response;
        }
        // If we get an unexpected error, we catch it here and return the error message
        catch (RequestFailedException ex)
        {
            // Log error to console and create a new response we can return to the requesting method
            _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
            response.Error = true;
            return response;
        }

        // Return the BlobUploadResponse object
        return response;

    }

    public async Task<BlobResponseDto> CopyAsync(string containerName, string originalBlobName, string newBlobName)
    {
        // Get a reference to a container named in appsettings.json and then create it
        var container = new BlobContainerClient(_storageConnectionString, containerName);
        // Create new upload response object that we can return to the requesting method
        BlobResponseDto response = new();

        try
        {
            // Create a BlobClient representing the source blob to copy.
            var sourceBlob = container.GetBlobClient(originalBlobName);

            // Ensure that the source blob exists.
            if (await sourceBlob.ExistsAsync())
            {
                // Lease the source blob for the copy operation 
                // to prevent another client from modifying it.
                var lease = sourceBlob.GetBlobLeaseClient();

                // Specifying -1 for the lease interval creates an infinite lease.
                await lease.AcquireAsync(TimeSpan.FromSeconds(-1));

                // Get a BlobClient representing the destination blob with a unique name.
                var destBlob = container.GetBlobClient(newBlobName);

                // Start the copy operation.
                await destBlob.StartCopyFromUriAsync(sourceBlob.Uri);

                // Update the source blob's properties.
                BlobProperties sourceProperties = await sourceBlob.GetPropertiesAsync();

                if (sourceProperties.LeaseState == LeaseState.Leased)
                {
                    // Break the lease on the source blob.
                    await lease.BreakAsync();
                }

                // Everything is OK and file got uploaded
                response.Status = $"File {newBlobName} Uploaded Successfully";
                response.Error = false;
                response.Blob.Uri = destBlob.Uri.AbsoluteUri;
                response.Blob.Name = destBlob.Name;
            }
        }
        // If the file already exists, we catch the exception and do not upload it
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            _logger.LogError(
                $"File with name {newBlobName} already exists in container. Set another name to store the file in the container: '{containerName}.'");
            response.Status =
                $"File with name {newBlobName} already exists. Please use another name to store your file.";
            response.Error = true;
            return response;
        }
        catch (RequestFailedException ex)
        {
            // Log error to console and create a new response we can return to the requesting method
            _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
            response.Error = true;
            return response;
        }

        // Return the BlobUploadResponse object
        return response;

    }
}
