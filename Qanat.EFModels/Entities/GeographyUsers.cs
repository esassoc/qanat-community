using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects.Geography;
using SendGrid.Helpers.Mail;

namespace Qanat.EFModels.Entities;

public static class GeographyUsers
{
    public static async Task<List<GeographyUserDto>> ListAsync(QanatDbContext dbContext, int geographyID)
    {
        var geographyUsers = await dbContext.GeographyUsers.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Geography)
            .Include(x => x.User.WaterAccountUsers).ThenInclude(x => x.WaterAccount)
            .Include(x => x.User.WellRegistrations)
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var geographyUserDtos = geographyUsers
            .Select(x => x.AsGeographyUserDto())
            .OrderBy(x => x.User.FullName)
            .ToList();

        return geographyUserDtos;
    }

    public static void AddGeographyNormalUserIfAbsent(QanatDbContext dbContext, int userID, int geographyID)
    {
        AddGeographyNormalUsersIfAbsent(dbContext, new List<int>(){ userID }, geographyID);
    }

    public static void AddGeographyNormalUsersIfAbsent(QanatDbContext dbContext, List<int> userIDs, int geographyID)
    {
        var geographyUserIDs = dbContext.GeographyUsers
            .Where(x => x.GeographyID == geographyID && userIDs.Contains(x.UserID))
            .Select(x => x.UserID).ToList();

        var newGeographyUsers = userIDs.Where(x => !geographyUserIDs.Contains(x))
            .Select(x => new GeographyUser()
            {
                UserID = x,
                GeographyID = geographyID,
                GeographyRoleID = (int)GeographyRoleEnum.Normal,
                ReceivesNotifications = false
            });

        dbContext.GeographyUsers.AddRange(newGeographyUsers);
        dbContext.SaveChanges();
    }

    public static List<EmailAddress> ListEmailAddressesForGeographyManagersWhoReceiveNotifications(QanatDbContext dbContext, int geographyID)
    {
        // getting all geography users who receive notifications
        var geographyUsers = dbContext.Users.Include(x => x.GeographyUsers).AsNoTracking()
            .Where(x => x.GeographyUsers.Any(y => y.GeographyID == geographyID && y.ReceivesNotifications))
            .Select(x => x.AsUserDto()).ToList();

        // ensuring all listed users are managers
        var hasManagerDashboardFlag = Flag.HasManagerDashboard.FlagName;
        var geographyManagers = geographyUsers
            .Where(x => x.GeographyFlags.ContainsKey(geographyID) &&
                        x.GeographyFlags[geographyID].ContainsKey(hasManagerDashboardFlag) &&
                        x.GeographyFlags[geographyID][hasManagerDashboardFlag]);

        var geographyManagerEmails = geographyManagers.Select(x => new EmailAddress(x.Email, x.FullName)).ToList();
        return geographyManagerEmails;
    }
}