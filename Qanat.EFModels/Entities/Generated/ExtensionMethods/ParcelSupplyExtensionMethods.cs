//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelSupply]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ParcelSupplyExtensionMethods
    {
        public static ParcelSupplySimpleDto AsSimpleDto(this ParcelSupply parcelSupply)
        {
            var dto = new ParcelSupplySimpleDto()
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
                UploadedFileName = parcelSupply.UploadedFileName
            };
            return dto;
        }
    }
}