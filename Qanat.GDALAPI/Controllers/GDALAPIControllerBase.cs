using Microsoft.AspNetCore.Mvc;
using Qanat.Common;
using Qanat.GDALAPI.Services;

namespace Qanat.GDALAPI.Controllers;

public abstract class GDALAPIControllerBase<T> : ControllerBase
{
    protected readonly ILogger<T> _logger;
    protected readonly GDALRunnerService _gdalRunnerService;
    protected readonly IAzureStorage _azureStorage;

    protected GDALAPIControllerBase(ILogger<T> logger, GDALRunnerService gdalRunnerService, IAzureStorage azureStorage)
    {
        _logger = logger;
        _gdalRunnerService = gdalRunnerService;
        _azureStorage = azureStorage;
    }

    protected async Task RetrieveGdbFromFileOrBlobStorage(string containerName, string canonicalName, DisposableTempFile disposableTempGdbZip)
    {
        _logger.LogInformation($"Retrieving GDB File from blob storage: {canonicalName}");
        await _azureStorage.DownloadToAsync(containerName, canonicalName,
            disposableTempGdbZip.FileInfo.FullName);
    }

    public static string GetMapProjection(int coordinateSystemId)
    {
        return $"EPSG:{coordinateSystemId}";
    }

    public static string SanitizeStringForGdb(string str)
    {
        var arr = str.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray();
        return new string(arr).Replace(" ", "_");
    }
}