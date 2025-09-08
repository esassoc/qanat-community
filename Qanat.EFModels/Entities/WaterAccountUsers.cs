using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterAccountUsers
{
    public static List<WaterAccountUserMinimalDto> GetWaterAccountUsersForUserID(QanatDbContext dbContext, int userID, UserDto callingUser)
    {
        /*MK 2/13/25: If the user is checking their own water accounts, they should see all of them.
                      If they are checking someone else's water accounts, they should only see the water accounts where the calling user is a manager unless they are an admin. */

        var filterToGeographyIDs = Geographies.ListAsSimpleDto(dbContext).Select(x => x.GeographyID);
        if (userID != callingUser.UserID)
        {
            var callingUserIsAdmin = callingUser.Flags[Flag.IsSystemAdmin.FlagName];
            if (!callingUserIsAdmin)
            {
                var geographyIDsWhereCurrentUserIsManager = callingUser.GeographyFlags.Where(x => x.Value[Flag.HasManagerDashboard.FlagName]).Select(x => x.Key);
                filterToGeographyIDs = filterToGeographyIDs.Where(x => geographyIDsWhereCurrentUserIsManager.Contains(x)).ToList();
            }
        }

        var waterAccountUsers = dbContext.WaterAccountUsers
            .Include(x => x.User)
            .Include(x => x.WaterAccount).ThenInclude(x => x.Geography)
            .Where(x => x.UserID == userID && filterToGeographyIDs.Contains(x.WaterAccount.GeographyID));

        return waterAccountUsers.Select(x => x.AsWaterAccountUserMinimalDto()).ToList();
    }

    public static WaterAccountUserMinimalDto GetWaterAccountUserForUserIDAndWaterAccountID(QanatDbContext dbContext, int userID, int waterAccountID)
    {
        var waterAccountUser = dbContext.WaterAccountUsers
            .Include(x => x.User)
            .Include(x => x.WaterAccount).ThenInclude(x => x.Geography)
            .SingleOrDefault(x => x.UserID == userID && x.WaterAccountID == waterAccountID);

        return waterAccountUser?.AsWaterAccountUserMinimalDto();
    }

    public static List<ErrorMessage> ValidateClaimWaterAccounts(QanatDbContext dbContext, int userID, List<OnboardingWaterAccountDto> onboardingWaterAccountDtos)
    {
        var results = new List<ErrorMessage>();

        // just in case, ensuring a WaterAccountUserStaging record currently exists for each geography account to be claimed
        var geographyAccountIDs = onboardingWaterAccountDtos.Select(x => x.WaterAccountID).ToList();
        var geographyAccountIDsFromWaterAccountUserStagings = dbContext.WaterAccountUserStagings.AsNoTracking()
            .Where(x => geographyAccountIDs.Contains(x.WaterAccountID) && x.UserID == userID)
            .Select(x => x.WaterAccountID).ToList();

        foreach (var dto in onboardingWaterAccountDtos)
        {
            if (dto.IsClaimed.HasValue && dto.IsClaimed.Value && !geographyAccountIDsFromWaterAccountUserStagings.Contains(dto.WaterAccountID))
            {
                results.Add(new ErrorMessage()
                {
                    Type = "Claim Water Account",
                    Message = $"You do not have currently permission to claim geography account {dto.WaterAccountName}. Please ensure the correct geography account PIN have been entered."
                });
            }
        }

        return results;
    }

    public static void ClaimWaterAccounts(QanatDbContext dbContext, int userID, List<OnboardingWaterAccountDto> onboardingWaterAccountDtos)
    {
        foreach (var dto in onboardingWaterAccountDtos)
        {
            if (dto.IsClaimed.HasValue && dto.IsClaimed.Value)
            {
                // add to geography account user table as owner
                var waterAccountUser = dbContext.WaterAccountUsers
                    .SingleOrDefault(x => x.UserID == userID && x.WaterAccountID == dto.WaterAccountID);
                if (waterAccountUser == null)
                {
                    dbContext.WaterAccountUsers.Add(new WaterAccountUser()
                    {
                        UserID = userID,
                        WaterAccountID = dto.WaterAccountID,
                        WaterAccountRoleID = (int)WaterAccountRoleEnum.WaterAccountHolder, // when claiming, users become an owner 
                        ClaimDate = DateTime.UtcNow
                    });
                }
            }
        }

        // add to geography role of normal for geography that geography account is in
        var geographyIDsToAssign = dbContext.WaterAccounts.AsNoTracking().Where(x => onboardingWaterAccountDtos.Select(y =>  y.WaterAccountID).Contains(x.WaterAccountID)).Select(x => x.GeographyID).ToList();
        foreach (var geographyID in geographyIDsToAssign)
        {
            GeographyUsers.AddGeographyNormalUserIfAbsent(dbContext, userID, geographyID);
        }

        var waterAccountUserStagings = dbContext.WaterAccountUserStagings.Where(x => x.UserID == userID);
        dbContext.RemoveRange(waterAccountUserStagings);
        dbContext.SaveChanges();
    }

    public static List<ErrorMessage> ValidateAddUserData(QanatDbContext dbContext, AddUserByEmailDto addUserByEmailDto)
    {
        var results = new List<ErrorMessage>();
        if (string.IsNullOrEmpty(addUserByEmailDto.Email))
        {
            results.Add(new ErrorMessage()
            {
                Type = "Add user",
                Message = "Please enter a email."
            });
        }

        if (addUserByEmailDto.WaterAccountRoleID == 0)
        {
            results.Add(new ErrorMessage()
            {
                Type = "Add user",
                Message = "Please choose a role."
            });
        }

        return results;
    }

    public static List<ErrorMessage> ValidateEmail(string email)
    {
        var results = new List<ErrorMessage>();

        if (!string.IsNullOrEmpty(email) && !email.Contains('@'))
        {
            results.Add( new ErrorMessage()
            {
                Type = "Email",
                Message = "Please enter a valid email."
            });
        }

        return results;
    }

    public static void AddUserToWaterAccount(QanatDbContext dbContext, int geographyAccountID, AddUserByEmailDto addUserByEmailDto, User user)
    {
        dbContext.WaterAccountUsers.Add(new WaterAccountUser()
        {
            WaterAccountID = geographyAccountID,
            WaterAccountRoleID = addUserByEmailDto.WaterAccountRoleID,
            UserID = user.UserID,
            ClaimDate = DateTime.UtcNow
        });
        dbContext.SaveChanges();
    }

    public static async Task<List<ErrorMessage>> ValidateAddUserAsync(QanatDbContext dbContext, int waterAccountID, WaterAccountUserMinimalDto user)
    {
        var errorMessages = new List<ErrorMessage>();

        var existingWaterAccountUser = await dbContext.WaterAccountUsers.FirstOrDefaultAsync(x => x.WaterAccountID == waterAccountID && x.UserID == user.UserID);
        if (existingWaterAccountUser != null)
        {
            errorMessages.Add(new ErrorMessage()
            {
                Type = "User",
                Message = "User already is associated to the Water Account."
            });
        }

        var waterAccountRole = WaterAccountRole.All.FirstOrDefault(x => x.WaterAccountRoleID == user.WaterAccountRoleID);
        if (waterAccountRole == null)
        {
            errorMessages.Add(new ErrorMessage()
            {
                Type = "Water Account Role",
                Message = $"Could not find a Water Account Role with the ID {user.WaterAccountRoleID}."
            });
        }

        return errorMessages;
    }

    public static async Task<WaterAccountUserMinimalDto> AddUserAsync(QanatDbContext dbContext, int waterAccountID, WaterAccountUserMinimalDto user)
    {
        var newWaterAccountUser = new WaterAccountUser()
        {
            WaterAccountID = waterAccountID,
            WaterAccountRoleID = user.WaterAccountRoleID,
            UserID = user.UserID,
            ClaimDate = DateTime.UtcNow
        };

        await dbContext.WaterAccountUsers.AddAsync(newWaterAccountUser);
        await dbContext.SaveChangesAsync();

        var addedWaterAccountUser = GetWaterAccountUserForUserIDAndWaterAccountID(dbContext, user.UserID, waterAccountID);
        return addedWaterAccountUser;
    }

    public static void RemoveUserFromWaterAccount(QanatDbContext dbContext, int waterAccountUserID)
    {
        var user = dbContext.WaterAccountUsers.Single(x => x.WaterAccountUserID == waterAccountUserID);
        dbContext.WaterAccountUsers.Remove(user);
        dbContext.SaveChanges();
    }

    public static void UpdatePendingDate(QanatDbContext dbContext, string email, int geographyAccountID)
    {
        var pendingUser = dbContext.Users.First(x => x.Email == email && x.RoleID == (int)RoleEnum.PendingLogin);
        pendingUser.CreateDate = DateTime.UtcNow;
        dbContext.SaveChanges();
    }
}