using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class Geographies
{
    private static IIncludableQueryable<Geography, User> GetImpl(QanatDbContext dbContext)
    {
        return dbContext.Geographies.AsNoTracking()
            .Include(x => x.SourceOfRecordWaterMeasurementType)
            .Include(x => x.GeographyConfiguration)
            .Include(x => x.GeographyBoundary)
            .Include(x => x.GeographyAllocationPlanConfiguration)
            .Include(x => x.GeographyUsers)
            .ThenInclude(x => x.User);
    }

    public static List<GeographyDto> ListAsDto(QanatDbContext dbContext)
    {
        return GetImpl(dbContext)
            .OrderBy(x => x.GeographyName)
            .Select(x => x.AsGeographyDto()).ToList();
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
        return GetImpl(dbContext).SingleOrDefault(x => x.GeographyID == geographyID)?.AsGeographyDto();
    }

    public static Geography GetByID(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Geographies.AsNoTracking().SingleOrDefault(x => x.GeographyID == geographyID);
    }

    public static GeographyDto GetByNameAsDto(QanatDbContext dbContext, string geographyName)
    {
        return GetImpl(dbContext).AsNoTracking().SingleOrDefault(x => x.GeographyName.ToLower() == geographyName)?.AsGeographyDto();
    }

    public static WaterMeasurementType GetSourceOfRecordWaterMeasurementTypeByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Geographies.Include(x => x.SourceOfRecordWaterMeasurementType).AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID)?.SourceOfRecordWaterMeasurementType;
    }

    public static AdminGeographyUpdateRequestDto GetAsAdminGeographyUpdateRequestDto(QanatDbContext dbContext, int geographyID)
    {
        var geography = dbContext.Geographies.Single(x => x.GeographyID == geographyID);

        return new AdminGeographyUpdateRequestDto()
        {
            GeographyID = geographyID,
            GeographyDisplayName = geography.GeographyDisplayName,
            StartYear = geography.StartYear,
            DefaultDisplayYear = geography.DefaultDisplayYear,
            APNRegexPattern = geography.APNRegexPattern,
            APNRegexDisplay = geography.APNRegexPatternDisplay,
            LandownerDashboardSupplyLabel = geography.LandownerDashboardSupplyLabel,
            LandownerDashboardUsageLabel = geography.LandownerDashboardUsageLabel,
            ContactEmail = geography.ContactEmail,
            ContactPhoneNumber = geography.ContactPhoneNumber,
            DisplayUsageGeometriesAsField = geography.DisplayUsageGeometriesAsField,
            AllowLandownersToRequestAccountChanges = geography.AllowLandownersToRequestAccountChanges
        };
    }

    public static List<ErrorMessage> ValidateGeographyUpdate(AdminGeographyUpdateRequestDto requestDto)
    {
        var errors = new List<ErrorMessage>();

        var currentYear = DateTime.Now.Year;
        if (requestDto.StartYear > currentYear)
        {
            errors.Add(new ErrorMessage() { Type = "Start Year", Message = "Start Year cannot be later than the current year." });
        }
        if (requestDto.DefaultDisplayYear > currentYear)
        {
            errors.Add(new ErrorMessage() { Type = "Default Display Year", Message = "Default Display Year cannot be later than the current year." });
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

        return errors;
    }

    public static void UpdateBasicGeographyFields(QanatDbContext dbContext, int geographyID, AdminGeographyUpdateRequestDto requestDto)
    {
        var geography = dbContext.Geographies.Single(x => x.GeographyID == geographyID);
        
        geography.GeographyDisplayName = requestDto.GeographyDisplayName;
        geography.StartYear = requestDto.StartYear;
        geography.DefaultDisplayYear = requestDto.DefaultDisplayYear;
        geography.APNRegexPattern = requestDto.APNRegexPattern;
        geography.APNRegexPatternDisplay = requestDto.APNRegexDisplay;
        geography.LandownerDashboardSupplyLabel = requestDto.LandownerDashboardSupplyLabel;
        geography.LandownerDashboardUsageLabel = requestDto.LandownerDashboardUsageLabel;
        geography.ContactEmail = requestDto.ContactEmail;
        geography.ContactPhoneNumber = requestDto.ContactPhoneNumber;
        geography.DisplayUsageGeometriesAsField = requestDto.DisplayUsageGeometriesAsField;
        geography.AllowLandownersToRequestAccountChanges = requestDto.AllowLandownersToRequestAccountChanges;

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