using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterAccountParcelExtensionMethods
{
    public static WaterAccountParcelSimpleDto AsSimpleDto(this WaterAccountParcel waterAccountParcel)
    {
        var dto = new WaterAccountParcelSimpleDto()
        {
            WaterAccountParcelID = waterAccountParcel.WaterAccountParcelID,
            GeographyID = waterAccountParcel.GeographyID,
            WaterAccountID = waterAccountParcel.WaterAccountID,
            ParcelID = waterAccountParcel.ParcelID,
            ReportingPeriodID = waterAccountParcel.ReportingPeriodID
        };
        return dto;
    }
}