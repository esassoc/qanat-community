using Microsoft.AspNetCore.Mvc;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Services.GDAL;
using Qanat.Common;
using Qanat.GDALAPI.Services;
using System.Text.RegularExpressions;

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