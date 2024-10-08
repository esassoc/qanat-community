//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellIrrigatedParcel]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WellIrrigatedParcelExtensionMethods
    {
        public static WellIrrigatedParcelSimpleDto AsSimpleDto(this WellIrrigatedParcel wellIrrigatedParcel)
        {
            var dto = new WellIrrigatedParcelSimpleDto()
            {
                WellIrrigatedParcelID = wellIrrigatedParcel.WellIrrigatedParcelID,
                WellID = wellIrrigatedParcel.WellID,
                ParcelID = wellIrrigatedParcel.ParcelID
            };
            return dto;
        }
    }
}