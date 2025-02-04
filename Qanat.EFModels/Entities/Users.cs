using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.User;
using Qanat.Models.Helpers;
using System.Security.Claims;

namespace Qanat.EFModels.Entities;

public static class Users
{
    //MK 9/5/2024 -- This is the user ID for the system admin user. Not a huge fan of hardcoding this, but need to move on for now.
    public static int QanatSystemAdminUserID = 7;

    public static IEnumerable<UserDto> List(QanatDbContext dbContext)
    {
        return GetUserImpl(dbContext).Where(x => x.RoleID != (int)RoleEnum.PendingLogin).AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => x.AsUserDto()).AsEnumerable();
    }

    public static IEnumerable<UserDto> ListByRole(QanatDbContext dbContext, RoleEnum roleEnum)
    {
        var users = GetUserImpl(dbContext).Where(x => x.IsActive && x.RoleID == (int)roleEnum)
            .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
            .Select(x => x.AsUserDto())
            .AsEnumerable();

        return users;
    }

    public static IEnumerable<UserDto> ListByRole(QanatDbContext dbContext, List<int> roles)
    {
        var users = GetUserImpl(dbContext).Where(x => roles.Contains(x.RoleID))
            .Select(x => x.AsUserDto())
            .AsEnumerable();

        return users;
    }

    public static IEnumerable<string> GetEmailAddressesForAdminsThatReceiveSupportEmails(QanatDbContext dbContext)
    {
        var users = GetUserImpl(dbContext).Where(x => x.IsActive && x.RoleID == (int)RoleEnum.SystemAdmin && x.ReceiveSupportEmails)
            .Select(x => x.Email)
            .AsEnumerable();

        return users;
    }

    public static UserDto GetByUserID(QanatDbContext dbContext, int userID)
    {
        var user = GetUserImpl(dbContext).SingleOrDefault(x => x.UserID == userID);
        return user?.AsUserDto();
    }

    public static List<UserDto> GetByUserID(QanatDbContext dbContext, List<int> userIDs)
    {
        return GetUserImpl(dbContext).Where(x => userIDs.Contains(x.UserID)).Select(x => x.AsUserDto()).ToList();
    }

    public static UserDto GetByUserGuid(QanatDbContext dbContext, Guid userGuid)
    {
        var user = GetUserImpl(dbContext).SingleOrDefault(x => x.UserGuid == userGuid);

        return user?.AsUserDto();
    }

    private static IQueryable<User> GetUserImpl(QanatDbContext dbContext)
    {
        return dbContext.Users
            .Include(x => x.GeographyUsers)
                .ThenInclude(x => x.Geography)
            .Include(x => x.WaterAccountUsers)
            .Include(x => x.ModelUsers)
            .AsNoTracking();
    }

    public static UserDto GetByEmail(QanatDbContext dbContext, string email)
    {
        var user = GetUserImpl(dbContext).SingleOrDefault(x => x.Email == email);
        return user?.AsUserDto();
    }

    public static List<ErrorMessage> ValidateUpdate(QanatDbContext dbContext, UserUpsertDto userUpsertDto, int userID)
    {
        var result = new List<ErrorMessage>();
        if (!userUpsertDto.RoleID.HasValue)
        {
            result.Add(new ErrorMessage() { Type = "Role ID", Message = "Role ID is required." });
        }

        var role = Role.AllLookupDictionary[userUpsertDto.RoleID.GetValueOrDefault()];
        if (role == null)
        {
            result.Add(new ErrorMessage() { Type = "Role ID", Message = $"A Role with the ID of {userUpsertDto.RoleID} could not be found." });
        }

        if (!userUpsertDto.ScenarioPlannerRoleID.HasValue)
        {
            result.Add(new ErrorMessage() { Type = "Scenario Planner Role ID", Message = "Scenario Planner Role ID is required." });
        }

        var scenarioPlannerRole = ScenarioPlannerRole.AllLookupDictionary[userUpsertDto.ScenarioPlannerRoleID.GetValueOrDefault()];
        if (scenarioPlannerRole == null)
        {
            result.Add(new ErrorMessage() { Type = "Scenario Planner Role ID", Message = $"A Scenario Planner Role with the ID of {userUpsertDto.ScenarioPlannerRoleID} could not be found." });
            return result;
        }

        if (userUpsertDto.ScenarioPlannerRoleID == ScenarioPlannerRole.NoAccess.ScenarioPlannerRoleID && userUpsertDto.ModelIDs.Any())
        {
            result.Add(new ErrorMessage() { Type = "Models", Message = "Models cannot be assigned to a user with no access to the Scenario Planner." });
        }

        foreach (var modelID in userUpsertDto.ModelIDs)
        {
            var model = Model.All.FirstOrDefault(x => x.ModelID == modelID);
            if (model == null)
            {
                result.Add(new ErrorMessage(){ Type="Model ID", Message = $"A Model with the ID of {modelID} could not be found."});
            }
        }

        return result;
    }

    public static async Task<UserDto> UpdateUserEntity(QanatDbContext dbContext, int userID, UserUpsertDto userEditDto)
    {
        if (!userEditDto.RoleID.HasValue)
        {
            return null;
        }

        var user = dbContext.Users
            .Single(x => x.UserID == userID);

        user.RoleID = userEditDto.RoleID.Value;
        user.IsActive = userEditDto.RoleID.HasValue && userEditDto.RoleID != (int)RoleEnum.NoAccess;
        user.ReceiveSupportEmails = userEditDto.RoleID.GetValueOrDefault(0) == Role.SystemAdmin.PrimaryKey && userEditDto.ReceiveSupportEmails;

        user.ScenarioPlannerRoleID = userEditDto.ScenarioPlannerRoleID ?? ScenarioPlannerRole.NoAccess.ScenarioPlannerRoleID;
        user.GETRunCustomerID = userEditDto.GETRunCustomerID;
        user.GETRunUserID = userEditDto.GETRunUserID;
        user.UpdateDate = DateTime.UtcNow;

        //Clear out userIDs if roleID is no access to remove access from models.
        if (user.ScenarioPlannerRoleID == ScenarioPlannerRole.NoAccess.ScenarioPlannerRoleID)
        {
            userEditDto.ModelIDs.Clear();
        }

        await UpdateModelUsersForUser(dbContext, userID, userEditDto.ModelIDs);

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(user).ReloadAsync();
        return GetByUserID(dbContext, userID);
    }

    private static async Task UpdateModelUsersForUser(QanatDbContext dbContext, int userID, List<int> modelIDs)
    {
        var existingModelUsers = dbContext.ModelUsers.Where(x => x.UserID == userID).ToList();
        var modelUsersToAdd = modelIDs.Except(existingModelUsers.Select(x => x.ModelID)).Select(x => new ModelUser()
        {
            ModelID = x,
            UserID = userID
        }).ToList();

        await dbContext.ModelUsers.AddRangeAsync(modelUsersToAdd);

        var modelUsersToRemove = existingModelUsers.Where(x => !modelIDs.Contains(x.ModelID)).ToList();
        dbContext.ModelUsers.RemoveRange(modelUsersToRemove);
    }

    public static UserDto UpdateClaims(QanatDbContext dbContext, int? userID, ClaimsPrincipal claims)
    {
        User user;
        var email = claims?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.Emails)?.Value;
        var sub = claims?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.Sub)?.Value;
        var userGuid = Guid.Parse(sub);

        if (userID.HasValue)
        {
            user = dbContext.Users.SingleOrDefault(x => x.UserID == userID);
        }
        else
        {
            user = dbContext.Users.SingleOrDefault(x => x.Email == email);
        }

        if (user == null)
        {
            user = new User
            {
                UserGuid = userGuid,
                IsActive = true,
                RoleID = Role.Normal.RoleID,
                CreateDate = DateTime.UtcNow,
                ReceiveSupportEmails = false
            };

            dbContext.Users.Add(user);
        }

        var firstName = claims?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.GivenName)?.Value;
        var lastName = claims?.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.FamilyName)?.Value;

        if (!string.IsNullOrEmpty(sub))
        {
            user.UserGuid = userGuid;
        }

        if (!string.IsNullOrEmpty(firstName))
        {
            user.FirstName = firstName;
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            user.LastName = lastName;
        }

        if (!string.IsNullOrEmpty(email))
        {
            user.Email = email;
        }

        if (user.RoleID == (int)RoleEnum.PendingLogin)
        {
            user.RoleID = Role.Normal.RoleID;
        }

        dbContext.SaveChanges();
        dbContext.Entry(user).Reload();

        return GetByUserID(dbContext, user.UserID);
    }

    public static bool ValidateAllExist(QanatDbContext dbContext, List<WaterAccountUserMinimalDto> waterAccountUsers)
    {
        var userIDList = waterAccountUsers.Select(x => x.User.UserID);
        return dbContext.Users.Count(x => userIDList.Contains(x.UserID)) == waterAccountUsers.Distinct().Count();
    }

    public static int GetNumberOfWellRegistrationsForUser(QanatDbContext dbContext, int userID, int geographyID)
    {
        var wellRegistrations = dbContext.WellRegistrations.AsNoTracking()
            .Include(x => x.Parcel).ThenInclude(x => x.ParcelGeometry)
            .Include(x => x.Geography)
            .Where(x => x.CreateUserID == userID && x.GeographyID == geographyID);

        return wellRegistrations.Any() 
            ? wellRegistrations.Count() 
            : 0;
    }

    public static int GetNumberOfWaterAccountForUser(QanatDbContext dbContext, int userID, int geographyID)
    {
        var listOfWaterAccountIDs = dbContext.fWaterAccountUser(userID).Select(x => x.WaterAccountID).ToList();
        var waterAccountsForGeography = dbContext.WaterAccounts.Where(x => x.GeographyID == geographyID)
            .Where(x => listOfWaterAccountIDs.Contains(x.WaterAccountID)).ToList();

        return waterAccountsForGeography.Count;
    }

    public static List<UserGeographySummaryDto> ListUserGeographySummariesByUserID(QanatDbContext dbContext, int userID)
    {
        var waterAccounts = dbContext.WaterAccounts
            .Include(x => x.Geography).ThenInclude(x => x.GeographyConfiguration)
            .Include(x => x.Parcels).ThenInclude(x => x.ParcelZones)
            .Include(x => x.Parcels).ThenInclude(x => x.Wells)
            .AsNoTracking()
            .Where(x => x.WaterAccountUsers.Any(y => y.UserID == userID))
            .ToList();

        var geographyAllocationPlanZones = dbContext.GeographyAllocationPlanConfigurations.AsNoTracking()
            .Include(x => x.ZoneGroup)
            .ThenInclude(x => x.Zones)
            .Select(x => new
            {
                GeographyID = x.GeographyID,
                ZoneIDs = x.ZoneGroup.Zones.Select(y => y.ZoneID)
            })
            .ToDictionary(x => x.GeographyID);

        return waterAccounts.GroupBy(x => x.GeographyID)
            .Select(x =>
            {
                var parcels = x.SelectMany(y => y.Parcels).ToList();
                var zonesDict = Zones.ListAsDisplayDto(dbContext).ToDictionary(y => y.ZoneID);
                var geographyAllocationZoneIDs = geographyAllocationPlanZones.ContainsKey(x.Key)
                    ? geographyAllocationPlanZones[x.Key].ZoneIDs
                    : new List<int>();
                return new UserGeographySummaryDto()
                {
                    GeographyID = x.Key,
                    GeographyName = x.First().Geography.GeographyName,
                    GeographyDisplayName = x.First().Geography.GeographyDisplayName,
                    WaterAccounts = x.Select(waterAccount => new WaterAccountSummaryDto()
                    {
                        WaterAccountID = waterAccount.WaterAccountID,
                        WaterAccountNumber = waterAccount.WaterAccountNumber,
                        GeographyName = x.First().Geography.GeographyName,
                        Zones = WaterAccounts.GetZonesForParcel(waterAccount, zonesDict, geographyAllocationZoneIDs),
                        NumOfParcels = waterAccount.Parcels.Count(),
                        Area = waterAccount.Parcels.Sum(x => x.ParcelArea)
                    }).OrderByDescending(y => y.Area).ToList(),
                    WellsCount = x.First().Geography.GeographyConfiguration.WellRegistryEnabled ?
                        parcels.SelectMany(y => y.Wells).Count() : null,
                };
            }).ToList();
    }
}