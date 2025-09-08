using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.EFModels.Entities;

public static class GeographyUserExtensionMethods
{
    public static GeographyUserSimpleDto AsSimpleDto(this GeographyUser geographyUser)
    {
        var dto = new GeographyUserSimpleDto()
        {
            GeographyUserID = geographyUser.GeographyUserID,
            GeographyID = geographyUser.GeographyID,
            UserID = geographyUser.UserID,
            GeographyRoleID = geographyUser.GeographyRoleID,
            ReceivesNotifications = geographyUser.ReceivesNotifications
        };
        return dto;
    }

    public static GeographyUserDto AsGeographyUserDto(this GeographyUser geographyUser)
    {
        var geographyUserDto = new GeographyUserDto()
        {
            GeographyUserID = geographyUser.GeographyUserID,
            Geography = geographyUser.Geography.AsSimpleDto(),
            User = geographyUser.User.AsUserDto(),
            GeographyRole = geographyUser.GeographyRole.AsSimpleDto(),
            WaterAccounts = geographyUser.User?.WaterAccountUsers.Select(x => x.WaterAccount).Select(wa => wa.AsSimpleDto()).ToList(),
            WellRegistrationCount = geographyUser.User?.WellRegistrations.Count ?? 0
        };

        return geographyUserDto;
    }
}