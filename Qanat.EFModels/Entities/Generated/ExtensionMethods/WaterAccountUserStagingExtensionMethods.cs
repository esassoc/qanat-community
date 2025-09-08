//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountUserStaging]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterAccountUserStagingExtensionMethods
    {
        public static WaterAccountUserStagingSimpleDto AsSimpleDto(this WaterAccountUserStaging waterAccountUserStaging)
        {
            var dto = new WaterAccountUserStagingSimpleDto()
            {
                WaterAccountUserStagingID = waterAccountUserStaging.WaterAccountUserStagingID,
                UserID = waterAccountUserStaging.UserID,
                WaterAccountID = waterAccountUserStaging.WaterAccountID,
                ClaimDate = waterAccountUserStaging.ClaimDate
            };
            return dto;
        }
    }
}