using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.Swagger.Entities;

public class UserPermissions
{
    /// <summary>
    /// Returns true if user role has IsSystemAdmin flag set to true
    /// </summary>
    /// <param name="callingUser"></param>
    public static bool UserIsSystemAdmin(UserDto callingUser)
    {
        callingUser.Flags.TryGetValue(Flag.IsSystemAdmin.FlagName, out var isSystemAdmin);
        return isSystemAdmin;
    }

    /// <summary>
    /// Returns a list of IDs for all geographies where user is a water manager
    /// </summary>
    /// <param name="callingUser"></param>
    public static List<int> ListManagedGeographyIDsByUser(UserDto callingUser)
    {
        var geographyIDsWhereUserIsManager = callingUser.GeographyFlags
            .Where(x => x.Value[Flag.HasManagerDashboard.FlagName])
            .Select(x => x.Key).ToList();

        return geographyIDsWhereUserIsManager;
    }

    // list associated water accounts
    /// <summary>
    /// Returns a list of IDs for all water accounts within the given geography where user is an account holder or viewer
    /// </summary>
    /// <param name="callingUser"></param>
    public static List<int> ListAssociatedWaterAccountsByGeographyIDAndUser(QanatDbContext dbContext, int geographyID, UserDto callingUser)
    {
        var waterAccountIDs = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountUsers)
            .Where(x => x.GeographyID == geographyID && x.WaterAccountUsers.Any(y => y.UserID == callingUser.UserID))
            .Select(x => x.WaterAccountID)
            .ToList();

        return waterAccountIDs;
    }
}