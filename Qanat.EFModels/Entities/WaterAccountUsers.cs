using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;
using System.Net.Mail;

namespace Qanat.EFModels.Entities;

public static class WaterAccountUsers
{
    public static List<WaterAccountUserMinimalDto> GetWaterAccountUsersForUserID(QanatDbContext dbContext, int userID)
    {
        var geographyAccountUsers = dbContext.WaterAccountUsers
            .Include(x => x.User)
            .Include(x => x.WaterAccount).ThenInclude(x => x.Geography)
            .Where(x => x.UserID == userID);

        return geographyAccountUsers.Select(x => x.AsWaterAccountUserMinimalDto()).ToList();
    }

    public static WaterAccountUserMinimalDto GetWaterAccountUserForUserIDAndWaterAccountID(QanatDbContext dbContext, int callingUserUserID, int waterAccountID)
    {
        var waterAccountUser = dbContext.WaterAccountUsers
            .Include(x => x.User)
            .Include(x => x.WaterAccount).ThenInclude(x => x.Geography)
            .SingleOrDefault(x => x.UserID == callingUserUserID && x.WaterAccountID == waterAccountID);

        return waterAccountUser?.AsWaterAccountUserMinimalDto();
    }

    public static List<WaterAccountUserMinimalDto> UpdateUserWaterAccounts(QanatDbContext dbContext, int userID, List<WaterAccountUserMinimalDto> geographyAccountUserMinimalDtos)
    {
        var newAccountUsers = geographyAccountUserMinimalDtos.Select(dto => new WaterAccountUser()
        {
            WaterAccountID = dto.WaterAccount.WaterAccountID,
            UserID = dto.UserID,
            ClaimDate = DateTime.UtcNow,
            WaterAccountRoleID = dto.WaterAccountRole.WaterAccountRoleID
        }).ToList();

        var existingWaterAccountUsers = dbContext.WaterAccountUsers.Where(x => x.UserID == userID).ToList();

        var allInDatabase = dbContext.WaterAccountUsers;
        existingWaterAccountUsers.Merge(newAccountUsers, allInDatabase, (x, y) => x.WaterAccountID == y.WaterAccountID && x.UserID == y.UserID,
            (existing, updated) =>
            {
                existing.WaterAccountRoleID = updated.WaterAccountRoleID;
            });
        dbContext.SaveChanges();

        foreach (var geographyAccountUserMinimalDto in geographyAccountUserMinimalDtos)
        {
            GeographyUsers.AddGeographyNormalUserIfAbsent(dbContext, userID, geographyAccountUserMinimalDto.WaterAccount.GeographyID);
        }

        return GetWaterAccountUsersForUserID(dbContext, userID);
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

    public static MailMessage AddUserToWaterAccount(QanatDbContext dbContext, int geographyAccountID, AddUserByEmailDto addUserByEmailDto, User user, User invitingUser)
    {
        var geographyAccount = dbContext.WaterAccounts.Single(x => x.WaterAccountID == geographyAccountID);
        dbContext.WaterAccountUsers.Add(new WaterAccountUser()
        {
            WaterAccountID = geographyAccountID,
            WaterAccountRoleID = addUserByEmailDto.WaterAccountRoleID,
            UserID = user.UserID,
            ClaimDate = DateTime.UtcNow
        });
        dbContext.SaveChanges();

        var mailMessage = new MailMessage
        {
            Subject = $"You’ve been added to a new Water Account in the Ground Water Accounting Platform",
            Body = $"Hello {user.FullName},<br /><br />" +
                   $"{invitingUser.FullName} has added you to their {geographyAccount.WaterAccountName} Water Account in the Groundwater Accounting Platform.<br /><br />" +
                   $"You can now view this Water Account from your dashboard on the platform:<br />" +
                   $"https://groundwateraccounting.org/water-dashboard<br /><br />" +
                   $"If you think you have been added to this Water Account in error, please contact {invitingUser.FullName} at {invitingUser.Email}.<br /><br />" +
                   $"Thank you,<br />" +
                   $"The Groundwater Accounting Platform Team",
            IsBodyHtml = true
        };

        mailMessage.To.Add(new MailAddress(addUserByEmailDto.Email));
        return mailMessage;
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