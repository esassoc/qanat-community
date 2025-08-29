using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities.ExtensionMethods;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.EFModels.Entities;

public class WaterAccountCoverCropStatuses
{
    public static async Task<List<ErrorMessage>> ValidateCreateAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int waterAccountID, UserDto callingUser)
    {
        var errors = new List<ErrorMessage>();
        var existingCoverCropStatus = await dbContext.WaterAccountCoverCropStatuses.AnyAsync(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterAccountID == waterAccountID);
        if (existingCoverCropStatus)
        {
            errors.Add(new ErrorMessage("WaterAccountCoverCropStatus", "A Cover Crop status already exists for this water account in the specified geography and reporting period."));
        }

        return errors;
    }

    public static async Task<WaterAccountCoverCropStatusDto> CreateAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int waterAccountID, UserDto callingUser)
    {
        var waterAccountCoverCropStatus = new WaterAccountCoverCropStatus
        {
            GeographyID = geographyID,
            ReportingPeriodID = reportingPeriodID,
            WaterAccountID = waterAccountID,
            SelfReportStatusID = SelfReportStatus.Draft.SelfReportStatusID,
            CreateUserID = callingUser.UserID,
            CreateDate = DateTime.UtcNow
        };

        dbContext.WaterAccountCoverCropStatuses.Add(waterAccountCoverCropStatus);
        await dbContext.SaveChangesAsync();

        var newCoverCropStatus = await GetByIDAsync(dbContext, geographyID, reportingPeriodID, waterAccountCoverCropStatus.WaterAccountCoverCropStatusID);
        return newCoverCropStatus;
    }

    public static async Task<List<WaterAccountCoverCropStatusDto>> ListByGeographyIDAndReportingPeriodIDForUserAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, UserDto callingUser)
    {
        var result = new List<WaterAccountCoverCropStatusDto>();

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
        var waterAccountCoverCropStatuses = await dbContext.WaterAccountCoverCropStatuses.AsNoTracking()
            .Include(x => x.CreateUser)
            .Include(x => x.UpdateUser)
            .Include(x => x.SubmittedByUser)
            .Include(x => x.ApprovedByUser)
            .Include(x => x.ReturnedByUser)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && filteredWaterAccounts.Select(wa => wa.WaterAccountID).Contains(x.WaterAccountID))
            .ToListAsync();

        var waterAccountUsersForUserAndGeography = await dbContext.WaterAccountUsers.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Where(x => x.WaterAccount.GeographyID == geographyID && x.UserID == callingUser.UserID)
            .ToListAsync();

        foreach (var filteredWaterAccount in filteredWaterAccounts)
        {
            var waterAccountCoverCropStatus = waterAccountCoverCropStatuses.FirstOrDefault(x => x.WaterAccountID == filteredWaterAccount.WaterAccountID);
            var selfReportStatus = waterAccountCoverCropStatus?.SelfReportStatus.AsSimpleDto();

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

            result.Add(new WaterAccountCoverCropStatusDto
            {
                WaterAccountCoverCropStatusID = waterAccountCoverCropStatus?.WaterAccountCoverCropStatusID,
                SelfReportStatus = selfReportStatus,
                Geography = filteredWaterAccount.Geography.AsMinimalDto(),
                ReportingPeriod = reportingPeriod,
                WaterAccount = filteredWaterAccount.AsWaterAccountMinimalDto(),
                UsageLocations = usageLocations.Select(x => x.AsDto()).ToList(),
                CurrentUserCanEdit = isAdmin || isWaterManager || isWaterAccountHolder,
                SubmittedByUser = waterAccountCoverCropStatus?.SubmittedByUser?.AsUserWithFullNameDto(),
                SubmittedDate = waterAccountCoverCropStatus?.SubmittedDate,
                ApprovedByUser = waterAccountCoverCropStatus?.ApprovedByUser?.AsUserWithFullNameDto(),
                ApprovedDate = waterAccountCoverCropStatus?.ApprovedDate,
                ReturnedByUser = waterAccountCoverCropStatus?.ReturnedByUser?.AsUserWithFullNameDto(),
                ReturnedDate = waterAccountCoverCropStatus?.ReturnedDate,
                CreateUser = waterAccountCoverCropStatus?.CreateUser?.AsUserWithFullNameDto(),
                CreateDate = waterAccountCoverCropStatus?.CreateDate,
                UpdateUser = waterAccountCoverCropStatus?.UpdateUser?.AsUserWithFullNameDto(),
                UpdateDate = waterAccountCoverCropStatus?.UpdateDate
            });
        }

        return result;
    }

    public static async Task<List<WaterAccountCoverCropStatusDto>> ListByGeographyIDAsync(QanatDbContext dbContext, int geographyID)
    {
        var coverCropStatuses = await dbContext.WaterAccountCoverCropStatuses.AsNoTracking()
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

        return coverCropStatuses;
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
            .Where(x => x.CoverCropSelfReportReadyForAccountHolders && x.CoverCropSelfReportStartDate.HasValue && x.CoverCropSelfReportStartDate.Value < DateTime.UtcNow && (x.CoverCropSelfReportEndDate.HasValue && x.CoverCropSelfReportEndDate.Value > DateTime.UtcNow))
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
            var waterAccountCoverCropStatuses = await dbContext.WaterAccountCoverCropStatuses.AsNoTracking()
                .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID && filteredWaterAccounts.Select(wa => wa.WaterAccountID).Contains(x.WaterAccountID))
                .ToListAsync();

            var selfReportSummaryDto = new SelfReportSummaryDto
            {
                ReportingPeriodID = reportingPeriod.ReportingPeriodID,
                ReportingPeriodName = reportingPeriod.Name,
                NotStartedCount = filteredWaterAccounts.Count(x => !waterAccountCoverCropStatuses.Any(y => y.WaterAccountID == x.WaterAccountID && y.ReportingPeriodID == reportingPeriod.ReportingPeriodID)),
                InProgressCount = waterAccountCoverCropStatuses.Count(x => x.SelfReportStatus.SelfReportStatusID == SelfReportStatus.Draft.SelfReportStatusID),
                SubmittedCount = waterAccountCoverCropStatuses.Count(x => x.SelfReportStatus.SelfReportStatusID == SelfReportStatus.Submitted.SelfReportStatusID),
                ReturnedCount = waterAccountCoverCropStatuses.Count(x => x.SelfReportStatus.SelfReportStatusID == SelfReportStatus.Returned.SelfReportStatusID),
                ApprovedCount = waterAccountCoverCropStatuses.Count(x => x.SelfReportStatus.SelfReportStatusID == SelfReportStatus.Approved.SelfReportStatusID)
            };
            results.Add(selfReportSummaryDto);
        }

        return results;
    }

    public static async Task<WaterAccountCoverCropStatusDto> GetByIDAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int waterAccountCoverCropStatusID)
    {
        var waterAccountCoverCropStatus = await dbContext.WaterAccountCoverCropStatuses.AsNoTracking()
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
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatusID);

        var usageLocationIDs = waterAccountCoverCropStatus?.WaterAccount.WaterAccountParcels
            .Where(x => x.ReportingPeriodID == reportingPeriodID)
            .SelectMany(x => x.Parcel.UsageLocations)
            .Where(x => x.ReportingPeriodID == reportingPeriodID)
            .Select(x => x.UsageLocationID)
            .ToList() ?? new List<int>();

        var usageLocationSourceOfRecords = await dbContext.vWaterMeasurementSourceOfRecords.AsNoTracking()
            .Where(x => usageLocationIDs.Contains(x.UsageLocationID) && x.ReportingPeriodID == reportingPeriodID)
            .ToListAsync();

        return waterAccountCoverCropStatus?.AsDto(waterAccountCoverCropStatus.ReportingPeriodID, usageLocationSourceOfRecords);
    }

    public static async Task<List<ErrorMessage>> ValidateSubmitAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int coverCropStatusID)
    {
        var errors = new List<ErrorMessage>();

        var waterAccountCoverCropStatus = await dbContext.WaterAccountCoverCropStatuses.AsNoTracking()
            .SingleAsync(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterAccountCoverCropStatusID == coverCropStatusID);

        var usageLocations = await dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.UsageLocationType)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.Parcel.WaterAccountParcels.Any(wap => wap.ReportingPeriodID == reportingPeriodID && wap.WaterAccountID == waterAccountCoverCropStatus.WaterAccountID))
            .ToListAsync();

        var missingCoverCropData = usageLocations.All(x => !x.UsageLocationType.CountsAsCoverCropped);
        if (missingCoverCropData)
        {
            errors.Add(new ErrorMessage("Usage Locations", "At least one usage location must have a type that counts as cover cropped."));
        }

        return errors;
    }

    public static async Task<WaterAccountCoverCropStatusDto> SubmitAsync(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int coverCropStatusID, UserDto callingUser)
    {
        var waterAccountCoverCropStatus = await dbContext.WaterAccountCoverCropStatuses
            .SingleAsync(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterAccountCoverCropStatusID == coverCropStatusID);

        waterAccountCoverCropStatus.SelfReportStatusID = SelfReportStatus.Submitted.SelfReportStatusID;
        waterAccountCoverCropStatus.SubmittedByUserID = callingUser.UserID;
        waterAccountCoverCropStatus.SubmittedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        var updatedCoverCropStatus = await GetByIDAsync(dbContext, geographyID, reportingPeriodID, coverCropStatusID);
        return updatedCoverCropStatus;
    }

    public static async Task<List<ErrorMessage>> ValidateApproveAsync(QanatDbContext dbContext, int geographyID, List<int> coverCropStatusIDs)
    {
        var errors = new List<ErrorMessage>();

        var waterAccountCoverCropStatuses = await dbContext.WaterAccountCoverCropStatuses.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var existingCoverCropStatusIDs = waterAccountCoverCropStatuses.Select(x => x.WaterAccountCoverCropStatusID);
        var invalidCoverCropStatusIDs = coverCropStatusIDs.Except(existingCoverCropStatusIDs).ToList();
        if (invalidCoverCropStatusIDs.Any())
        {
            errors.Add(new ErrorMessage("Invalid Cover Crop Status IDs", $"The following cover crop status IDs are invalid: {string.Join(", ", invalidCoverCropStatusIDs)}"));
        }

        return errors;
    }

    public static async Task ApproveAsync(QanatDbContext dbContext, int geographyID, List<int> coverCropStatusIDs, UserDto callingUser)
    {
        var waterAccountCoverCropStatuses = await dbContext.WaterAccountCoverCropStatuses
            .Where(x => x.GeographyID == geographyID && coverCropStatusIDs.Contains(x.WaterAccountCoverCropStatusID))
            .ToListAsync();

        foreach (var waterAccountCoverCropStatus in waterAccountCoverCropStatuses)
        {
            if (waterAccountCoverCropStatus.SelfReportStatus.SelfReportStatusID != SelfReportStatus.Approved.SelfReportStatusID)
            {
                waterAccountCoverCropStatus.SelfReportStatusID = SelfReportStatus.Approved.SelfReportStatusID;
                waterAccountCoverCropStatus.ApprovedByUserID = callingUser.UserID;
                waterAccountCoverCropStatus.ApprovedDate = DateTime.UtcNow;
                waterAccountCoverCropStatus.ReturnedByUserID = null;
                waterAccountCoverCropStatus.ReturnedDate = null;
            }
        }

        await dbContext.SaveChangesAsync();
    }

    public static async Task<List<ErrorMessage>> ValidateReturnAsync(QanatDbContext dbContext, int geographyID, List<int> coverCropStatusIDs)
    {
        var errors = new List<ErrorMessage>();
        var waterAccountCoverCropStatuses = await dbContext.WaterAccountCoverCropStatuses.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var existingCoverCropStatusIDs = waterAccountCoverCropStatuses.Select(x => x.WaterAccountCoverCropStatusID);
        var invalidCoverCropStatusIDs = coverCropStatusIDs.Except(existingCoverCropStatusIDs).ToList();
        if (invalidCoverCropStatusIDs.Any())
        {
            errors.Add(new ErrorMessage("Invalid Cover Crop Status IDs", $"The following cover crop status IDs are invalid: {string.Join(", ", invalidCoverCropStatusIDs)}"));
        }

        return errors;
    }

    public static async Task ReturnAsync(QanatDbContext dbContext, int geographyID, List<int> coverCropStatusIDs, UserDto callingUser)
    {
        var waterAccountCoverCropStatuses = await dbContext.WaterAccountCoverCropStatuses
            .Where(x => x.GeographyID == geographyID && coverCropStatusIDs.Contains(x.WaterAccountCoverCropStatusID))
            .ToListAsync();

        foreach (var waterAccountCoverCropStatus in waterAccountCoverCropStatuses)
        {
            if (waterAccountCoverCropStatus.SelfReportStatus.SelfReportStatusID != SelfReportStatus.Returned.SelfReportStatusID)
            {
                waterAccountCoverCropStatus.SelfReportStatusID = SelfReportStatus.Returned.SelfReportStatusID;
                waterAccountCoverCropStatus.ReturnedByUserID = callingUser.UserID;
                waterAccountCoverCropStatus.ReturnedDate = DateTime.UtcNow;
                waterAccountCoverCropStatus.ApprovedByUserID = null;
                waterAccountCoverCropStatus.ApprovedDate = null;
            }
        }

        await dbContext.SaveChangesAsync();
    }
}