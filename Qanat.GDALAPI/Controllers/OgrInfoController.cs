using Qanat.Common;
using Qanat.GDALAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Qanat.Common.Services.GDAL;
using System.Text.RegularExpressions;
using Qanat.Common.GeoSpatial;

namespace Qanat.GDALAPI.Controllers;

[ApiController]
public class OgrInfoController : GDALAPIControllerBase<OgrInfoController>
{
    public OgrInfoController(ILogger<OgrInfoController> logger, GDALRunnerService gdalRunnerService, IAzureStorage azureStorage) : base(logger, gdalRunnerService, azureStorage)
    {
    }

    [HttpPost("ogrinfo/gdb-srid")]
    public async Task<ActionResult<int>> GdbToSRID([FromBody] OgrInfoRequestDto requestDto)
    {
        using var disposableTempGdbZip = DisposableTempFile.MakeDisposableTempFileEndingIn(".gdb.zip");
        await RetrieveGdbFromFileOrBlobStorage(requestDto.BlobContainer, requestDto.CanonicalName, disposableTempGdbZip);
        var args = BuildOgrInfoCommandLineArgumentsToListFeatureClassInfos(disposableTempGdbZip.FileInfo.FullName);
        try
        {
            var processUtilityResult = _gdalRunnerService.OgrInfo(args);

            var regex = new Regex(@"^    ID\[""EPSG"",(\d*)\]", RegexOptions.Multiline);
            var match = regex.Match(processUtilityResult.StdOut);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int srid))
            {
                return srid;
            }

            return Proj4NetHelper.WEB_MERCATOR;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost("ogrinfo/gdb-srid-wkt")]
    public async Task<ActionResult<string>> GetGDBSRIDWKT([FromBody] OgrInfoRequestDto requestDto)
    {
        using var disposableTempGdbZip = DisposableTempFile.MakeDisposableTempFileEndingIn(".gdb.zip");
        await RetrieveGdbFromFileOrBlobStorage(requestDto.BlobContainer, requestDto.CanonicalName, disposableTempGdbZip);
        var args = BuildOgrInfoCommandLineArgumentsToRetrieveWKTInfos(disposableTempGdbZip.FileInfo.FullName);
        try
        {
            var processUtilityResult = _gdalRunnerService.OgrInfo(args);

            var regex = new Regex(@"(?s)Layer SRS WKT:\s*(.*?)(?=\n\S)", RegexOptions.Multiline);
            var match = regex.Match(processUtilityResult.StdOut);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            throw new Exception($"Could not find well known text within {processUtilityResult.StdOut}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    [HttpPost("ogrinfo/gdb-feature-classes")]
    public async Task<ActionResult<List<FeatureClassInfo>>> GdbToFeatureClassInfo([FromBody] OgrInfoRequestDto requestDto)
    {
        using var disposableTempGdbZip = DisposableTempFile.MakeDisposableTempFileEndingIn(".gdb.zip");
        await RetrieveGdbFromFileOrBlobStorage(requestDto.BlobContainer, requestDto.CanonicalName, disposableTempGdbZip);
        var args = BuildOgrInfoCommandLineArgumentsToListFeatureClassInfos(disposableTempGdbZip.FileInfo.FullName);
        try
        {
            var processUtilityResult = _gdalRunnerService.OgrInfo(args);

            var featureClassesFromFileGdb = processUtilityResult.StdOut.Split(new[] { "\r\nLayer name: " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
            var featureClassInfos = new List<FeatureClassInfo>();
            foreach (var featureClassBlob in featureClassesFromFileGdb)
            {
                var featureClassInfo = new FeatureClassInfo();
                var features = featureClassBlob.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                featureClassInfo.LayerName = features.First().ToLower();
                featureClassInfo.FeatureType = features.First(x => x.StartsWith("Geometry: ")).Substring("Geometry: ".Length);
                featureClassInfo.FeatureCount = int.Parse(features.First(x => x.StartsWith("Feature Count: ")).Substring("Feature Count: ".Length));

                var columnNamesBlob = featureClassBlob.Split(new[] { "FID Column = " }, StringSplitOptions.RemoveEmptyEntries);
                if (columnNamesBlob.Length == 2)
                {
                    featureClassInfo.Columns = columnNamesBlob.Skip(1).Single()
                        .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => !x.StartsWith("Geometry Column")).Select(x =>
                            x.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries).First().ToLower()).ToList();
                }
                else
                {
                    featureClassInfo.Columns = new List<string>();
                }

                featureClassInfos.Add(featureClassInfo);
            }

            return Ok(featureClassInfos);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static List<string> BuildOgrInfoCommandLineArgumentsToListFeatureClassInfos(string inputGdbFile)
    {
        var commandLineArguments = new List<string>
        {
            "-al",
            "-ro",
            "-so",
            "-noextent",
            inputGdbFile
        };

        return commandLineArguments;
    }

    public static List<string> BuildOgrInfoCommandLineArgumentsToRetrieveWKTInfos(string inputGdbFile)
    {
        var commandLineArguments = new List<string>
        {
            "-al",
            "-ro",
            "-so",
            "-noextent",
            "-wkt_format",
            "WKT1",
            inputGdbFile
        };

        return commandLineArguments;
    }
}