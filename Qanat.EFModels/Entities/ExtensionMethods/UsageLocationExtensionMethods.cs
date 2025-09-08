using Qanat.EFModels.Entities.ExtensionMethods;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class UsageLocationExtensionMethods
{
    public static UsageLocationDto AsDto(this UsageLocation usageLocation, List<vWaterMeasurementSourceOfRecord> sourceOfRecords = null)
    {
        var waterAccount = usageLocation.Parcel.WaterAccountParcels.FirstOrDefault(x => x.ReportingPeriodID == usageLocation.ReportingPeriodID)?.WaterAccount;
        var fallowSelfReport = waterAccount?.WaterAccountFallowStatuses
            .FirstOrDefault(x => x.ReportingPeriodID == usageLocation.ReportingPeriodID);

        var coverCropSelfReport = waterAccount?.WaterAccountCoverCropStatuses
            .FirstOrDefault(x => x.ReportingPeriodID == usageLocation.ReportingPeriodID);

        string sourceOfRecordName = null;
        decimal? sourceOfRecordsValueInAcreFeet = null;
        decimal? sourceOfRecordsValueInFeet = null;
        if (sourceOfRecords != null)
        {
            var sourceOfRecordsForUsageLocation = sourceOfRecords.Where(x => x.UsageLocationID == usageLocation.UsageLocationID).ToList();
            if (sourceOfRecordsForUsageLocation.Any())
            {
                sourceOfRecordName = sourceOfRecordsForUsageLocation.First().WaterMeasurementTypeName;
                sourceOfRecordsValueInAcreFeet = sourceOfRecordsForUsageLocation.Sum(x => x.ReportedValueInAcreFeet);
                sourceOfRecordsValueInFeet = sourceOfRecordsForUsageLocation.Sum(x => x.ReportedValueInFeet);
            }
        }

        return new UsageLocationDto
        {
            UsageLocationID = usageLocation.UsageLocationID,
            Geography = usageLocation.Geography.AsSimpleDto(),
            UsageLocationType = usageLocation.UsageLocationType?.AsSimpleDto(),
            WaterAccount = waterAccount?.AsSimpleDto(),
            Parcel = usageLocation.Parcel.AsSimpleDto(),
            ReportingPeriod = usageLocation.ReportingPeriod.AsSimpleDto(),
            Name = usageLocation.Name,
            Area = usageLocation.Area,
            FallowStatus = fallowSelfReport?.SelfReportStatus.SelfReportStatusDisplayName,
            FallowSelfReportApproved = fallowSelfReport?.ApprovedDate.HasValue ?? false,
            FallowNote = usageLocation.FallowNote,
            CoverCropStatus = coverCropSelfReport?.SelfReportStatus.SelfReportStatusDisplayName,
            CoverCropSelfReportApproved = coverCropSelfReport?.ApprovedDate.HasValue ?? false,
            CoverCropNote = usageLocation.CoverCropNote,
            SourceOfRecordWaterMeasurementTypeName = sourceOfRecordName,
            SourceOfRecordValueInAcreFeet = sourceOfRecordsValueInAcreFeet,
            SourceOfRecordValueInFeet = sourceOfRecordsValueInFeet,
            Crops = usageLocation.UsageLocationCrops.Select(c => c.AsSimpleDto()).ToList(),
            CreateUser = usageLocation.CreateUser.AsSimpleDto(),
            CreateDate = usageLocation.CreateDate,
            UpdateUser = usageLocation.UpdateUser?.AsSimpleDto(),
            UpdateDate = usageLocation.UpdateDate
        };
    }
}