//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellRegistrationIrrigatedParcel]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellRegistrationIrrigatedParcelExtensionMethods
    {
        public static WellRegistrationIrrigatedParcelSimpleDto AsSimpleDto(this WellRegistrationIrrigatedParcel wellRegistrationIrrigatedParcel)
        {
            var dto = new WellRegistrationIrrigatedParcelSimpleDto()
            {
                WellRegistrationIrrigatedParcelID = wellRegistrationIrrigatedParcel.WellRegistrationIrrigatedParcelID,
                WellRegistrationID = wellRegistrationIrrigatedParcel.WellRegistrationID,
                ParcelID = wellRegistrationIrrigatedParcel.ParcelID
            };
            return dto;
        }
    }
}