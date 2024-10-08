//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationWaterUse]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellRegistrationWaterUseExtensionMethods
    {
        public static WellRegistrationWaterUseSimpleDto AsSimpleDto(this WellRegistrationWaterUse wellRegistrationWaterUse)
        {
            var dto = new WellRegistrationWaterUseSimpleDto()
            {
                WellRegistrationWaterUseID = wellRegistrationWaterUse.WellRegistrationWaterUseID,
                WellRegistrationID = wellRegistrationWaterUse.WellRegistrationID,
                WellRegistrationWaterUseTypeID = wellRegistrationWaterUse.WellRegistrationWaterUseTypeID,
                WellRegistrationWaterUseDescription = wellRegistrationWaterUse.WellRegistrationWaterUseDescription
            };
            return dto;
        }
    }
}