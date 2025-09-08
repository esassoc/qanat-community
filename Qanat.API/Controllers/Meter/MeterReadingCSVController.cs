using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/meter-readings/csv")]
public class MeterReadingCSVController(QanatDbContext dbContext, ILogger<MeterReadingCSVController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<MeterReadingCSVController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet("template")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    [SwaggerResponse(statusCode: 200, Type = typeof(FileContentResult))]
    public async Task<IActionResult> DownloadCSVTemplate(int geographyID)
    {
        var headerList = string.Join(",", MeterReadingUpsertDtoCSVMap.Headers);

        // CreateAsync a memory stream with the header.
        using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream);
        await writer.WriteLineAsync(headerList);
        await writer.FlushAsync();

        memoryStream.Position = 0;

        var bytes = memoryStream.ToArray();

        // CreateAsync a CSV file with the header.
        var fileName = $"MeterReadingTemplate_{geographyID}.csv";
        var contentType = "text/csv";

        return new FileContentResult(bytes, contentType) { FileDownloadName = fileName };
    }

    [HttpPost("upload")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [RequestSizeLimit(524288000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<MeterReadingDto>>> UploadCSV(int geographyID, [FromForm] MeterReadingCSVFileUploadDto fileUploadDto)
    {
        if (fileUploadDto?.CSVFile == null || fileUploadDto.CSVFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        if (fileUploadDto.CSVFile.ContentType != "text/csv")
        {
            return BadRequest("Invalid file type. Please upload a CSV file.");
        }

        // Read the CSV file into a byte array and parse.
        var bytes = await HttpUtilities.GetIFormFileData(fileUploadDto.CSVFile);
        var meterReadingsFromCSVResult = await MeterReadings.ParseMeterReadingsFromCSVAsync(bytes);
        meterReadingsFromCSVResult.Errors.ForEach(e => ModelState.AddModelError(e.Type, e.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Excel is notorious for putting in blank rows. Let's just skip past those. 
        var nonBlankRecords = meterReadingsFromCSVResult.Records.Where(x => !x.IsBlank()).ToList();

        // Validate records.
        var recordErrors = await MeterReadings.ValidateCSVRecordsAsync(_dbContext, geographyID, nonBlankRecords);
        recordErrors.ForEach(e => ModelState.AddModelError(e.Type, e.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // CreateAsync meter readings.
        var meterReadings = await MeterReadings.BulkInsertAsync(_dbContext, geographyID, nonBlankRecords);
        return Ok(meterReadings);
    }
}