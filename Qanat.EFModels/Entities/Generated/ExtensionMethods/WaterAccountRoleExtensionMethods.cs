//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountRole]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterAccountRoleExtensionMethods
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
    }
}