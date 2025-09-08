using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Models.ExtensionMethods;

public static class UserDtoExtensionMethods
{
    public static bool IsAdminOrWaterManager(this UserDto user, int geographyID)
    {
        user.Flags.TryGetValue(Flag.IsSystemAdmin.FlagName, out var isSystemAdmin);

        var isWaterManager = false;
        user.GeographyFlags.TryGetValue(geographyID, out var geographyFlags);
        geographyFlags?.TryGetValue(Flag.HasManagerDashboard.FlagName, out isWaterManager);

        return isSystemAdmin || isWaterManager;
    }
}