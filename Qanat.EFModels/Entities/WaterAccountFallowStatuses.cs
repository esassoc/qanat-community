using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities.ExtensionMethods;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.EFModels.Entities;

public class WaterAccountFallowStatuses
{
    public static async Task<List<ErrorMessage>> ValidateCreateAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int waterAccountID, UserDto callingUser)
    {
        var errors = new List<ErrorMessage>();
        var existingFallowStatus = await dbContext.WaterAccountFallowStatuses.AnyAsync(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterAccountID == waterAccountID);
        if (existingFallowStatus)
        {
            errors.Add(new ErrorMessage("WaterAccountFallowStatus", "A fallow status already exists for this water account in the specified geography and reporting period."));
        }

        return errors;
    }

    public static async Task<WaterAccountFallowStatusDto> CreateAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int waterAccountID, UserDto callingUser)
    {
        var waterAccountFallowStatus = new WaterAccountFallowStatus
        {
            GeographyID = geographyID,
            ReportingPeriodID = reportingPeriodID,
            WaterAccountID = waterAccountID,
            SelfReportStatusID = SelfReportStatus.Draft.SelfReportStatusID,
            CreateUserID = callingUser.UserID,
            CreateDate = DateTime.UtcNow
        };

        dbContext.WaterAccountFallowStatuses.Add(waterAccountFallowStatus);
        await dbContext.SaveChangesAsync();

        var newFallowStatus = await GetByIDAsync(dbContext, geographyID, reportingPeriodID, waterAccountFallowStatus.WaterAccountFallowStatusID);
        return newFallowStatus;
    }

    public static async Task<List<WaterAccountFallowStatusDto>> ListByGeographyIDAndReportingPeriodIDForUserAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, UserDto callingUser)
    {
        var result = new List<WaterAccountFallowStatusDto>();

        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsSimpleDtoAsync(dbContext, geographyID, reportingPeriodID);

        var waterAccounts = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Geography).ThenInclude(x => x.GeographyConfiguration)
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.Geography)
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UsageLocationType)
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.ReportingPeriod)
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.CreateUser)
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UpdateUser)
            .Include(x => x.WaterAccountUsers).ThenInclude(x => x.User)
            .Where(x => x.GeographyID == geographyID);

        var hasCurrentGeographyFlags = callingUser.GeographyFlags.TryGetValue(geographyID, out var geographyFlags);
        var isWaterManager = false;
        if (hasCurrentGeographyFlags)
        {
            geographyFlags.TryGetValue(Flag.HasManagerDashboard.FlagName, out isWaterManager);
        }

        callingUser.Flags.TryGetValue(Flag.IsSystemAdmin.FlagName, out var isAdmin);
        if (!isAdmin && !isWaterManager)
        {
            waterAccounts = waterAccounts.Where(x => x.WaterAccountUsers.Select(wau => wau.UserID).Contains(callingUser.UserID));
        }

        var filteredWaterAccounts = await waterAccounts.ToListAsync();
        var waterAccountFallowStatuses = await dbContext.WaterAccountFallowStatuses.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && filteredWaterAccounts.Select(wa => wa.WaterAccountID).Contains(x.WaterAccountID))
            .Include(x => x.SubmittedByUser)
            .Include(x => x.ApprovedByUser)
            .Include(x => x.ReturnedByUser)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .ToListAsync();

        var waterAccountUsersForUserAndGeography = await dbContext.WaterAccountUsers.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Where(x => x.WaterAccount.GeographyID == geographyID && x.UserID == callingUser.UserID)
            .ToListAsync();

        foreach (var filteredWaterAccount in filteredWaterAccounts)
        {
            var waterAccountFallowStatus = waterAccountFallowStatuses.FirstOrDefault(x => x.WaterAccountID == filteredWaterAccount.WaterAccountID);
            var selfReportStatus = waterAccountFallowStatus?.SelfReportStatus.AsSimpleDto();

            var usageLocations = filteredWaterAccount.WaterAccountParcels
                .Where(x => x.ReportingPeriodID == reportingPeriodID)
                .SelectMany(x => x.Parcel.UsageLocations.ToList())
                .Where(x => x.ReportingPeriodID == reportingPeriodID)
                .ToList();

            var waterAccountUser = waterAccountUsersForUserAndGeography.FirstOrDefault(x => x.WaterAccountID == filteredWaterAccount.WaterAccountID);
            var isWaterAccountHolder = false;
            if (waterAccountUser != null)
            {
                var rightsDictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Rights>>(waterAccountUser.WaterAccountRole.Rights, RightsSerializerOptions.JsonSerializerOptions);
                rightsDictionary.TryGetValue(Permission.WaterAccountRights.PermissionName, out var rightsValue);

                isWaterAccountHolder = rightsValue?.CanUpdate ?? false;
            }

            result.Add(new WaterAccountFallowStatusDto
            {
                WaterAccountFallowStatusID = waterAccountFallowStatus?.WaterAccountFallowStatusID,
                SelfReportStatus = selfReportStatus,
                Geography = filteredWaterAccount.Geography.AsMinimalDto(),
                ReportingPeriod = reportingPeriod,
                WaterAccount = filteredWaterAccount.AsWaterAccountMinimalDto(),
                UsageLocations = usageLocations.Select(x => x.AsDto()).ToList(),
                CurrentUserCanEdit = isAdmin || isWaterManager || isWaterAccountHolder,
                SubmittedByUser = waterAccountFallowStatus?.SubmittedByUser?.AsUserWithFullNameDto(),
                SubmittedDate = waterAccountFallowStatus?.SubmittedDate,
                ApprovedByUser = waterAccountFallowStatus?.ApprovedByUser?.AsUserWithFullNameDto(),
                ApprovedDate = waterAccountFallowStatus?.ApprovedDate,
                ReturnedByUser = waterAccountFallowStatus?.ReturnedByUser?.AsUserWithFullNameDto(),
                ReturnedDate = waterAccountFallowStatus?.ReturnedDate,
                CreateUser = waterAccountFallowStatus?.CreateUser?.AsUserWithFullNameDto(),
                CreateDate = waterAccountFallowStatus?.CreateDate,
                UpdateUser = waterAccountFallowStatus?.UpdateUser?.AsUserWithFullNameDto(),
                UpdateDate = waterAccountFallowStatus?.UpdateDate
            });
        }

        return result;
    }

    public static async Task<List<WaterAccountFallowStatusDto>> ListByGeographyIDAsync(QanatDbContext dbContext, int geographyID)
    {
        var fallowStatuses = await dbContext.WaterAccountFallowStatuses.AsNoTracking()
            .Include(x => x.Geography).ThenInclude(x => x.GeographyConfiguration)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.Geography)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UsageLocationType)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.ReportingPeriod)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.CreateUser)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UpdateUser)
            .Include(x => x.SubmittedByUser)
            .Include(x => x.ApprovedByUser)
            .Include(x => x.ReturnedByUser)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsDto(x.ReportingPeriodID, new List<vWaterMeasurementSourceOfRecord>()))
            .ToListAsync();

        return fallowStatuses;
    }

    public static async Task<List<SelfReportSummaryDto>> ListSummariesByGeographyIDAsync(QanatDbContext dbContext, int geographyID, UserDto callingUser)
    {
        var results = new List<SelfReportSummaryDto>();

        var hasCurrentGeographyFlags = callingUser.GeographyFlags.TryGetValue(geographyID, out var geographyFlags);
        var isWaterManager = false;
        if (hasCurrentGeographyFlags)
        {
            geographyFlags.TryGetValue(Flag.HasManagerDashboard.FlagName, out isWaterManager);
        }

        callingUser.Flags.TryGetValue(Flag.IsSystemAdmin.FlagName, out var isAdmin);
        var callingUserIsAdminOrWaterManager = isAdmin || isWaterManager;

        var reportingPeriods = await ReportingPeriods.ListByGeographyIDAsync(dbContext, geographyID, callingUser);
        reportingPeriods = reportingPeriods
            .Where(x => x.FallowSelfReportReadyForAccountHolders && x.FallowSelfReportStartDate.HasValue && x.FallowSelfReportStartDate.Value < DateTime.UtcNow && (x.FallowSelfReportEndDate.HasValue && x.FallowSelfReportEndDate.Value > DateTime.UtcNow))
            .ToList();

        var waterAccounts = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountUsers)
            .Where(x => x.GeographyID == geographyID);

        if (!callingUserIsAdminOrWaterManager)
        {
            waterAccounts = waterAccounts.Where(x => x.WaterAccountUsers.Any(wau => wau.UserID == callingUser.UserID));
        }

        var filteredWaterAccounts = await waterAccounts.ToListAsync();
        foreach (var reportingPeriod in reportingPeriods)
        {
            var waterAccountFallowStatuses = await dbContext.WaterAccountFallowStatuses.AsNoTracking()
                .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID && filteredWaterAccounts.Select(wa => wa.WaterAccountID).Contains(x.WaterAccountID))
                .ToListAsync();

            var selfReportSummaryDto = new SelfReportSummaryDto
            {
                ReportingPeriodID = reportingPeriod.ReportingPeriodID,
                ReportingPeriodName = reportingPeriod.Name,
                NotStartedCount = filteredWaterAccounts.Count(x => !waterAccountFallowStatuses.Any(y => y.WaterAccountID == x.WaterAccountID && y.ReportingPeriodID == reportingPeriod.ReportingPeriodID)),
                InProgressCount = waterAccountFallowStatuses.Count(x => x.SelfReportStatus.SelfReportStatusID == SelfReportStatus.Draft.SelfReportStatusID),
                SubmittedCount = waterAccountFallowStatuses.Count(x => x.SelfReportStatus.SelfReportStatusID == SelfReportStatus.Submitted.SelfReportStatusID),
                ReturnedCount = waterAccountFallowStatuses.Count(x => x.SelfReportStatus.SelfReportStatusID == SelfReportStatus.Returned.SelfReportStatusID),
                ApprovedCount = waterAccountFallowStatuses.Count(x => x.SelfReportStatus.SelfReportStatusID == SelfReportStatus.Approved.SelfReportStatusID)
            };

            results.Add(selfReportSummaryDto);
        }

        return results;
    }

    public static async Task<WaterAccountFallowStatusDto> GetByIDAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int waterAccountFallowStatusID)
    {
        var waterAccountFallowStatus = await dbContext.WaterAccountFallowStatuses.AsNoTracking()
            .Include(x => x.Geography).ThenInclude(x => x.GeographyConfiguration)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.Geography)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UsageLocationType)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.ReportingPeriod)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.CreateUser)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UpdateUser)
            .Include(x => x.SubmittedByUser)
            .Include(x => x.ApprovedByUser)
            .Include(x => x.ReturnedByUser)
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .FirstOrDefaultAsync(x => x.WaterAccountFallowStatusID == waterAccountFallowStatusID);

        var usageLocationIDs = waterAccountFallowStatus?.WaterAccount.WaterAccountParcels
            .Where(x => x.ReportingPeriodID == reportingPeriodID)
            .SelectMany(x => x.Parcel.UsageLocations)
            .Where(x => x.ReportingPeriodID == reportingPeriodID)
            .Select(x => x.UsageLocationID)
            .ToList() ?? new List<int>();

        var usageLocationSourceOfRecords = await dbContext.vWaterMeasurementSourceOfRecords.AsNoTracking()
            .Where(x => usageLocationIDs.Contains(x.UsageLocationID) && x.ReportingPeriodID == reportingPeriodID)
            .ToListAsync();

        return waterAccountFallowStatus?.AsDto(waterAccountFallowStatus.ReportingPeriodID, usageLocationSourceOfRecords);
    }

    public static async Task<List<ErrorMessage>> ValidateSubmitAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int fallowStatusID)
    {
        var errors = new List<ErrorMessage>();

        var waterAccountFallowStatus = await dbContext.WaterAccountFallowStatuses.AsNoTracking()
            .SingleAsync(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterAccountFallowStatusID == fallowStatusID);

        var usageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.Parcel.WaterAccountParcels.Any(wap => wap.ReportingPeriodID == reportingPeriodID && wap.WaterAccountID == waterAccountFallowStatus.WaterAccountID))
            .ToListAsync();

        var missingCoverCropData = usageLocations.All(x => !x.UsageLocationType.CountsAsFallowed);
        if (missingCoverCropData)
        {
            errors.Add(new ErrorMessage("Usage Locations", "At least one usage location must have a type that counts as fallowed."));
        }

        return errors;
    }

    public static async Task<WaterAccountFallowStatusDto> SubmitAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int fallowStatusID, UserDto callingUser)
    {
        var waterAccountFallowStatus = await dbContext.WaterAccountFallowStatuses
            .SingleAsync(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterAccountFallowStatusID == fallowStatusID);

        waterAccountFallowStatus.SelfReportStatusID = SelfReportStatus.Submitted.SelfReportStatusID;
        waterAccountFallowStatus.SubmittedByUserID = callingUser.UserID;
        waterAccountFallowStatus.SubmittedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        var updatedFallowStatus = await GetByIDAsync(dbContext, geographyID, reportingPeriodID, fallowStatusID);
        return updatedFallowStatus;
    }

    public static async Task<List<ErrorMessage>> ValidateApproveAsync(QanatDbContext dbContext, int geographyID, List<int> fallowStatusIDs)
    {
        var errors = new List<ErrorMessage>();

        var waterAccountFallowStatuses = await dbContext.WaterAccountFallowStatuses.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var existingFallowStatusIDs = waterAccountFallowStatuses.Select(x => x.WaterAccountFallowStatusID);
        var invalidFallowStatusIDs = fallowStatusIDs.Except(existingFallowStatusIDs).ToList();
        if (invalidFallowStatusIDs.Any())
        {
            errors.Add(new ErrorMessage("Invalid Fallow Status IDs", $"The following fallow status IDs are invalid: {string.Join(", ", invalidFallowStatusIDs)}"));
        }

        return errors;
    }

    public static async Task ApproveAsync(QanatDbContext dbContext, int geographyID, List<int> fallowStatusIDs, UserDto callingUser)
    {
        var waterAccountFallowStatuses = await dbContext.WaterAccountFallowStatuses
            .Where(x => x.GeographyID == geographyID && fallowStatusIDs.Contains(x.WaterAccountFallowStatusID))
            .ToListAsync();

        foreach (var waterAccountFallowStatus in waterAccountFallowStatuses)
        {
            if (waterAccountFallowStatus.SelfReportStatus.SelfReportStatusID != SelfReportStatus.Approved.SelfReportStatusID)
            {
                waterAccountFallowStatus.SelfReportStatusID = SelfReportStatus.Approved.SelfReportStatusID;
                waterAccountFallowStatus.ApprovedByUserID = callingUser.UserID;
                waterAccountFallowStatus.ApprovedDate = DateTime.UtcNow;
                waterAccountFallowStatus.ReturnedByUserID = null;
                waterAccountFallowStatus.ReturnedDate = null;
            }
        }

        await dbContext.SaveChangesAsync();
    }

    public static async Task<List<ErrorMessage>> ValidateReturnAsync(QanatDbContext dbContext, int geographyID, List<int> fallowStatusIDs)
    {
        var errors = new List<ErrorMessage>();
        var waterAccountFallowStatuses = await dbContext.WaterAccountFallowStatuses.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var existingFallowStatusIDs = waterAccountFallowStatuses.Select(x => x.WaterAccountFallowStatusID);
        var invalidFallowStatusIDs = fallowStatusIDs.Except(existingFallowStatusIDs).ToList();
        if (invalidFallowStatusIDs.Any())
        {
            errors.Add(new ErrorMessage("Invalid Fallow Status IDs", $"The following fallow status IDs are invalid: {string.Join(", ", invalidFallowStatusIDs)}"));
        }

        return errors;
    }

    public static async Task ReturnAsync(QanatDbContext dbContext, int geographyID, List<int> fallowStatusIDs, UserDto callingUser)
    {
        var waterAccountFallowStatuses = await dbContext.WaterAccountFallowStatuses
            .Where(x => x.GeographyID == geographyID && fallowStatusIDs.Contains(x.WaterAccountFallowStatusID))
            .ToListAsync();

        foreach (var waterAccountFallowStatus in waterAccountFallowStatuses)
        {
            if (waterAccountFallowStatus.SelfReportStatus.SelfReportStatusID != SelfReportStatus.Returned.SelfReportStatusID)
            {
                waterAccountFallowStatus.SelfReportStatusID = SelfReportStatus.Returned.SelfReportStatusID;
                waterAccountFallowStatus.ReturnedByUserID = callingUser.UserID;
                waterAccountFallowStatus.ReturnedDate = DateTime.UtcNow;
                waterAccountFallowStatus.ApprovedByUserID = null;
                waterAccountFallowStatus.ApprovedDate = null;
            }
        }

        await dbContext.SaveChangesAsync();
    }
}