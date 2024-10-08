using Qanat.Models.DataTransferObjects;
using System.Linq;

namespace Qanat.EFModels.Entities;

public static partial class WaterAccountParcelExtensionMethods
{
    public static WaterAccountParcelDto AsWaterAccountParcelDto(
        this WaterAccountParcel waterAccountParcel)
    {
        var dto = new WaterAccountParcelDto()
        {
            WaterAccountParcelID = waterAccountParcel.WaterAccountParcelID,
            GeographyID = waterAccountParcel.GeographyID,
            WaterAccountID = waterAccountParcel.WaterAccountID,
            ParcelID = waterAccountParcel.ParcelID,
            EffectiveYear = waterAccountParcel.EffectiveYear,
            EndYear = waterAccountParcel.EndYear,
            WaterAccount = waterAccountParcel.WaterAccount?.AsWaterAccountMinimalDto()
        };
        return dto;
    }
}