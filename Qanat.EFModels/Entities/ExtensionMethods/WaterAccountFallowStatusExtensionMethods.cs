using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities.ExtensionMethods;

public static class WaterAccountFallowStatusExtensionMethods
{
    public static WaterAccountFallowStatusDto AsDto(this WaterAccountFallowStatus waterAccountFallowStatus, int reportingPeriodID, List<vWaterMeasurementSourceOfRecord> sourceOfRecords = null)
    {
        var usageLocations = waterAccountFallowStatus.WaterAccount.WaterAccountParcels
            .Where(x => x.ReportingPeriodID == reportingPeriodID)
            .SelectMany(x => x.Parcel.UsageLocations.ToList())
            .Where(x => x.ReportingPeriodID == reportingPeriodID && x.UsageLocationType.CanBeRemoteSensed)
            .ToList();

        return new WaterAccountFallowStatusDto
        {
            WaterAccountFallowStatusID = waterAccountFallowStatus.WaterAccountFallowStatusID,
            Geography = waterAccountFallowStatus.Geography.AsMinimalDto(),
            SelfReportStatus = waterAccountFallowStatus.SelfReportStatus.AsSimpleDto(),
            ReportingPeriod = waterAccountFallowStatus.ReportingPeriod.AsSimpleDto(),
            WaterAccount = waterAccountFallowStatus.WaterAccount.AsWaterAccountMinimalDto(),
            UsageLocations = usageLocations.Select(x => x.AsDto(sourceOfRecords)).ToList(),

            SubmittedByUser = waterAccountFallowStatus.SubmittedByUser?.AsUserWithFullNameDto(),
            SubmittedDate = waterAccountFallowStatus.SubmittedDate,
            ApprovedByUser = waterAccountFallowStatus.ApprovedByUser?.AsUserWithFullNameDto(),
            ApprovedDate = waterAccountFallowStatus.ApprovedDate,
            ReturnedByUser = waterAccountFallowStatus.ReturnedByUser?.AsUserWithFullNameDto(),
            ReturnedDate = waterAccountFallowStatus.ReturnedDate,

            CreateUser = waterAccountFallowStatus.CreateUser?.AsUserWithFullNameDto(),
            CreateDate = waterAccountFallowStatus.CreateDate,
            UpdateUser = waterAccountFallowStatus.UpdateUser?.AsUserWithFullNameDto(),
            UpdateDate = waterAccountFallowStatus.UpdateDate,
        };
    }
}