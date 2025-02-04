using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NetTopologySuite.Geometries;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.EFModels.Entities;

public static class Geographies
{
    public static int GSABoundaryBuffer = 0;

    private static IIncludableQueryable<Geography, User> GetImpl(QanatDbContext dbContext)
    {
        return dbContext.Geographies.AsNoTracking()
            .Include(x => x.SourceOfRecordWaterMeasurementType)
            .Include(x => x.GeographyConfiguration)
            .Include(x => x.GeographyBoundary)
            .Include(x => x.GeographyAllocationPlanConfiguration)
            .Include(x => x.DefaultReportingPeriod).ThenInclude(x => x.Geography)
            .Include(x => x.DefaultReportingPeriod).ThenInclude(x => x.CreateUser)
            .Include(x => x.DefaultReportingPeriod).ThenInclude(x => x.UpdateUser)
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

    public static GeographyDto GetByNameAsDto(QanatDbContext dbContext, string geographyName)
    {
        return GetImpl(dbContext).AsNoTracking().SingleOrDefault(x => x.GeographyName.ToLower() == geographyName)?.AsDto();
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
            DefaultReportingPeriodID = geography.DefaultReportingPeriodID,
            APNRegexPattern = geography.APNRegexPattern,
            APNRegexDisplay = geography.APNRegexPatternDisplay,
            LandownerDashboardSupplyLabel = geography.LandownerDashboardSupplyLabel,
            LandownerDashboardUsageLabel = geography.LandownerDashboardUsageLabel,
            ContactEmail = geography.ContactEmail,
            ContactPhoneNumber = geography.ContactPhoneNumber,
            DisplayUsageGeometriesAsField = geography.DisplayUsageGeometriesAsField,
            AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges,
            AllowWaterMeasurementSelfReporting = geography.AllowWaterMeasurementSelfReporting,
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

        var currentYear = DateTime.Now.Year;

        var reportingPeriod = dbContext.ReportingPeriods.AsNoTracking()
            .SingleOrDefault(x => x.ReportingPeriodID == requestDto.DefaultReportingPeriodID);

        if (reportingPeriod == null)
        {
            errors.Add(new ErrorMessage() { Type = "Default Reporting Period", Message = $"Could not find a Reporting Period with the ID {requestDto.DefaultReportingPeriodID}." });
        }
        else if (reportingPeriod.StartDate.Year > currentYear) //MK 1/30/2025: Not sure this check is still relevant, seems like it can break non calendar year reporting periods.
        {
            errors.Add(new ErrorMessage() { Type = "Default Reporting Period", Message = $"Can not set Default Reporting Period to a future year." });
        }

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
        geography.DefaultReportingPeriodID = requestDto.DefaultReportingPeriodID;
        geography.APNRegexPattern = requestDto.APNRegexPattern;
        geography.APNRegexPatternDisplay = requestDto.APNRegexDisplay;
        geography.LandownerDashboardSupplyLabel = requestDto.LandownerDashboardSupplyLabel;
        geography.LandownerDashboardUsageLabel = requestDto.LandownerDashboardUsageLabel;
        geography.ContactEmail = requestDto.ContactEmail;
        geography.ContactPhoneNumber = requestDto.ContactPhoneNumber;
        geography.DisplayUsageGeometriesAsField = requestDto.DisplayUsageGeometriesAsField;
        geography.AllowLandownersToRequestAccountChanges = requestDto.AllowLandownersToRequestAccountChanges;
        geography.AllowWaterMeasurementSelfReporting = requestDto.AllowWaterMeasurementSelfReporting;
        geography.ShowSupplyOnWaterBudgetComponent = requestDto.ShowSupplyOnWaterBudgetComponent;
        geography.WaterBudgetSlotAHeader = requestDto.WaterBudgetSlotAHeader;
        geography.WaterBudgetSlotAWaterMeasurementTypeID = requestDto.WaterBudgetSlotAWaterMeasurementTypeID;
        geography.WaterBudgetSlotBHeader = requestDto.WaterBudgetSlotBHeader;
        geography.WaterBudgetSlotBWaterMeasurementTypeID = requestDto.WaterBudgetSlotBWaterMeasurementTypeID;
        geography.WaterBudgetSlotCHeader = requestDto.WaterBudgetSlotCHeader;
        geography.WaterBudgetSlotCWaterMeasurementTypeID = requestDto.WaterBudgetSlotCWaterMeasurementTypeID;

        dbContext.SaveChanges();
    }

    public static List<ErrorMessage> ValidateUpdateGeographyWaterManagers(QanatDbContext dbContext, int geographyID, List<UserDto> users)
    {
        var errors = new List<ErrorMessage>();

        var existingNonManagers = dbContext.GeographyUsers.Where(x => x.GeographyRoleID != (int)GeographyRoleEnum.WaterManager && x.GeographyID == geographyID).ToList();
        var usersAssignedToSpecificWaterAccounts = users.Where(x => existingNonManagers.Select(y => y.UserID).Contains(x.UserID)).ToList();

        foreach (var usersAssignedToSpecificWaterAccount in usersAssignedToSpecificWaterAccounts)
        {
            errors.Add(new ErrorMessage() { Type = "User", Message = $"{usersAssignedToSpecificWaterAccount.FullName} can't be both a Water Manager and be assigned to specific Water Accounts." });
        }

        return errors;
    }

    public static GeographyDto UpdateGeographyWaterManagers(QanatDbContext dbContext, int geographyID, List<UserDto> users)
    {
        var waterManagerRole = (int)GeographyRoleEnum.WaterManager;

        var existingWaterManagersForGeography = dbContext.GeographyUsers.Where(x =>
            x.GeographyRoleID == waterManagerRole && x.GeographyID == geographyID).ToList();

        var usersToAdd = users.Where(x => !existingWaterManagersForGeography.Select(y => y.UserID).Contains(x.UserID));

        var usersToRemove =
            existingWaterManagersForGeography.Where(x => !users.Select(y => y.UserID).Contains(x.UserID));

        dbContext.GeographyUsers.RemoveRange(usersToRemove);
        dbContext.GeographyUsers.AddRange(usersToAdd.Select(x => new GeographyUser()
        {
            GeographyID = geographyID,
            UserID = x.UserID,
            GeographyRoleID = waterManagerRole
        }));

        dbContext.SaveChanges();

        return GetByIDAsDto(dbContext, geographyID);
    }

    public static void UpdateIsOpenETConfiguration(QanatDbContext dbContext, int geographyID, OpenETConfigurationDto openETConfigurationDto)
    {
        var geography = dbContext.Geographies.Single(x => x.GeographyID == geographyID);

        geography.IsOpenETActive = openETConfigurationDto.IsOpenETActive;
        geography.OpenETShapeFilePath = openETConfigurationDto.OpenETShapefilePath;

        dbContext.SaveChanges();
    }
}