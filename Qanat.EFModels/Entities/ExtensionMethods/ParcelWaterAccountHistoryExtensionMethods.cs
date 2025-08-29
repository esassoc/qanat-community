using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class ParcelWaterAccountHistoryExtensionMethods
    {
        public static ParcelWaterAccountHistorySimpleDto AsSimpleDto(this ParcelWaterAccountHistory parcelWaterAccountHistory)
        {
            var dto = new ParcelWaterAccountHistorySimpleDto()
            {
                ParcelWaterAccountHistoryID = parcelWaterAccountHistory.ParcelWaterAccountHistoryID,
                GeographyID = parcelWaterAccountHistory.GeographyID,
                ParcelID = parcelWaterAccountHistory.ParcelID,
                ReportingPeriodID = parcelWaterAccountHistory.ReportingPeriodID,
                FromWaterAccountID = parcelWaterAccountHistory.FromWaterAccountID,
                FromWaterAccountNumber = parcelWaterAccountHistory.FromWaterAccountNumber,
                FromWaterAccountName = parcelWaterAccountHistory.FromWaterAccountName,
                ToWaterAccountID = parcelWaterAccountHistory.ToWaterAccountID,
                ToWaterAccountNumber = parcelWaterAccountHistory.ToWaterAccountNumber,
                ToWaterAccountName = parcelWaterAccountHistory.ToWaterAccountName,
                Reason = parcelWaterAccountHistory.Reason,
                CreateUserID = parcelWaterAccountHistory.CreateUserID,
                CreateDate = parcelWaterAccountHistory.CreateDate
            };
            return dto;
        }
    }
}