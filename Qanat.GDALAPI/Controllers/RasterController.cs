using Microsoft.AspNetCore.Mvc;
using Qanat.Common;
using Qanat.Common.Services.GDAL;
using Qanat.GDALAPI.Services;
using System.Text.RegularExpressions;

namespace Qanat.GDALAPI.Controllers;

[ApiController]
public class RasterController : GDALAPIControllerBase<RasterController>
{
    public RasterController(ILogger<RasterController> logger, GDALRunnerService gdalRunnerService, IAzureStorage azureStorage) : base(logger, gdalRunnerService, azureStorage)
    {
    }

    [HttpPost("gdalwarp/calculate-mean")]
    public async Task<ActionResult<decimal>> UpsertGdb([FromBody] GDALWarpRequestDto requestDto)
    {
        using var disposableTempInputTiffFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".tiff");
        using var disposableTempOutputTiffFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".tiff");
        using var disposableTempJSONFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".json");

        _logger.LogInformation($"Retrieving tiff file from blob storage: {requestDto.InputTiffCanonicalName}");
        await _azureStorage.DownloadToAsync(requestDto.BlobContainer, requestDto.InputTiffCanonicalName,
            disposableTempInputTiffFile.FileInfo.FullName);

        await System.IO.File.WriteAllTextAsync(disposableTempJSONFile.FileInfo.FullName, requestDto.CutLineGeoJSON);

        _gdalRunnerService.GDALWarp(BuildCommandLineArgumentsForGDALWarp(
            disposableTempOutputTiffFile.FileInfo.FullName,
            disposableTempInputTiffFile.FileInfo.FullName,
            disposableTempJSONFile.FileInfo.FullName
        ));

        var result = _gdalRunnerService.GDALInfo(BuildCommandLineArgumentsForGDALInfo(disposableTempOutputTiffFile.FileInfo.FullName));
        var stdOutString = result.StdOut;

        var regex = new Regex(@"^    STATISTICS_MEAN=(\d*)", RegexOptions.Multiline);

        var match = regex.Match(stdOutString);
        if (match.Success && decimal.TryParse(match.Groups[1].Value, out decimal meanValue))
        {
            return Ok(meanValue);
        }

        _logger.LogInformation(result.StdOutAndStdErr);
        return BadRequest("Failed to calculate mean value from raster.");
    }

    private static List<string> BuildCommandLineArgumentsForGDALWarp(string outputTiffFilePath, string inputTiffFilePath, string cutLineGeoJsonFilePath)
    {
        var commandLineArguments = new List<string>
        {
            "-cutline",
            cutLineGeoJsonFilePath,
            "-crop_to_cutline",
            inputTiffFilePath, 
            outputTiffFilePath
        };

        return commandLineArguments.ToList();
    }

    private static List<string> BuildCommandLineArgumentsForGDALInfo(string inputTiffFilePath)
    {
        var commandLineArguments = new List<string>
        {
            inputTiffFilePath, 
        };

        return commandLineArguments.ToList();
    }
}