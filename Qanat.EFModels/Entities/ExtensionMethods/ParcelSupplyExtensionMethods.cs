using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ParcelSupplyExtensionMethods
{
    public static ParcelSupplyDetailDto AsDetailDto(this ParcelSupply parcelSupply)
    {
        return new ParcelSupplyDetailDto()
        {
            ParcelSupplyID = parcelSupply.ParcelSupplyID,
            GeographyID = parcelSupply.GeographyID,
            ParcelID = parcelSupply.ParcelID,
            TransactionDate = parcelSupply.TransactionDate,
            EffectiveDate = parcelSupply.EffectiveDate,
            TransactionAmount = parcelSupply.TransactionAmount,
            WaterTypeID = parcelSupply.WaterTypeID,
            UserID = parcelSupply.UserID,
            UserComment = parcelSupply.UserComment,
            UploadedFileName = parcelSupply.UploadedFileName,
            Parcel = parcelSupply.Parcel?.AsParcelMinimalDto(),
            WaterType = parcelSupply.WaterType?.AsSimpleDto(),
        };
    }
}