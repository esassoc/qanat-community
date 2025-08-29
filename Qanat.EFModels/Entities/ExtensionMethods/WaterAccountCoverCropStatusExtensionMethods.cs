using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities.ExtensionMethods;

public static class WaterAccountCoverCropStatusExtensionMethods
{
    public static WaterAccountCoverCropStatusDto AsDto(this WaterAccountCoverCropStatus waterAccountCoverCropStatus, int reportingPeriodID, List<vWaterMeasurementSourceOfRecord> sourceOfRecordMeasurements = null)
    {
        var usageLocations = waterAccountCoverCropStatus.WaterAccount.WaterAccountParcels
            .Where(x => x.ReportingPeriodID == reportingPeriodID)
            .SelectMany(x => x.Parcel.UsageLocations.ToList())
            .Where(x => x.ReportingPeriodID == reportingPeriodID && x.UsageLocationType.CanBeRemoteSensed)
            .ToList();

        return new WaterAccountCoverCropStatusDto
        {
            WaterAccountCoverCropStatusID = waterAccountCoverCropStatus.WaterAccountCoverCropStatusID,
            Geography = waterAccountCoverCropStatus.Geography.AsMinimalDto(),
            SelfReportStatus = waterAccountCoverCropStatus.SelfReportStatus.AsSimpleDto(),
            ReportingPeriod = waterAccountCoverCropStatus.ReportingPeriod.AsSimpleDto(),
            WaterAccount = waterAccountCoverCropStatus.WaterAccount.AsWaterAccountMinimalDto(),
            UsageLocations = usageLocations.Select(x => x.AsDto(sourceOfRecordMeasurements)).ToList(),
            
            SubmittedByUser = waterAccountCoverCropStatus.SubmittedByUser?.AsUserWithFullNameDto(),
            SubmittedDate = waterAccountCoverCropStatus.SubmittedDate,
            ApprovedByUser = waterAccountCoverCropStatus.ApprovedByUser?.AsUserWithFullNameDto(),
            ApprovedDate = waterAccountCoverCropStatus.ApprovedDate,
            ReturnedByUser = waterAccountCoverCropStatus.ReturnedByUser?.AsUserWithFullNameDto(),
            ReturnedDate = waterAccountCoverCropStatus.ReturnedDate,

            CreateUser = waterAccountCoverCropStatus.CreateUser?.AsUserWithFullNameDto(),
            CreateDate = waterAccountCoverCropStatus.CreateDate,
            UpdateUser = waterAccountCoverCropStatus.UpdateUser?.AsUserWithFullNameDto(),
            UpdateDate = waterAccountCoverCropStatus.UpdateDate,
        };
    }
}