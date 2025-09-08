using Azure;

namespace Qanat.GDALAPI.Services;

public interface IAzureStorage
{
    Task<Response> DownloadToAsync(string containerName, string blobFilename, string location);
    Task<bool> ExistsAsync(string containerName, string blobFilename);
    Task<bool> UploadAsync(string containerName, string blobFilename, StreamContent stream);
}