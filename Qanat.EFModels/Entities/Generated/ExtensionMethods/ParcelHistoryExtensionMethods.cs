//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelHistory]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ParcelHistoryExtensionMethods
    {
        public static ParcelHistorySimpleDto AsSimpleDto(this ParcelHistory parcelHistory)
        {
            var dto = new ParcelHistorySimpleDto()
            {
                ParcelHistoryID = parcelHistory.ParcelHistoryID,
                GeographyID = parcelHistory.GeographyID,
                ParcelID = parcelHistory.ParcelID,
                EffectiveYear = parcelHistory.EffectiveYear,
                UpdateDate = parcelHistory.UpdateDate,
                UpdateUserID = parcelHistory.UpdateUserID,
                ParcelArea = parcelHistory.ParcelArea,
                OwnerName = parcelHistory.OwnerName,
                OwnerAddress = parcelHistory.OwnerAddress,
                ParcelStatusID = parcelHistory.ParcelStatusID,
                IsReviewed = parcelHistory.IsReviewed,
                IsManualOverride = parcelHistory.IsManualOverride,
                ReviewDate = parcelHistory.ReviewDate,
                WaterAccountID = parcelHistory.WaterAccountID
            };
            return dto;
        }
    }
}