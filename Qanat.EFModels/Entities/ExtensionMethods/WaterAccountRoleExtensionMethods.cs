using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.EFModels.Entities
{
    public static class WaterAccountRoleExtensionMethods
    {
        public static WaterAccountRoleSimpleDto AsSimpleDto(this WaterAccountRole waterAccountRole)
        {
            var dto = new WaterAccountRoleSimpleDto()
            {
                WaterAccountRoleID = waterAccountRole.WaterAccountRoleID,
                WaterAccountRoleName = waterAccountRole.WaterAccountRoleName,
                WaterAccountRoleDisplayName = waterAccountRole.WaterAccountRoleDisplayName,
                WaterAccountRoleDescription = waterAccountRole.WaterAccountRoleDescription,
                SortOrder = waterAccountRole.SortOrder,
                Rights = waterAccountRole.Rights,
                Flags = waterAccountRole.Flags
            };
            return dto;
        }

        public static Dictionary<string, Rights> AsWaterAccountRights(this WaterAccountRole waterAccountRole)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Rights>>(waterAccountRole.Rights, RightsSerializerOptions.JsonSerializerOptions);
        }

        public static Dictionary<string, bool> AsWaterAccountFlags(this WaterAccountRole waterAccountRole)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(waterAccountRole.Flags);
        }
    }
}