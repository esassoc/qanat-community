using Microsoft.AspNetCore.Mvc;
using Qanat.GDALAPI.Services;

namespace Qanat.GDALAPI.Controllers;

[ApiController]
public class GdalSrsInfoController : GDALAPIControllerBase<OgrInfoController>
{
    public GdalSrsInfoController(ILogger<OgrInfoController> logger, GDALRunnerService gdalRunnerService,
        IAzureStorage azureStorage) : base(logger, gdalRunnerService, azureStorage)
    {
    }

    [HttpGet("gdalsrsinfo/epsg/{coordinateSystemID}")]
    public async Task<ActionResult<string>> GetWktForCoordinateSystem([FromRoute] int coordinateSystemID)
    {
        var args = BuildCommandLineArgsForGdalSrsInfo(coordinateSystemID);
        try
        {
            var processUtilityResult = _gdalRunnerService.GDALSrsInfo(args);

            return processUtilityResult.StdOut;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public static List<string> BuildCommandLineArgsForGdalSrsInfo(int coordinateSystemId)
    {
        var commandLineArguments = new List<string>
        {
            "-o",
            "wkt1",
            $"epsg:{coordinateSystemId}",
        };

        return commandLineArguments;
    }
}