using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NetTopologySuite.Geometries;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;
using Qanat.Models.DataTransferObjects.User;

namespace Qanat.EFModels.Entities;

public static class Geographies
{
    public const int GSABoundaryBuffer = 0;

    private static IIncludableQueryable<Geography, User> GetImpl(QanatDbContext dbContext)
    {
        return dbContext.Geographies.AsNoTracking()
            .Include(x => x.SourceOfRecordWaterMeasurementType)
            .Include(x => x.GeographyConfiguration)
            .Include(x => x.GeographyBoundary)
            .Include(x => x.GeographyAllocationPlanConfiguration)
            .Include(x => x.GeographyUsers).ThenInclude(x => x.User);
    }

    public static List<GeographyPublicDto> ListAsPublicDto(QanatDbContext dbContext)
    {
        return dbContext.Geographies.AsNoTracking().Include(x => x.GeographyAllocationPlanConfiguration)
            .Include(x => x.GeographyConfiguration).Select(x => x.AsPublicDto()).ToList();
    }

    public static List<GeographyDto> ListAsDto(QanatDbContext dbContext)
    {
        return GetImpl(dbContext)
            .OrderBy(x => x.GeographyName)
            .Select(x => x.AsDto()).ToList();
    }

    public static List<GeographySimpleDto> ListAsSimpleDto(QanatDbContext dbContext)
    {
        return GetImpl(dbContext)
            .Select(x => x.AsSimpleDto()).ToList();
    }

    public static List<GeographyDisplayDto> ListAsDisplayDto(QanatDbContext dbContext)
    {
        return dbContext.Geographies.AsNoTracking()
            .Select(x => x.AsDisplayDto()).ToList();
    }

    public static GeographyDto GetByIDAsDto(QanatDbContext dbContext, int geographyID)
    {
        return GetImpl(dbContext).SingleOrDefault(x => x.GeographyID == geographyID)?.AsDto();
    }

    public static Geography GetByID(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Geographies.AsNoTracking().SingleOrDefault(x => x.GeographyID == geographyID);
    }

    public static GeographyDisplayDto GetByIDAsDisplayDto(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Geographies.AsNoTracking().SingleOrDefault(x => x.GeographyID == geographyID)?.AsDisplayDto();
    }

    public static GeographyPublicDto GetByNameAsPublicDto(QanatDbContext dbContext, string geographyName)
    {
        return GetImpl(dbContext).AsNoTracking().SingleOrDefault(x => x.GeographyName.ToLower() == geographyName)?.AsPublicDto();
    }

    public static async Task<GeographyMinimalDto> GetByNameAsMinimalDtoAsync(QanatDbContext dbContext, string geographyName)
    {
        var geography = await dbContext.Geographies.AsNoTracking()
            .Include(x => x.GeographyConfiguration)
            .Include(x => x.GeographyAllocationPlanConfiguration)
            .SingleOrDefaultAsync(x => x.GeographyName.ToLower() == geographyName.ToLower().Trim());

        return geography?.AsMinimalDto();
    }

    public static async Task<GeographyMinimalDto> GetByIDAsMinimalDtoAsync(QanatDbContext dbContext, int geographyID)
    {
        var geography = await dbContext.Geographies.AsNoTracking()
            .Include(x => x.GeographyConfiguration)
            .Include(x => x.GeographyAllocationPlanConfiguration)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID);

