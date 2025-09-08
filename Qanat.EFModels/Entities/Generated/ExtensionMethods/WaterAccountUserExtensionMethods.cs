//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountUser]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterAccountUserExtensionMethods
    {
        public static WaterAccountUserSimpleDto AsSimpleDto(this WaterAccountUser waterAccountUser)
        {
            var dto = new WaterAccountUserSimpleDto()
            {
                WaterAccountUserID = waterAccountUser.WaterAccountUserID,
                UserID = waterAccountUser.UserID,
                WaterAccountID = waterAccountUser.WaterAccountID,
                WaterAccountRoleID = waterAccountUser.WaterAccountRoleID,
                ClaimDate = waterAccountUser.ClaimDate
            };
            return dto;
        }
    }
}