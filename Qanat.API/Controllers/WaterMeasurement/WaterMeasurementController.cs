using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Helpers.WaterMeasurementsXL;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.API.Services.OpenET;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using Swashbuckle.AspNetCore.Annotations;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/water-measurements/")]
public class WaterMeasurementController : SitkaController<WaterMeasurementController>
{
    private readonly UserDto _callingUser;
    private readonly RasterProcessingService _rasterProcessingService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private FileService _fileService;

    public WaterMeasurementController(QanatDbContext dbContext, ILogger<WaterMeasurementController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser, RasterProcessingService rasterProcessingService, IBackgroundJobClient backgroundJobClient, FileService fileService) : base(dbContext, logger, qanatConfiguration)
    {
        _callingUser = callingUser;
        _rasterProcessingService = rasterProcessingService;
        _backgroundJobClient = backgroundJobClient;
        _fileService = fileService;
    }

    [HttpGet("years/{year}/excel-download")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Create)]
    [SwaggerResponse(statusCode: 200, Type = typeof(FileContentResult))]
    public async Task<IActionResult> ListWaterMeasurements([FromRoute] int geographyID, [FromRoute] int year)
    {
        var geography = Geographies.GetByID(_dbContext, geographyID);
        var stream = await WaterMeasurementsXL.CreateWaterMeasurementWBForGeography(_dbContext, geographyID, year);
        return new FileContentResult(stream, "application/octet-stream") { FileDownloadName = $"waterMeasurementsFor{geography.GeographyName}_{year}.csv" };
    }

    [HttpGet("source-of-record")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<WaterMeasurementTypeSimpleDto> GetSourceOfRecordWaterMeasurementType([FromRoute] int geographyID)
    {
        var sourceOfRecordWaterMeasurementTypeSimpleDto = Geographies.GetSourceOfRecordWaterMeasurementTypeByGeographyID(_dbContext, geographyID)?.AsSimpleDto();
        return Ok(sourceOfRecordWaterMeasurementTypeSimpleDto);
    }

    [HttpPost("calculations/years/{year}/months/{month}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Create)]
    public async Task<IActionResult> RunAllCalculationsForGeography([FromRoute] int geographyID, [FromRoute] int year, [FromRoute] int month)
    {
        var effectiveDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        await WaterMeasurementCalculations.RunAllMeasurementTypesForGeography(_dbContext, geographyID, effectiveDate);
        return Ok();
    }

    #region By Parcel

    [HttpGet("parcels/{parcelID}/chart-data")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
    public ActionResult<List<ParcelWaterMeasurementChartDatumDto>> ListWaterMeasurementChartDataForParcel([FromRoute] int geographyID, [FromRoute] int parcelID)
    {
        var chartDataDtos = WaterMeasurements.ListAsParcelWaterMeasurementChartDatumDto(_dbContext, geographyID, parcelID, _callingUser);

        var geography = Geographies.GetByID(_dbContext, geographyID);
        if (!geography.IsOpenETActive)
        {
            chartDataDtos = chartDataDtos.Where(x => !WaterMeasurementTypes.OpenETWaterMeasurementTypeNames.Contains(x.WaterMeasurementTypeName)).ToList();
        }

        return Ok(chartDataDtos);
    }

    [HttpGet("parcels/{parcelID}/excel-download")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
    [SwaggerResponse(statusCode: 200, Description = "CSV of Parcel Water Usage Estimates", ContentTypes = new string[] { "text/csv" }, Type = typeof(FileContentResult))]
    public async Task<IActionResult> ListWaterMeasurementsForParcel([FromRoute] int geographyID, [FromRoute] int parcelID)
    {
        var usageEntityNames = _dbContext.UsageEntities.AsNoTracking().Where(x => x.ParcelID == parcelID).Select(x => x.UsageEntityName).ToList();
        var parcelNumber = _dbContext.Parcels.Single(x => x.ParcelID == parcelID).ParcelNumber;

        var result = await WaterMeasurementsXL.CreateWaterMeasurementWBForGeography(_dbContext, geographyID, null, usageEntityNames);
        var geography = Geographies.GetByID(_dbContext, geographyID);
        return new FileContentResult(result, "application/octet-stream") { FileDownloadName = $"waterMeasurementsFor{geography.GeographyName}_{parcelNumber}.csv" };
    }

    #endregion

    #region CSV Upload

    [HttpPost("csv-headers")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [RequestSizeLimit(524288000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
    [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Create)]
    public async Task<IActionResult> ListCSVHeaders([FromForm] CsvUpsertDto csvUpsertDto, [FromRoute] int geographyID)
    {
        var extension = Path.GetExtension(csvUpsertDto.UploadedFile.FileName);
        if (extension != ".csv")
        {
            ModelState.AddModelError("FileResource", $"{extension[1..].ToUpper()} is not an accepted file extension");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var fileData = await HttpUtilities.GetIFormFileData(csvUpsertDto.UploadedFile);

        if (!ListHeadersFromCsvUpload(fileData, out var headerNames))
        {
            return BadRequest(ModelState);
        }

        return Ok(headerNames);
    }

    [HttpPost("csv")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [RequestSizeLimit(524288000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
    [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Create)]
    public async Task<IActionResult> NewCSVUpload([FromForm] WaterMeasurementCsvUpsertDto waterMeasurementCsvUpsertDto, [FromRoute] int geographyID)
    {
        var extension = Path.GetExtension(waterMeasurementCsvUpsertDto.UploadedFile.FileName);
        if (extension != ".csv")
        {
            ModelState.AddModelError("FileResource", $"{extension[1..].ToUpper()} is not an accepted file extension");
            return BadRequest(ModelState);
        }

        if (waterMeasurementCsvUpsertDto.APNColumnName == waterMeasurementCsvUpsertDto.QuantityColumnName)
        {
            ModelState.AddModelError("Value Column", "The selected Value column cannot match the selected APN column. Two distinct header names are required.");
            return BadRequest(ModelState);
        }

        var waterMeasurementType = _dbContext.WaterMeasurementTypes.Single(x => x.WaterMeasurementTypeID == waterMeasurementCsvUpsertDto.WaterMeasurementTypeID);
        if (!waterMeasurementType.IsUserEditable)
        {
            ModelState.AddModelError("Water Measurement Type", "The selected Water Measurement Type is not editable.");
            return BadRequest(ModelState);
        }

        var fileData = await HttpUtilities.GetIFormFileData(waterMeasurementCsvUpsertDto.UploadedFile);

        if (!ParseCsvUpload(fileData, waterMeasurementCsvUpsertDto.APNColumnName, waterMeasurementCsvUpsertDto.QuantityColumnName, waterMeasurementCsvUpsertDto.CommentColumnName, out var records))
        {
            return BadRequest(ModelState);
        }

        if (!ValidateCsvUploadData(records, geographyID))
        {
            return BadRequest(ModelState);
        }

        var effectiveDate = DateTime.Parse(waterMeasurementCsvUpsertDto.EffectiveDate);
        var waterMeasurementCsvResponseDto = await WaterMeasurements.CreateFromCSV(_dbContext, records, effectiveDate, waterMeasurementCsvUpsertDto.WaterMeasurementTypeID.Value, waterMeasurementCsvUpsertDto.UnitTypeID.Value, geographyID);

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, waterMeasurementCsvUpsertDto.WaterMeasurementTypeID.Value, effectiveDate);

        return Ok(waterMeasurementCsvResponseDto);
    }

    private bool ListHeadersFromCsvUpload(byte[] fileData, out List<string> headerNames)
    {
        try
        {
            using var memoryStream = new MemoryStream(fileData);
            using var reader = new StreamReader(memoryStream);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            csvReader.Context.RegisterClassMap(new ParcelTransactionCSVMap("APN", "Usage", "Comment"));
            csvReader.Read();
            csvReader.ReadHeader();

            headerNames = null;

            if (csvReader.HeaderRecord.Count(x => !string.IsNullOrWhiteSpace(x)) == 1)
            {
                ModelState.AddModelError("UploadedFile", "The uploaded CSV only contains one header name. At least two named columns are required.");
                return false;
            }

            var headerNamesDuplicated = csvReader.HeaderRecord.Where(x => !string.IsNullOrWhiteSpace(x)).GroupBy(x => x).Where(x => x.Count() > 1).ToList();
            if (headerNamesDuplicated.Any())
            {
                ModelState.AddModelError("UploadedFile",
                    $"The following header {(headerNamesDuplicated.Count > 1 ? "names appear" : "name appears")} more than once: {string.Join(", ", headerNamesDuplicated.OrderBy(x => x.Key).Select(x => x.Key))}");
                return false;
            }

            headerNames = csvReader.HeaderRecord.ToList();
            return true;
        }
        catch
        {
            ModelState.AddModelError("UploadedFile",
                "There was an error parsing the CSV. Please ensure the file is formatted correctly.");
            headerNames = null;
            return false;
        }
    }

    private bool ParseCsvUpload(byte[] fileData, string apnColumnName, string quantityColumnName, string commentColumnName, out List<ParcelTransactionCSV> records)
    {
        try
        {
            using var memoryStream = new MemoryStream(fileData);
            using var reader = new StreamReader(memoryStream);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            csvReader.Context.RegisterClassMap(new ParcelTransactionCSVMap(apnColumnName, quantityColumnName, commentColumnName));
            records = csvReader.GetRecords<ParcelTransactionCSV>().ToList();
            return true;
        }
        catch (HeaderValidationException e)
        {
            var headerMessage = e.Message.Split('.')[0];
            ModelState.AddModelError("UploadedFile",
                $"{headerMessage}. Please check that the column name is not missing or misspelled.");
            records = null;
            return false;
        }
        catch (CsvHelper.MissingFieldException e)
        {
            var headerMessage = e.Message.Split('.')[0];
            ModelState.AddModelError("UploadedFile",
                $"{headerMessage}. Please check that the column name is not missing or misspelled.");
            records = null;
            return false;
        }
        catch
        {
            ModelState.AddModelError("UploadedFile",
                "There was an error parsing the CSV. Please ensure the file is formatted correctly.");
            records = null;
            return false;
        }
    }

    private bool ValidateCsvUploadData(List<ParcelTransactionCSV> records, int geographyID)
    {
        var isValid = true;

        // no null APNs
        var nullUsageEntityNamesCount = records.Count(x => x.UsageEntityName == "");
        if (nullUsageEntityNamesCount > 0)
        {
            ModelState.AddModelError("UploadedFile",
                $"The uploaded file contains {nullUsageEntityNamesCount} {(nullUsageEntityNamesCount > 1 ? "rows" : "row")} specifying a value with no corresponding APN or Usage Entity Name.");
            isValid = false;
        }

        // no null quantities
        var nullQuantities = records.Where(x => x.Quantity == null).ToList();
        if (nullQuantities.Any())
        {
            ModelState.AddModelError("UploadedFile",
                $"The following {(nullQuantities.Count > 1 ? "APN/Usage Entity Names" : "APN/Usage Entity Name")} had no usage quantity entered: {string.Join(", ", nullQuantities.Select(x => x.UsageEntityName))}");
            isValid = false;
        }

        // no duplication APN/UsageEntityNames
        var duplicateUsageEntityNames = records.GroupBy(x => x.UsageEntityName).Where(x => x.Count() > 1).ToList();
        if (duplicateUsageEntityNames.Any())
        {
            ModelState.AddModelError("UploadedFile",
                               $"The following {(duplicateUsageEntityNames.Count > 1 ? "APN/Usage Entity Names" : "APN/Usage Entity Name")} appear more than once: {string.Join(", ", duplicateUsageEntityNames.Select(x => x.Key))}");
            isValid = false;
        }
        return isValid;
    }

    #endregion

    #region Raster Upload

    [HttpPost("raster-upload")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [RequestSizeLimit(524288000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
    [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Create)]
    public async Task<IActionResult> UploadRaster([FromRoute] int geographyID, [FromForm] WaterMeasurementRasterUploadDto rasterFileUploadDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var allowedExtensions = new[] { ".tif", ".tiff" };
        var extension = Path.GetExtension(rasterFileUploadDto.UploadedFile.FileName);
        if (!allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError("UploadedFile", "The uploaded file is not a valid raster file. Please upload a .tif or .tiff file.");
            return BadRequest(ModelState);
        }

        var geographyAsDto = Geographies.GetByIDAsDto(_dbContext, geographyID);
        var waterMeasurementType = _dbContext.WaterMeasurementTypes.SingleOrDefault(x => x.WaterMeasurementTypeID == rasterFileUploadDto.WaterMeasurementTypeID);
        if (waterMeasurementType == null)
        {
            ModelState.AddModelError("WaterMeasurementTypeID", "The selected Water Measurement Type does not exist.");
            return BadRequest(ModelState);
        }

        var unitType = UnitType.All.SingleOrDefault(x => x.UnitTypeID == rasterFileUploadDto.UnitTypeID);
        if (unitType == null)
        {
            ModelState.AddModelError("UnitTypeID", "The selected Unit Type does not exist.");
            return BadRequest(ModelState);
        }

        var parsedEffectiveDate = DateTime.TryParse(rasterFileUploadDto.EffectiveDate, out var effectiveDate);
        if (!parsedEffectiveDate)
        {
            ModelState.AddModelError("EffectiveDate", "The Effective Date is not a valid date.");
            return BadRequest(ModelState);
        }

        var rasterFileResource = await _fileService.CreateFileResource(_dbContext, rasterFileUploadDto.UploadedFile, _callingUser.UserID);

        var firstOfMonth = new DateTime(effectiveDate.Year, effectiveDate.Month, 1);
        var reportedDate = firstOfMonth.AddMonths(1).AddDays(-1);

        var backgroundJobID = _backgroundJobClient.Enqueue(() => _rasterProcessingService.ProcessRasterByFileCanonicalNameForAllUsageEntities(geographyAsDto, null, waterMeasurementType.WaterMeasurementTypeID, unitType.UnitTypeID, reportedDate, rasterFileResource.FileResourceCanonicalName, true, false));
        var continuedWithJobID = _backgroundJobClient.ContinueJobWith(backgroundJobID, () => _rasterProcessingService.RunCalculations(geographyID, waterMeasurementType.WaterMeasurementTypeID, reportedDate));

        var result = new HangfireBackgroundJobResultDto()
        {
            BackgroundJobID = backgroundJobID,
            ContinuedWithJobID = continuedWithJobID
        };

        return Ok(result);
    }

    #endregion
}