        return geography?.AsMinimalDto();
    }

    public static List<GeographyMinimalDto> ListAsGeographyMinimalDto(QanatDbContext dbContext)
    {
        return dbContext.Geographies.AsNoTracking()
            .Include(x => x.GeographyConfiguration)
            .Include(x => x.GeographyAllocationPlanConfiguration)
            .OrderBy(x => x.IsDemoGeography ? 0 : 1).ThenBy(x => x.GeographyName)
            .Select(x => x.AsMinimalDto())
            .ToList();
    }

    public static List<GeographyMinimalDto> ListByIDsAsGeographyMinimalDto(QanatDbContext dbContext, List<int> geographyIDs)
    {
        return dbContext.Geographies.AsNoTracking()
            .Include(x => x.GeographyConfiguration)
            .Include(x => x.GeographyAllocationPlanConfiguration)
            .Where(x => geographyIDs.Contains(x.GeographyID)).AsEnumerable()
            .OrderBy(x => x.IsDemoGeography ? 0 : 1).ThenBy(x => x.GeographyName)
            .Select(x => x.AsMinimalDto())
            .ToList();
    }

    public static List<GeographyDisplayDto> ListByIDsAsGeographyDisplayDto(QanatDbContext dbContext, List<int> geographyIDs)
    {
        return dbContext.Geographies.AsNoTracking()
            .Where(x => geographyIDs.Contains(x.GeographyID)).AsEnumerable()
            .OrderBy(x => x.GeographyName)
            .Select(x => x.AsDisplayDto())
            .ToList();
    }

    public static WaterMeasurementType GetSourceOfRecordWaterMeasurementTypeByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Geographies.AsNoTracking()
            .Include(x => x.SourceOfRecordWaterMeasurementType)
            .SingleOrDefault(x => x.GeographyID == geographyID)?.SourceOfRecordWaterMeasurementType;
    }

    public static GeographyForAdminEditorsDto GetAsAdminGeographyUpdateRequestDto(QanatDbContext dbContext, int geographyID)
    {
        var geography = dbContext.Geographies.AsNoTracking().Include(geography => geography.GeographyBoundary)
            .Include(geography => geography.GeographyUsers).ThenInclude(geographyUser => geographyUser.User).Single(x => x.GeographyID == geographyID);

        var adminGeographyUpdateRequestDto = new GeographyForAdminEditorsDto()
        {
            GeographyID = geography.GeographyID,
            GeographyDisplayName = geography.GeographyDisplayName,
            APNRegexPattern = geography.APNRegexPattern,
            APNRegexDisplay = geography.APNRegexPatternDisplay,
            LandownerDashboardSupplyLabel = geography.LandownerDashboardSupplyLabel,
            LandownerDashboardUsageLabel = geography.LandownerDashboardUsageLabel,
            ContactEmail = geography.ContactEmail,
            ContactPhoneNumber = geography.ContactPhoneNumber,
            ContactAddressLine1 = geography.ContactAddressLine1,
            ContactAddressLine2 = geography.ContactAddressLine2,
            AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges,
            ShowSupplyOnWaterBudgetComponent = geography.ShowSupplyOnWaterBudgetComponent,
            WaterBudgetSlotAHeader = geography.WaterBudgetSlotAHeader,
            WaterBudgetSlotAWaterMeasurementTypeID = geography.WaterBudgetSlotAWaterMeasurementTypeID,
            WaterBudgetSlotBHeader = geography.WaterBudgetSlotBHeader,
            WaterBudgetSlotBWaterMeasurementTypeID = geography.WaterBudgetSlotBWaterMeasurementTypeID,
            WaterBudgetSlotCHeader = geography.WaterBudgetSlotCHeader,
            WaterBudgetSlotCWaterMeasurementTypeID = geography.WaterBudgetSlotCWaterMeasurementTypeID,
            WaterManagers = geography.GeographyUsers
                .Where(x => x.GeographyRole.GeographyRoleID == (int)GeographyRoleEnum.WaterManager)
                .Select(x => x.User.AsUserDto()).ToList(),
            BoundingBox = geography.GeographyBoundary != null ?
                new BoundingBoxDto(new List<Geometry>() { geography.GeographyBoundary.BoundingBox }) : null,

        };

        return adminGeographyUpdateRequestDto;
    }

    public static List<ErrorMessage> ValidateGeographyUpdate(QanatDbContext dbContext, GeographyForAdminEditorsDto requestDto)
    {
        var errors = new List<ErrorMessage>();

        try
        {
            var regexTestString = requestDto.APNRegexDisplay.Replace('X', '1');
            var regexPatternDisplayMatchesPattern = Parcels.IsValidParcelNumber(requestDto.APNRegexPattern, regexTestString);

            if (!regexPatternDisplayMatchesPattern)
            {
                errors.Add(new ErrorMessage() { Type = "APN Regex Display", Message = "The Regex Pattern Display and the APN Regex pattern do not match!" });
            }
        }
        catch (Exception e)
        {
            errors.Add(new ErrorMessage() { Type = "APN Regex", Message = $"Unable to validate the provided APN Regex pattern." });
        }

        if (!requestDto.ShowSupplyOnWaterBudgetComponent)
        {
            var waterMeasurementTypes = WaterMeasurementTypes.ListAsSimpleDto(dbContext, requestDto.GeographyID);
            if (string.IsNullOrWhiteSpace(requestDto.WaterBudgetSlotAHeader))
            {
                errors.Add(new ErrorMessage() { Type = "Water Budget Slot A Header", Message = "Water Budget Slot A Header is required." });
            }

            if (!requestDto.WaterBudgetSlotAWaterMeasurementTypeID.HasValue)
            {
                errors.Add(new ErrorMessage() { Type = "Water Budget Slot A Water Measurement Type", Message = "Water Budget Slot A Water Measurement Type is required." });
            }
            else
            {
                var slotAWaterMeasurementType = waterMeasurementTypes.SingleOrDefault(x => x.WaterMeasurementTypeID == requestDto.WaterBudgetSlotAWaterMeasurementTypeID);
                if (slotAWaterMeasurementType == null)
                {
                    errors.Add(new ErrorMessage() { Type = "Water Budget Slot A Water Measurement Type", Message = $"Could not find a Water Measurement Type with ID {requestDto.WaterBudgetSlotAWaterMeasurementTypeID}." });
                }
            }

            if (string.IsNullOrWhiteSpace(requestDto.WaterBudgetSlotBHeader))
            {
                errors.Add(new ErrorMessage() { Type = "Water Budget Slot B Header", Message = "Water Budget Slot B Header is required." });
            }

            if (!requestDto.WaterBudgetSlotBWaterMeasurementTypeID.HasValue)
            {
                errors.Add(new ErrorMessage() { Type = "Water Budget Slot B Water Measurement Type", Message = "Water Budget Slot B Water Measurement Type is required." });
            }
            else
            {
                var slotBWaterMeasurementType = waterMeasurementTypes.SingleOrDefault(x => x.WaterMeasurementTypeID == requestDto.WaterBudgetSlotBWaterMeasurementTypeID);
                if (slotBWaterMeasurementType == null)
                {
                    errors.Add(new ErrorMessage() { Type = "Water Budget Slot B Water Measurement Type", Message = $"Could not find a Water Measurement Type with ID {requestDto.WaterBudgetSlotBWaterMeasurementTypeID}." });
                }
            }

            if (string.IsNullOrWhiteSpace(requestDto.WaterBudgetSlotCHeader))
            {
                errors.Add(new ErrorMessage() { Type = "Water Budget Slot C Header", Message = "Water Budget Slot C Header is required." });
            }

            if (!requestDto.WaterBudgetSlotCWaterMeasurementTypeID.HasValue)
            {
                errors.Add(new ErrorMessage() { Type = "Water Budget Slot C Water Measurement Type", Message = "Water Budget Slot C Water Measurement Type is required." });
            }
            else
            {
                var slotCWaterMeasurementType = waterMeasurementTypes.SingleOrDefault(x => x.WaterMeasurementTypeID == requestDto.WaterBudgetSlotCWaterMeasurementTypeID);
                if (slotCWaterMeasurementType == null)
                {
                    errors.Add(new ErrorMessage() { Type = "Water Budget Slot C Water Measurement Type", Message = $"Could not find a Water Measurement Type with ID {requestDto.WaterBudgetSlotCWaterMeasurementTypeID}." });
                }
            }
        }

        return errors;
    }

    public static void UpdateBasicGeographyFields(QanatDbContext dbContext, int geographyID, GeographyForAdminEditorsDto requestDto)
    {
        var geography = dbContext.Geographies.Single(x => x.GeographyID == geographyID);

        geography.GeographyDisplayName = requestDto.GeographyDisplayName;
        geography.APNRegexPattern = requestDto.APNRegexPattern;
        geography.APNRegexPatternDisplay = requestDto.APNRegexDisplay;
        geography.LandownerDashboardSupplyLabel = requestDto.LandownerDashboardSupplyLabel;
        geography.LandownerDashboardUsageLabel = requestDto.LandownerDashboardUsageLabel;
        geography.ContactEmail = requestDto.ContactEmail;
        geography.ContactPhoneNumber = requestDto.ContactPhoneNumber;
        geography.ContactAddressLine1 = requestDto.ContactAddressLine1;
        geography.ContactAddressLine2 = requestDto.ContactAddressLine2;
        geography.AllowLandownersToRequestAccountChanges = requestDto.AllowLandownersToRequestAccountChanges;
        geography.ShowSupplyOnWaterBudgetComponent = requestDto.ShowSupplyOnWaterBudgetComponent;
        geography.WaterBudgetSlotAHeader = requestDto.WaterBudgetSlotAHeader;
        geography.WaterBudgetSlotAWaterMeasurementTypeID = requestDto.WaterBudgetSlotAWaterMeasurementTypeID;
        geography.WaterBudgetSlotBHeader = requestDto.WaterBudgetSlotBHeader;
        geography.WaterBudgetSlotBWaterMeasurementTypeID = requestDto.WaterBudgetSlotBWaterMeasurementTypeID;
        geography.WaterBudgetSlotCHeader = requestDto.WaterBudgetSlotCHeader;
        geography.WaterBudgetSlotCWaterMeasurementTypeID = requestDto.WaterBudgetSlotCWaterMeasurementTypeID;

        dbContext.SaveChanges();
    }

    public static async Task<GeographyMinimalDto> UpdateGeographySelfReportingAsync(QanatDbContext dbContext, int geographyID, GeographySelfReportEnabledUpdateDto updateDto)
    {
        var geography = dbContext.Geographies.Single(x => x.GeographyID == geographyID);

        geography.AllowWaterMeasurementSelfReporting = updateDto.AllowWaterMeasurementSelfReporting;
        geography.AllowFallowSelfReporting = updateDto.AllowFallowSelfReporting;
        geography.AllowCoverCropSelfReporting = updateDto.AllowCoverCropSelfReporting;

        await dbContext.SaveChangesAsync();

        var updatedGeography = await GetByIDAsMinimalDtoAsync(dbContext, geographyID);
        return updatedGeography;
    }

    public static List<ErrorMessage> ValidateUpdateGeographyWaterManagers(QanatDbContext dbContext, int geographyID, List<GeographyWaterManagerDto> geographyWaterManagerDtos)
    {
        var errors = new List<ErrorMessage>();

        var existingNonManagers = dbContext.GeographyUsers.Where(x => x.GeographyRoleID != (int)GeographyRoleEnum.WaterManager && x.GeographyID == geographyID).ToList();
        var usersAssignedToSpecificWaterAccounts = geographyWaterManagerDtos.Where(x => existingNonManagers.Select(y => y.UserID).Contains(x.UserID)).ToList();

        foreach (var usersAssignedToSpecificWaterAccount in usersAssignedToSpecificWaterAccounts)
        {
            errors.Add(new ErrorMessage() { Type = "User", Message = $"{usersAssignedToSpecificWaterAccount.UserFullName} can't be added as a Water Manager while assigned to specific water accounts within this geography." });
        }

        return errors;
    }

    public static GeographyDto UpdateGeographyWaterManagers(QanatDbContext dbContext, int geographyID, List<GeographyWaterManagerDto> geographyWaterManagerDtos)
    {
        var waterManagerRole = (int)GeographyRoleEnum.WaterManager;

        var existingWaterManagersForGeography = dbContext.GeographyUsers.Where(x =>
            x.GeographyRoleID == waterManagerRole && x.GeographyID == geographyID).ToList();

        var userIDsWhoReceiveNotifications = geographyWaterManagerDtos.Where(x => x.ReceivesNotifications)
            .Select(x => x.UserID).ToList();
        existingWaterManagersForGeography.ForEach(x => x.ReceivesNotifications = userIDsWhoReceiveNotifications.Contains(x.UserID));

        var usersToRemove =
            existingWaterManagersForGeography.Where(x => !geographyWaterManagerDtos.Select(y => y.UserID).Contains(x.UserID)).ToList();
        var usersToAdd = geographyWaterManagerDtos
            .Where(x => !existingWaterManagersForGeography.Select(y => y.UserID).Contains(x.UserID))
            .Select(x => new GeographyUser()
            {
                GeographyID = geographyID,
                UserID = x.UserID,
                GeographyRoleID = waterManagerRole,
                ReceivesNotifications = x.ReceivesNotifications
            }).ToList();

        dbContext.GeographyUsers.RemoveRange(usersToRemove);
        dbContext.GeographyUsers.AddRange(usersToAdd);

        dbContext.SaveChanges();

        return GetByIDAsDto(dbContext, geographyID);
    }
}