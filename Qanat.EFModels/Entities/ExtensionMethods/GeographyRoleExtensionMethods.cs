using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.EFModels.Entities
{
    public static class GeographyRoleExtensionMethods
    {
        public static GeographyRoleSimpleDto AsSimpleDto(this GeographyRole geographyRole)
        {
            var dto = new GeographyRoleSimpleDto()
            {
                GeographyRoleID = geographyRole.GeographyRoleID,
                GeographyRoleName = geographyRole.GeographyRoleName,
                GeographyRoleDisplayName = geographyRole.GeographyRoleDisplayName,
                GeographyRoleDescription = geographyRole.GeographyRoleDescription,
                SortOrder = geographyRole.SortOrder,
                Rights = geographyRole.Rights,
                Flags = geographyRole.Flags
            };
            return dto;
        }

        public static Dictionary<string, Rights> AsGeographyRights(this GeographyRole geographyRole)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Rights>>(geographyRole.Rights, RightsSerializerOptions.JsonSerializerOptions);
        }
        public static Dictionary<string, bool> AsGeographyFlags(this GeographyRole geographyRole)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(geographyRole.Flags);
        }
    }
}