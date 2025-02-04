using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurementSelfReports
{
    #region Create and Validation

    public static async Task<List<ErrorMessage>> ValidateCreateAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, WaterMeasurementSelfReportCreateDto waterMeasurementSelfReportCreateDto)
    {
        var results = new List<ErrorMessage>();

        var existingSelfReport = await dbContext.WaterMeasurementSelfReports.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.ReportingYear == waterMeasurementSelfReportCreateDto.ReportingYear && x.WaterMeasurementTypeID == waterMeasurementSelfReportCreateDto.WaterMeasurementTypeID);

        if (existingSelfReport != null)
        {
            results.Add(new ErrorMessage() { Type = "Duplicate Self Report", Message = "A self report already exists for this Water Account, Reporting Period and Water Measurement Type." });
        }

        var waterMeasurementType = await dbContext.WaterMeasurementTypes.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementSelfReportCreateDto.WaterMeasurementTypeID);

        if (waterMeasurementType == null)
        {
            results.Add(new ErrorMessage() { Type = "Invalid Water Measurement Type", Message = $"The Water Measurement Type with ID of {waterMeasurementSelfReportCreateDto.WaterMeasurementTypeID} does not exist." });
            return results; //MK 12/10/2024 -- Can't check the rest of the conditions if we don't find a water measurement type so bail early.
        }

        if (!waterMeasurementType.IsActive)
        {
            results.Add(new ErrorMessage() { Type = "Inactive Water Measurement Type", Message = "The Water Measurement Type is not active." });
        }

        if (!waterMeasurementType.IsSelfReportable)
        {
            results.Add(new ErrorMessage() { Type = "Not Self Reportable", Message = "The Water Measurement Type is not self reportable." });
        }

        return results;
    }

    public static async Task<WaterMeasurementSelfReportSimpleDto> CreateAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, WaterMeasurementSelfReportCreateDto waterMeasurementSelfReportCreateDto, UserDto callingUser)
    {
        var newSelfReport = new WaterMeasurementSelfReport()
        {
            GeographyID = geographyID,
            WaterAccountID = waterAccountID,
            ReportingYear = waterMeasurementSelfReportCreateDto.ReportingYear,
            WaterMeasurementTypeID = waterMeasurementSelfReportCreateDto.WaterMeasurementTypeID,

            WaterMeasurementSelfReportStatusID = WaterMeasurementSelfReportStatus.Draft.WaterMeasurementSelfReportStatusID,

            CreateDate = DateTime.UtcNow,
            CreateUserID = callingUser.UserID,
        };

        dbContext.WaterMeasurementSelfReports.Add(newSelfReport);

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(newSelfReport).ReloadAsync();

        var selfReportAsSimpleDto = await GetSingleAsSimpleDtoAsync(dbContext, geographyID, waterAccountID, newSelfReport.WaterMeasurementSelfReportID);
        return selfReportAsSimpleDto;
    }

    #endregion

    #region Read

    public static async Task<List<WaterMeasurementSelfReportSimpleDto>> ListAsSimpleDtoForGeographyAsync(QanatDbContext dbContext, int geographyID)
    {
        var selfReportsAsSimpleDtos = await dbContext.WaterMeasurementSelfReports.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Include(x => x.WaterMeasurementType)
            .Include(x => x.WaterMeasurementSelfReportLineItems)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsSimpleDtoWithExtras())
            .ToListAsync();

        return selfReportsAsSimpleDtos;
    }

    public static async Task<List<WaterMeasurementSelfReportSimpleDto>> ListAsSimpleDtoForWaterAccountAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int reportingYear)
    {
        var selfReportsAsSimpleDtos = await dbContext.WaterMeasurementSelfReports.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Include(x => x.WaterMeasurementType)
            .Include(x => x.WaterMeasurementSelfReportLineItems)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.ReportingYear == reportingYear)
            .Select(x => x.AsSimpleDtoWithExtras())
            .ToListAsync();

        return selfReportsAsSimpleDtos;
    }

    public static async Task<WaterMeasurementSelfReportSimpleDto> GetSingleAsSimpleDtoAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID)
    {
        var selfReportAsSimpleDto = await dbContext.WaterMeasurementSelfReports.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Include(x => x.WaterMeasurementType)
            .Include(x => x.WaterMeasurementSelfReportLineItems)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID)
            .Select(x => x.AsSimpleDtoWithExtras())
            .SingleOrDefaultAsync();

        return selfReportAsSimpleDto;
    }

    public static async Task<WaterMeasurementSelfReportDto> GetSingleAsDtoAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID)
    {
        var selfReportAsDto = await dbContext.WaterMeasurementSelfReports.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.WaterAccount)
            .Include(x => x.WaterMeasurementType)
            .Include(x => x.WaterMeasurementSelfReportLineItems).ThenInclude(x => x.Parcel)
            .Include(x => x.WaterMeasurementSelfReportLineItems).ThenInclude(x => x.IrrigationMethod)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID)
            .Select(x => x.AsDto())
            .SingleOrDefaultAsync();

        return selfReportAsDto;
    }

    #endregion

    #region Update and Validation

    public static async Task<List<ErrorMessage>> ValidateUpdateAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID, WaterMeasurementSelfReportUpdateDto waterMeasurementSelfReportUpdateDto, UserDto callingUser)
    {
        var results = new List<ErrorMessage>();

        var existingSelfReport = await dbContext.WaterMeasurementSelfReports
            .Include(x => x.WaterMeasurementSelfReportLineItems)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID);

        if (existingSelfReport.WaterMeasurementSelfReportStatusID != WaterMeasurementSelfReportStatus.Draft.WaterMeasurementSelfReportStatusID && existingSelfReport.WaterMeasurementSelfReportStatusID != WaterMeasurementSelfReportStatus.Returned.WaterMeasurementSelfReportStatusID)
        {
            callingUser.Flags.TryGetValue(Flag.IsSystemAdmin.FlagName, out var isSystemAdmin);

            var isWaterManager = false;
            callingUser.GeographyFlags.TryGetValue(geographyID, out var geographyFlags);
            if (geographyFlags != null)
            {
                geographyFlags.TryGetValue(Flag.HasManagerDashboard.FlagName, out isWaterManager);
            }

            var canEditDraftSelfReports = isSystemAdmin || isWaterManager;
            if (!canEditDraftSelfReports)
            {
                results.Add(new ErrorMessage { Type = "Not In Draft or Returned", Message = "Only Draft or Returned Self Reports can be updated unless you are a System Admin or Geography Manager." });
            }
        }

        if (existingSelfReport.WaterMeasurementSelfReportStatusID == WaterMeasurementSelfReportStatus.Approved.WaterMeasurementSelfReportStatusID)
        {
            results.Add(new ErrorMessage { Type = "Approved", Message = "Approved Self Reports cannot be updated." });
        }

        foreach (var incomingLineItem in waterMeasurementSelfReportUpdateDto.LineItems)
        {
            var parcel = dbContext.Parcels.AsNoTracking().FirstOrDefault(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.ParcelID == incomingLineItem.ParcelID);
            if (parcel == null)
            {
                results.Add(new ErrorMessage() { Type = "Invalid Parcel", Message = $"A Parcel with ID of {incomingLineItem.ParcelID} does not exist." });
            }

            var irrigationMethod = dbContext.IrrigationMethods.AsNoTracking().FirstOrDefault(x => x.GeographyID == geographyID && x.IrrigationMethodID == incomingLineItem.IrrigationMethodID);
            if (irrigationMethod == null)
            {
                var errorMessage = new ErrorMessage() { Type = "Invalid Irrigation Method", Message = $"An Irrigation Method with ID of {incomingLineItem.IrrigationMethodID} does not exist." };
                results.Add(errorMessage);
            }
        }

        return results;
    }

    public static async Task<WaterMeasurementSelfReportDto> UpdateAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID, WaterMeasurementSelfReportUpdateDto waterMeasurementSelfReportUpdateDto, UserDto callingUser)
    {
        var existingSelfReport = await dbContext.WaterMeasurementSelfReports
            .Include(x => x.WaterMeasurementSelfReportLineItems)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID);

        var existingLineItems = existingSelfReport.WaterMeasurementSelfReportLineItems;
        var incomingLineItems = waterMeasurementSelfReportUpdateDto.LineItems;

        var lineItemsWithoutOverrideValues = incomingLineItems.Where(x => !x.HasAnyOverrideValue);
        var lineItemsToRemove = existingLineItems.Where(existingLineItem => lineItemsWithoutOverrideValues.Any(incomingLineItem => incomingLineItem.ParcelID == existingLineItem.ParcelID) || incomingLineItems.FirstOrDefault(incomingLineItem => incomingLineItem.ParcelID == existingLineItem.ParcelID) == null).ToList();
        dbContext.WaterMeasurementSelfReportLineItems.RemoveRange(lineItemsToRemove);

        var lineItemsToUpdate = existingLineItems.Where(existingLineItem => incomingLineItems.Any(incomingLineItem => incomingLineItem.ParcelID == existingLineItem.ParcelID)).ToList();
        foreach (var lineItemToUpdate in lineItemsToUpdate)
        {
            var incomingLineItem = incomingLineItems.Single(x => x.ParcelID == lineItemToUpdate.ParcelID);
            lineItemToUpdate.IrrigationMethodID = incomingLineItem.IrrigationMethodID;
            lineItemToUpdate.JanuaryOverrideValueInAcreFeet = incomingLineItem.JanuaryOverrideValueInAcreFeet;
            lineItemToUpdate.FebruaryOverrideValueInAcreFeet = incomingLineItem.FebruaryOverrideValueInAcreFeet;
            lineItemToUpdate.MarchOverrideValueInAcreFeet = incomingLineItem.MarchOverrideValueInAcreFeet;
            lineItemToUpdate.AprilOverrideValueInAcreFeet = incomingLineItem.AprilOverrideValueInAcreFeet;
            lineItemToUpdate.MayOverrideValueInAcreFeet = incomingLineItem.MayOverrideValueInAcreFeet;
            lineItemToUpdate.JuneOverrideValueInAcreFeet = incomingLineItem.JuneOverrideValueInAcreFeet;
            lineItemToUpdate.JulyOverrideValueInAcreFeet = incomingLineItem.JulyOverrideValueInAcreFeet;
            lineItemToUpdate.AugustOverrideValueInAcreFeet = incomingLineItem.AugustOverrideValueInAcreFeet;
            lineItemToUpdate.SeptemberOverrideValueInAcreFeet = incomingLineItem.SeptemberOverrideValueInAcreFeet;
            lineItemToUpdate.OctoberOverrideValueInAcreFeet = incomingLineItem.OctoberOverrideValueInAcreFeet;
            lineItemToUpdate.NovemberOverrideValueInAcreFeet = incomingLineItem.NovemberOverrideValueInAcreFeet;
            lineItemToUpdate.DecemberOverrideValueInAcreFeet = incomingLineItem.DecemberOverrideValueInAcreFeet;

            lineItemToUpdate.UpdateUserID = callingUser.UserID;
            lineItemToUpdate.UpdateDate = DateTime.UtcNow;

            dbContext.WaterMeasurementSelfReportLineItems.Update(lineItemToUpdate);
        }

        var lineItemsToAdd = incomingLineItems.Where(incomingLineItem => existingLineItems.All(existingLineItem => existingLineItem.ParcelID != incomingLineItem.ParcelID)).ToList();
        foreach (var lineItemToAdd in lineItemsToAdd)
        {
            var newLineItem = new WaterMeasurementSelfReportLineItem()
            {
                WaterMeasurementSelfReportID = existingSelfReport.WaterMeasurementSelfReportID,
                ParcelID = lineItemToAdd.ParcelID,
                IrrigationMethodID = lineItemToAdd.IrrigationMethodID,
                JanuaryOverrideValueInAcreFeet = lineItemToAdd.JanuaryOverrideValueInAcreFeet,
                FebruaryOverrideValueInAcreFeet = lineItemToAdd.FebruaryOverrideValueInAcreFeet,
                MarchOverrideValueInAcreFeet = lineItemToAdd.MarchOverrideValueInAcreFeet,
                AprilOverrideValueInAcreFeet = lineItemToAdd.AprilOverrideValueInAcreFeet,
                MayOverrideValueInAcreFeet = lineItemToAdd.MayOverrideValueInAcreFeet,
                JuneOverrideValueInAcreFeet = lineItemToAdd.JuneOverrideValueInAcreFeet,
                JulyOverrideValueInAcreFeet = lineItemToAdd.JulyOverrideValueInAcreFeet,
                AugustOverrideValueInAcreFeet = lineItemToAdd.AugustOverrideValueInAcreFeet,
                SeptemberOverrideValueInAcreFeet = lineItemToAdd.SeptemberOverrideValueInAcreFeet,
                OctoberOverrideValueInAcreFeet = lineItemToAdd.OctoberOverrideValueInAcreFeet,
                NovemberOverrideValueInAcreFeet = lineItemToAdd.NovemberOverrideValueInAcreFeet,
                DecemberOverrideValueInAcreFeet = lineItemToAdd.DecemberOverrideValueInAcreFeet,
                CreateUserID = callingUser.UserID,
                CreateDate = DateTime.UtcNow
            };

            dbContext.WaterMeasurementSelfReportLineItems.Add(newLineItem);
        }

        existingSelfReport.UpdateUserID = callingUser.UserID;
        existingSelfReport.UpdateDate = DateTime.UtcNow;

        dbContext.WaterMeasurementSelfReports.Update(existingSelfReport);

        await dbContext.SaveChangesAsync();

        var updatedDto = await GetSingleAsDtoAsync(dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID);
        return updatedDto;
    }

    #endregion

    #region Workflow

    public static async Task<List<ErrorMessage>> ValidateSubmitAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID)
    {
        var results = new List<ErrorMessage>();

        var selfReport = await dbContext.WaterMeasurementSelfReports.AsNoTracking()
            .Include(x => x.WaterMeasurementSelfReportLineItems).ThenInclude(waterMeasurementSelfReportLineItem => waterMeasurementSelfReportLineItem.Parcel)
            .SingleAsync(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID);

        if (selfReport.WaterMeasurementSelfReportStatusID != WaterMeasurementSelfReportStatus.Draft.WaterMeasurementSelfReportStatusID && selfReport.WaterMeasurementSelfReportStatusID != WaterMeasurementSelfReportStatus.Returned.WaterMeasurementSelfReportStatusID)
        {
            results.Add(new ErrorMessage() { Type = "Not In Draft or Returned", Message = "Only Draft or Returned Self Reports can be submitted." });
        }

        var hasAnyValue = selfReport.WaterMeasurementSelfReportLineItems.Any(x => x.HasAnyOverrideValue);
        if (!hasAnyValue)
        {
            results.Add(new ErrorMessage() { Type = "No Values", Message = "At least one Parcel must have a value to submit." });
        }

        foreach (var selfReportWaterMeasurementSelfReportLineItem in selfReport.WaterMeasurementSelfReportLineItems)
        {
            if (!selfReportWaterMeasurementSelfReportLineItem.HasAnyOverrideValue)
            {
                results.Add(new ErrorMessage() { Type = "No Values", Message = $"Parcel {selfReportWaterMeasurementSelfReportLineItem.Parcel.ParcelNumber} must have at least one value to submit." });
            }
        }

        return results;
    }

    public static async Task<WaterMeasurementSelfReportDto> SubmitAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID, UserDto callingUser)
    {
        var selfReport = await dbContext.WaterMeasurementSelfReports
            .Include(x => x.WaterMeasurementSelfReportLineItems)
            .SingleAsync(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID);

        selfReport.WaterMeasurementSelfReportStatusID = WaterMeasurementSelfReportStatus.Submitted.WaterMeasurementSelfReportStatusID;
        selfReport.SubmittedDate = DateTime.UtcNow;
        selfReport.UpdateUserID = callingUser.UserID;
        selfReport.UpdateDate = DateTime.UtcNow;

        dbContext.WaterMeasurementSelfReports.Update(selfReport);

        await dbContext.SaveChangesAsync();

        var updatedDto = await GetSingleAsDtoAsync(dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID);
        return updatedDto;
    }

    public static async Task<List<ErrorMessage>> ValidateApproveAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID)
    {
        var results = new List<ErrorMessage>();

        var selfReport = await dbContext.WaterMeasurementSelfReports.AsNoTracking()
            .Include(x => x.WaterMeasurementSelfReportLineItems)
            .SingleAsync(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID);

        if (selfReport.WaterMeasurementSelfReportStatusID != WaterMeasurementSelfReportStatus.Submitted.WaterMeasurementSelfReportStatusID)
        {
            results.Add(new ErrorMessage() { Type = "Not Submitted", Message = "Only Submitted Self Reports can be approved." });
        }

        return results;
    }

    public static async Task<ApproveSelfReportResult> ApproveAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID, UserDto callingUser)
    {
        var selfReport = await dbContext.WaterMeasurementSelfReports
            .Include(x => x.WaterMeasurementType)
            .Include(x => x.WaterMeasurementSelfReportLineItems).ThenInclude(x => x.IrrigationMethod)
            .SingleAsync(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID);

        selfReport.WaterMeasurementSelfReportStatusID = WaterMeasurementSelfReportStatus.Approved.WaterMeasurementSelfReportStatusID;
        selfReport.ApprovedDate = DateTime.UtcNow;
        selfReport.UpdateUserID = callingUser.UserID;
        selfReport.UpdateDate = DateTime.UtcNow;

        dbContext.WaterMeasurementSelfReports.Update(selfReport);

        await dbContext.SaveChangesAsync();

        var calculationsToRun = await WriteWaterMeasurementsForApprovedSelfReport(dbContext, geographyID, selfReport, callingUser);
        return calculationsToRun;
    }

    private static async Task<ApproveSelfReportResult> WriteWaterMeasurementsForApprovedSelfReport(QanatDbContext dbContext, int geographyID, WaterMeasurementSelfReport selfReport, UserDto callingUser)
    {
        var monthLookup = new Dictionary<int, string>()
        {
            { 1, "JanuaryOverrideValueInAcreFeet" },
            { 2, "FebruaryOverrideValueInAcreFeet" },
            { 3, "MarchOverrideValueInAcreFeet" },
            { 4, "AprilOverrideValueInAcreFeet" },
            { 5, "MayOverrideValueInAcreFeet" },
            { 6, "JuneOverrideValueInAcreFeet" },
            { 7, "JulyOverrideValueInAcreFeet" },
            { 8, "AugustOverrideValueInAcreFeet" },
            { 9, "SeptemberOverrideValueInAcreFeet" },
            { 10, "OctoberOverrideValueInAcreFeet" },
            { 11, "NovemberOverrideValueInAcreFeet" },
            { 12, "DecemberOverrideValueInAcreFeet" }
        };

        WaterMeasurementType consumedWaterMeasurementType = null;
        var parsedConsumedWaterMeasurementMetadata = selfReport.WaterMeasurementType.CalculationJSON.TryParseJObject(out var calculationJSON);
        if (parsedConsumedWaterMeasurementMetadata)
        {
            var consumedWaterMeasurementTypeIDAsString = calculationJSON["ConsumedWaterMeasurementTypeID"]?.ToString();
            var consumedWaterMeasurementTypeIDIsInt = int.TryParse(consumedWaterMeasurementTypeIDAsString, out var consumedWaterMeasurementTypeID);
            consumedWaterMeasurementType = consumedWaterMeasurementTypeIDIsInt
                ? await dbContext.WaterMeasurementTypes.AsNoTracking()
                    .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes)
                    .SingleAsync(x => x.WaterMeasurementTypeID == consumedWaterMeasurementTypeID)
                : null;
        }

        var waterMeasurements = new List<WaterMeasurement>();
        var lineItemsToConsider = selfReport.WaterMeasurementSelfReportLineItems.Where(x => x.HasAnyOverrideValue);
        foreach (var lineItem in lineItemsToConsider)
        {
            foreach (var month in monthLookup)
            {
                //MK 1/17/2025 -- The denormalized model was working decently well and wasn't too annoying to deal with until now. Reflection to the rescue. 
                var reportedValueInAcreFeet = (decimal?)lineItem.GetType().GetProperty(month.Value)?.GetValue(lineItem);

                if (reportedValueInAcreFeet.HasValue)
                {
                    var deliveredWaterMeasurements = await WriteWaterMeasurementForLineItem(dbContext, geographyID, selfReport.ReportingYear, selfReport.WaterMeasurementTypeID, lineItem, month.Key, reportedValueInAcreFeet, callingUser, false);
                    waterMeasurements.AddRange(deliveredWaterMeasurements);

                    if (consumedWaterMeasurementType != null)
                    {
                        var reportedValueInAcreFeetAfterIrrigationEfficiencyApplied = reportedValueInAcreFeet.Value * (lineItem.IrrigationMethod.EfficiencyAsPercentage / 100.0000m);
                        var consumedWaterMeasurements = await WriteWaterMeasurementForLineItem(dbContext, geographyID, selfReport.ReportingYear, consumedWaterMeasurementType.WaterMeasurementTypeID, lineItem, month.Key, reportedValueInAcreFeetAfterIrrigationEfficiencyApplied, callingUser, true);
                        waterMeasurements.AddRange(consumedWaterMeasurements);
                    }
                }
            }
        }

        await dbContext.WaterMeasurements.AddRangeAsync(waterMeasurements);

        await dbContext.SaveChangesAsync();

        var datesReported = waterMeasurements.Select(x => x.ReportedDate).Distinct().ToList();
        var waterMeasurementTypeIDs = waterMeasurements.DistinctBy(x => x.WaterMeasurementTypeID).Select(x => x.WaterMeasurementTypeID);

        var waterMeasurementTypes = dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes)
            .Where(x => waterMeasurementTypeIDs.Contains(x.WaterMeasurementTypeID) && x.WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes.Any())
            .ToList();

        var result = new ApproveSelfReportResult()
        {
            ReportedDates = datesReported,
            WaterMeasurementTypeIDs = waterMeasurementTypes.Select(x => x.WaterMeasurementTypeID).ToList()
        };

        return result;
    }

    private static async Task<List<WaterMeasurement>> WriteWaterMeasurementForLineItem(QanatDbContext dbContext, int geographyID, int reportingYear, int waterMeasurementTypeID, WaterMeasurementSelfReportLineItem lineItem, int month, decimal? reportedValueInAcreFeet, UserDto callingUser, bool includeIrrigationMethodInNote)
    {
        var waterMeasurements = new List<WaterMeasurement>();

        var date = new DateTime(reportingYear, month, 1);
        var day = DateTime.DaysInMonth(date.Year, date.Month);
        var reportedDate = new DateTime(date.Year, date.Month, day);

        var usageEntities = await UsageEntities.GetListByParcelID(dbContext, lineItem.ParcelID);
        var totalArea = Math.Round(usageEntities.Sum(x => (decimal)x.Area), 4, MidpointRounding.ToEven);

        foreach (var usageEntity in usageEntities)
        {
            var usageEntityArea = Math.Round((decimal)usageEntity.Area, 4, MidpointRounding.ToEven);

            var proportionOfTotalArea = usageEntityArea / totalArea;
            var reportedValueInAcreFeetProportional = reportedValueInAcreFeet * proportionOfTotalArea;
            var reportedValueDepthProportional = UnitConversionHelper.ConvertAcreFeetToMillimeters(reportedValueInAcreFeetProportional.GetValueOrDefault(), usageEntityArea);
            var waterMeasurementProportional = new WaterMeasurement()
            {
                GeographyID = geographyID,
                WaterMeasurementTypeID = waterMeasurementTypeID,
                UsageEntityName = usageEntity.UsageEntityName,
                UsageEntityArea = usageEntityArea,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = reportedValueInAcreFeetProportional,
                ReportedValue = reportedValueDepthProportional,
                LastUpdateDate = DateTime.UtcNow,
                UnitTypeID = UnitType.Millimeters.UnitTypeID, //MK 1/17/2025 -- Not sure how to avoid hardcoding this for now. Maybe a field on the self report itself? Maybe on the geography? Defaulting to MM for now as ETSGSA is the only self reporting atm. 
                FromManualUpload = true,
                Comment = includeIrrigationMethodInNote
                    ? $"Self Report approved for {reportingYear} by {callingUser.FullName}. Irrigation Method: {lineItem.IrrigationMethod.Name}, Efficiency: {lineItem.IrrigationMethod.EfficiencyAsPercentage}."
                    : $"Self Report approved for {reportingYear} by {callingUser.FullName}."
            };

            waterMeasurements.Add(waterMeasurementProportional);

            //var reportedValueInAcreFeetEqualPortion = reportedValueInAcreFeet / usageEntities.Count;
            //var reportedValueDepthEqualPortion = UnitConversionHelper.ConvertAcreFeetToMillimeters(reportedValueInAcreFeetEqualPortion.GetValueOrDefault(), usageEntityArea);
            //var waterMeasurementEqualPortion = new WaterMeasurement()
            //{
            //    GeographyID = geographyID,
            //    WaterMeasurementTypeID = waterMeasurementTypeID,
            //    UsageEntityName = usageEntity.UsageEntityName,
            //    UsageEntityArea = usageEntityArea,
            //    ReportedDate = reportedDate,
            //    ReportedValueInAcreFeet = reportedValueInAcreFeetEqualPortion,
            //    ReportedValue = reportedValueDepthEqualPortion,
            //    LastUpdateDate = DateTime.UtcNow,
            //    UnitTypeID = UnitType.Millimeters.UnitTypeID, //MK 1/17/2025 -- Not sure how to avoid hardcoding this for now. Maybe a field on the self report itself? Maybe on the geography? Defaulting to MM for now as ETSGSA is the only self reporting atm. 
            //    FromManualUpload = true,
            //    Comment = $"Self Report approved for {reportingYear} by {callingUser.FullName}."
            //};

            //waterMeasurements.Add(waterMeasurementEqualPortion);
        }

        return waterMeasurements;
    }

    public static async Task<List<ErrorMessage>> ValidateReturnAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID)
    {
        var results = new List<ErrorMessage>();

        var selfReport = await dbContext.WaterMeasurementSelfReports.AsNoTracking()
            .Include(x => x.WaterMeasurementSelfReportLineItems)
            .SingleAsync(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID);

        if (selfReport.WaterMeasurementSelfReportStatusID != WaterMeasurementSelfReportStatus.Submitted.WaterMeasurementSelfReportStatusID)
        {
            results.Add(new ErrorMessage() { Type = "Not Submitted", Message = "Only Submitted Self Reports can be returned." });
        }

        return results;
    }

    public static async Task<WaterMeasurementSelfReportDto> ReturnAsync(QanatDbContext dbContext, int geographyID, int waterAccountID, int waterMeasurementSelfReportID, UserDto callingUser)
    {
        var selfReport = await dbContext.WaterMeasurementSelfReports
            .Include(x => x.WaterMeasurementSelfReportLineItems)
            .SingleAsync(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.WaterMeasurementSelfReportID == waterMeasurementSelfReportID);

        selfReport.WaterMeasurementSelfReportStatusID = WaterMeasurementSelfReportStatus.Returned.WaterMeasurementSelfReportStatusID;
        selfReport.ReturnedDate = DateTime.UtcNow;
        selfReport.UpdateUserID = callingUser.UserID;
        selfReport.UpdateDate = DateTime.UtcNow;

        dbContext.WaterMeasurementSelfReports.Update(selfReport);

        await dbContext.SaveChangesAsync();

        var updatedDto = await GetSingleAsDtoAsync(dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID);
        return updatedDto;
    }

    #endregion
}

public class ApproveSelfReportResult
{
    public List<DateTime> ReportedDates { get; set; }
    public List<int> WaterMeasurementTypeIDs { get; set; }
}