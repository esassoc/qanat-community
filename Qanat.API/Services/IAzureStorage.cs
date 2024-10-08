using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services;

public interface IAzureStorage
{
    /// <summary>
    /// This method uploads a file submitted with the request
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <param name="blobFilename">Filename</param>
    /// <param name="stream">stream to upload</param>
    /// <returns>Blob with status</returns>
    Task<BlobResponseDto> UploadAsync(string containerName, string blobFilename, Stream stream);

    /// <summary>
    /// This method downloads a file with the specified filename
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <param name="blobFilename">Filename</param>
    /// <returns>Blob</returns>
    Task<BlobDto> DownloadAsync(string containerName, string blobFilename);

    /// <summary>
    /// This method deleted a file with the specified filename
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <param name="blobFilename">Filename</param>
    /// <returns>Blob with status</returns>
    Task<BlobResponseDto> DeleteAsync(string containerName, string blobFilename);

    /// <summary>
    /// This method returns a list of all files located in the container
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <returns>Blobs in a list</returns>
    Task<List<BlobDto>> ListAsync(string containerName);

    Task<BlobResponseDto> CopyAsync(string catalogContainerName, string originalBlobName, string newBlobName);
}