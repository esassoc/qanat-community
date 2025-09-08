//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellStatus]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellStatusExtensionMethods
    {
        public static WellStatusSimpleDto AsSimpleDto(this WellStatus wellStatus)
        {
            var dto = new WellStatusSimpleDto()
            {
                WellStatusID = wellStatus.WellStatusID,
                WellStatusName = wellStatus.WellStatusName,
                WellStatusDisplayName = wellStatus.WellStatusDisplayName
            };
            return dto;
        }
    }
}