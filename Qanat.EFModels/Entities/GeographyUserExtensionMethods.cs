using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class GeographyUserExtensionMethods
{
    public static GeographyUserDto AsGeographyUserDto(this GeographyUser geographyUser)
    {
        var geographyUserDto = new GeographyUserDto()
        {
            GeographyUserID = geographyUser.GeographyUserID,
            Geography = geographyUser.Geography.AsSimpleDto(),
            User = geographyUser.User.AsUserDto(),
            GeographyRole = geographyUser.GeographyRole.AsSimpleDto()
        };
        return geographyUserDto;
    }
}