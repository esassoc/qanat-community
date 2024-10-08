//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationWaterUseType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellRegistrationWaterUseTypeExtensionMethods
    {
        public static WellRegistrationWaterUseTypeSimpleDto AsSimpleDto(this WellRegistrationWaterUseType wellRegistrationWaterUseType)
        {
            var dto = new WellRegistrationWaterUseTypeSimpleDto()
            {
                WellRegistrationWaterUseTypeID = wellRegistrationWaterUseType.WellRegistrationWaterUseTypeID,
                WellRegistrationWaterUseTypeName = wellRegistrationWaterUseType.WellRegistrationWaterUseTypeName,
                WellRegistrationWaterUseTypeDisplayName = wellRegistrationWaterUseType.WellRegistrationWaterUseTypeDisplayName
            };
            return dto;
        }
    }
}