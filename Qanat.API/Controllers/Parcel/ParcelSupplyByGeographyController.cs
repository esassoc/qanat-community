using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers
{
    [ApiController]
    [RightsChecker]
    [Route("geographies/{geographyID}/parcel-supplies")]
    public class ParcelSupplyByGeographyController(QanatDbContext dbContext, ILogger<ParcelSupplyByGeographyController> logger, IOptions<QanatConfiguration> qanatConfiguration, [FromServices] UserDto callingUser)
        : SitkaController<ParcelSupplyByGeographyController>(dbContext, logger, qanatConfiguration)
    {
        [HttpPost]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Create)]
        public IActionResult New([FromBody] ParcelSupplyUpsertDto parcelSupplyUpsertDto, [FromRoute] int geographyID)
        {
            if (!parcelSupplyUpsertDto.ParcelIDs.Any())
            {
                ModelState.AddModelError("`APN", $"The APN field is required.");
                return BadRequest(ModelState);
            }

            var parcelID = parcelSupplyUpsertDto.ParcelIDs.First();
            var parcel = Parcels.GetByID(_dbContext, parcelID);
            if (parcel == null)
            {
                ModelState.AddModelError("Parcel APN", $"Parcel with ID {parcelID} does not exist!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            ParcelSupplies.Create(_dbContext, parcel, parcelSupplyUpsertDto, userDto.UserID);
            return Ok();
        }

        [HttpPost("bulk")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Create)]
        public IActionResult BulkNew([FromBody] ParcelSupplyUpsertDto parcelSupplyUpsertDto, [FromRoute] int geographyID)
        {
            if (parcelSupplyUpsertDto.WaterTypeID == null)
            {
                ModelState.AddModelError("SupplyType", "The Supply Type field is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var parcels = Parcels.ListByIDs(_dbContext, parcelSupplyUpsertDto.ParcelIDs);
            var createCount = ParcelSupplies.BulkCreate(_dbContext, parcels, parcelSupplyUpsertDto, userDto.UserID, true);
            return Ok(createCount);
        }

        [HttpPost("csv")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [RequestSizeLimit(524288000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Create)]
        public async Task<IActionResult> NewCSVUpload([FromForm] ParcelSupplyCsvUpsertDto parcelSupplyCsvUpsertDto, [FromRoute] int geographyID)
        {
            var extension = Path.GetExtension(parcelSupplyCsvUpsertDto.UploadedFile.FileName);
            if (extension != ".csv")
            {
                ModelState.AddModelError("FileResource", $"{extension[1..].ToUpper()} is not an accepted file extension");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fileData = await HttpUtilities.GetIFormFileData(parcelSupplyCsvUpsertDto.UploadedFile);
            var waterTypeName = WaterTypes.GetNameByID(_dbContext, parcelSupplyCsvUpsertDto.WaterTypeID.Value);

            if (!ParseCsvUpload(fileData, waterTypeName, out var records))
            {
                return BadRequest(ModelState);
            }

            if (!ValidateCsvUploadData(records, waterTypeName, geographyID))
            {
                return BadRequest(ModelState);
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            var effectiveDate = DateTime.Parse(parcelSupplyCsvUpsertDto.EffectiveDate);
            var createCount = ParcelSupplies.CreateFromCSV(_dbContext, records, parcelSupplyCsvUpsertDto.UploadedFile.FileName, effectiveDate, parcelSupplyCsvUpsertDto.WaterTypeID.Value, userDto.UserID, geographyID);
            return Ok(createCount);
        }

        [HttpGet("transaction-history")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
        public ActionResult<List<TransactionHistoryDto>> ListTransactionHistory([FromRoute] int geographyID)
        {
            var transactionHistoryDtos = _dbContext.vParcelSupplyTransactionHistories.AsNoTracking().Where(x => x.GeographyID == geographyID).Select(x => new TransactionHistoryDto()
            {
                TransactionDate = x.TransactionDate,
                EffectiveDate = x.EffectiveDate,
                CreateUserFullName = x.CreateUserFullName,
                WaterTypeName = x.WaterTypeName,
                AffectedParcelsCount = x.AffectedParcelsCount ?? 0,
                AffectedAcresCount = x.AffectedAcresCount ?? 0f,
                UploadedFileName = x.UploadedFileName,
                TransactionVolume = x.TransactionVolume,
                TransactionDepth = x.TransactionDepth
            }).ToList();

            return Ok(transactionHistoryDtos);
        }

        [HttpGet("year/{year}")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
        public ActionResult<List<WaterTypeSupplyDto>> GetSupplyOfWaterTypes([FromRoute] int geographyID, int year)
        {
            var waterTypeSupplyDtos = WaterTypeSupplies.ListByYearAndGeography(_dbContext, year, geographyID);
            return Ok(waterTypeSupplyDtos);
        }

        [HttpGet("recent-effective-dates/year/{year}")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
        public async Task<ActionResult<MostRecentEffectiveDatesDto>> GetMostRecentSupplyAndUsageDate([FromRoute] int geographyID, int year)
        {
            var mostRecentEffectiveDatesDto = await ReportingPeriods.GetMostRecentEffectiveDatesAsync(_dbContext, geographyID, year, callingUser);
            return Ok(mostRecentEffectiveDatesDto);
        }

        [HttpGet("water-accounts/{waterAccountID}/transactions/year/{year}")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithWaterAccountRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
        public async Task<ActionResult<List<ParcelActivityDto>>> GetTransactionsFromAccountID([FromRoute] int geographyID, int waterAccountID, int year)
        {
            var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, year, callingUser);
            if (reportingPeriod == null)
            {
                var geography = await Geographies.GetByIDAsMinimalDtoAsync(_dbContext, geographyID);
                return BadRequest($"No reporting period found for the year {year} for {geography.GeographyDisplayName}.");
            }

            var startDate = reportingPeriod.StartDate;
            var endDate = reportingPeriod.EndDate;
            var parcelIDs = Parcels.ListParcelsFromAccountIDAndEndDate(_dbContext, waterAccountID, endDate)
                .Select(x => x.ParcelID).Distinct().ToList();
            var parcelActivityDto = ParcelSupplies.ListAsParcelActivityDto(_dbContext, parcelIDs)
                .Where(x => x.EffectiveDate >= startDate && x.EffectiveDate <= endDate);

            return Ok(parcelActivityDto);
        }

        [HttpGet("monthly-usage-summary/year/{year}")]
        [EntityNotFound(typeof(Geography), "geographyID")]
        [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
        public ActionResult<GeographySourceOfRecordWaterMeasurementTypeMonthlyUsageSummaryDto> GetMonthlyUsageSummaryForGeographyAndReportingPeriod([FromRoute] int geographyID, [FromRoute] int year)
        {
            var geographySourceOfRecordWaterMeasurementTypeMonthlyUsageSummaryDto = MonthlyUsageSummary.ListByGeographyAndYear(_dbContext, geographyID, year);
            return Ok(geographySourceOfRecordWaterMeasurementTypeMonthlyUsageSummaryDto);
        }

        private bool ParseCsvUpload(byte[] fileData, string waterTypeDisplayName, out List<ParcelTransactionCSV> records)
        {
            try
            {
                using var memoryStream = new MemoryStream(fileData);
                using var reader = new StreamReader(memoryStream);
                using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

                csvReader.Context.RegisterClassMap(new ParcelTransactionCSVMap("APN", waterTypeDisplayName + " Quantity", "Comment"));
                csvReader.Read();
                csvReader.ReadHeader();

                var headerNamesDuplicated = csvReader.HeaderRecord.Where(x => !string.IsNullOrWhiteSpace(x)).GroupBy(x => x).Where(x => x.Count() > 1).ToList();
                if (headerNamesDuplicated.Any())
                {
                    ModelState.AddModelError("UploadedFile",
                        $"The following header {(headerNamesDuplicated.Count > 1 ? "names appear" : "name appears")} more than once: {string.Join(", ", headerNamesDuplicated.OrderBy(x => x.Key).Select(x => x.Key))}");
                    records = null;
                    return false;
                }

                records = csvReader.GetRecords<ParcelTransactionCSV>().ToList();
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

            return true;
        }

        private bool ValidateCsvUploadData(List<ParcelTransactionCSV> records, string waterTypeDisplayName, int geographyID)
        {
            var isValid = true;

            // no null APNs
            var nullAPNsCount = records.Count(x => x.UsageEntityName == "");
            if (nullAPNsCount > 0)
            {
                ModelState.AddModelError("UploadedFile",
                    $"The uploaded file contains {nullAPNsCount} {(nullAPNsCount > 1 ? "rows" : "row")} specifying a value with no corresponding APN.");
                isValid = false;
            }

            // no null quantities
            var nullQuantities = records.Where(x => x.Quantity == null).ToList();
            if (nullQuantities.Any())
            {
                ModelState.AddModelError("UploadedFile",
                    $"The following {(nullQuantities.Count > 1 ? "APN/Usage Entity Names" : "APN/Usage Entity Name")} had no {waterTypeDisplayName} Quantity entered: {string.Join(", ", nullQuantities.Select(x => x.UsageEntityName))}");
                isValid = false;
            }

            // no duplicate APNs
            var duplicateAPNs = records.GroupBy(x => x.UsageEntityName).Where(x => x.Count() > 1)
                .Select(x => x.Key).ToList();

            if (duplicateAPNs.Any())
            {
                ModelState.AddModelError("UploadedFile",
                    $"The uploaded file contains multiples rows with {(duplicateAPNs.Count > 1 ? "these APN/Usage Entity Names" : "this APN/Usage Entity Name")}: {string.Join(", ", duplicateAPNs)}");
                isValid = false;
            }

            // all valid APNs
            var allParcelNumbers = _dbContext.Parcels.AsNoTracking().Where(x => x.GeographyID == geographyID).Select(y => y.ParcelNumber);
            var unmatchedRecords = records.Where(x => !allParcelNumbers.Contains(x.UsageEntityName)).ToList();

            if (unmatchedRecords.Any())
            {
                ModelState.AddModelError("UploadedFile",
                    $"The uploaded file contains {(unmatchedRecords.Count > 1 ? "these APN/Usage Entity Names which do" : "this APN/Usage Entity Name which does")} not match any record in the system for this geography: {string.Join(", ", unmatchedRecords.Select(x => x.UsageEntityName))}");
                isValid = false;
            }

            return isValid;
        }
    }
}