//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountParcel]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterAccountParcelExtensionMethods
    {
        public static WaterAccountParcelSimpleDto AsSimpleDto(this WaterAccountParcel waterAccountParcel)
        {
            var dto = new WaterAccountParcelSimpleDto()
            {
                WaterAccountParcelID = waterAccountParcel.WaterAccountParcelID,
                GeographyID = waterAccountParcel.GeographyID,
                WaterAccountID = waterAccountParcel.WaterAccountID,
                ParcelID = waterAccountParcel.ParcelID,
                EffectiveYear = waterAccountParcel.EffectiveYear,
                EndYear = waterAccountParcel.EndYear
            };
            return dto;
        }
    }
}