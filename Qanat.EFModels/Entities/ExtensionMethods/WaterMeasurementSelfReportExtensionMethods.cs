using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurementSelfReportExtensionMethods
{
    public static WaterMeasurementSelfReportSimpleDto AsSimpleDto(this WaterMeasurementSelfReport waterMeasurementSelfReport)
    {
        var dto = new WaterMeasurementSelfReportSimpleDto()
        {
            WaterMeasurementSelfReportID = waterMeasurementSelfReport.WaterMeasurementSelfReportID,
            GeographyID = waterMeasurementSelfReport.GeographyID,
            WaterAccountID = waterMeasurementSelfReport.WaterAccountID,
            WaterMeasurementTypeID = waterMeasurementSelfReport.WaterMeasurementTypeID,
            ReportingPeriodID = waterMeasurementSelfReport.ReportingPeriodID,
            WaterMeasurementSelfReportStatusID = waterMeasurementSelfReport.WaterMeasurementSelfReportStatusID,
            SubmittedDate = waterMeasurementSelfReport.SubmittedDate,
            ApprovedDate = waterMeasurementSelfReport.ApprovedDate,
            ReturnedDate = waterMeasurementSelfReport.ReturnedDate,
            CreateDate = waterMeasurementSelfReport.CreateDate,
            CreateUserID = waterMeasurementSelfReport.CreateUserID,
            UpdateDate = waterMeasurementSelfReport.UpdateDate,
            UpdateUserID = waterMeasurementSelfReport.UpdateUserID
        };
        return dto;
    }

    public static WaterMeasurementSelfReportSimpleDto AsSimpleDtoWithExtras(this WaterMeasurementSelfReport waterMeasurementSelfReport)
    {
        var simpleDto = AsSimpleDto(waterMeasurementSelfReport);
        simpleDto.WaterAccountNumberAndName = waterMeasurementSelfReport.WaterAccount?.WaterAccountNumberAndName();
        simpleDto.ReportingPeriodName = waterMeasurementSelfReport.ReportingPeriod?.Name;
        simpleDto.WaterMeasurementTypeName = waterMeasurementSelfReport.WaterMeasurementType?.WaterMeasurementTypeName;
        simpleDto.WaterMeasurementSelfReportStatusDisplayName = waterMeasurementSelfReport.WaterMeasurementSelfReportStatus?.SelfReportStatusDisplayName;
        simpleDto.TotalVolume = waterMeasurementSelfReport.WaterMeasurementSelfReportLineItems.Sum(x => x.TotalAcreFeet);
        simpleDto.FileCount = waterMeasurementSelfReport.WaterMeasurementSelfReportFileResources.Count;
        simpleDto.CreateUserFullName = waterMeasurementSelfReport.CreateUser?.FullName;
        simpleDto.UpdateUserFullName = waterMeasurementSelfReport.UpdateUser?.FullName;
        return simpleDto;
    }

    public static WaterMeasurementSelfReportDto AsDto(this WaterMeasurementSelfReport waterMeasurementSelfReport)
    {
        var dto = new WaterMeasurementSelfReportDto
        {
            WaterMeasurementSelfReportID = waterMeasurementSelfReport.WaterMeasurementSelfReportID,
            Geography = waterMeasurementSelfReport.Geography.AsSimpleDto(),
            WaterAccount = waterMeasurementSelfReport.WaterAccount.AsSimpleDto(),
            WaterMeasurementType = waterMeasurementSelfReport.WaterMeasurementType.AsSimpleDto(),
            ReportingPeriod = waterMeasurementSelfReport.ReportingPeriod.AsSimpleDto(),
            WaterMeasurementSelfReportStatus = waterMeasurementSelfReport.WaterMeasurementSelfReportStatus.AsSimpleDto(),
            SubmittedDate = waterMeasurementSelfReport.SubmittedDate,
            ApprovedDate = waterMeasurementSelfReport.ApprovedDate,
            ReturnedDate = waterMeasurementSelfReport.ReturnedDate,
            LineItems = waterMeasurementSelfReport.WaterMeasurementSelfReportLineItems.OrderBy(x=> x.Parcel.ParcelNumber).Select(x => x.AsSimpleDtoWithExtras()).ToList(),
            CreateDate = waterMeasurementSelfReport.CreateDate,
            CreateUser = waterMeasurementSelfReport.CreateUser.AsSimpleDto(),
            UpdateDate = waterMeasurementSelfReport.UpdateDate,
            UpdateUser = waterMeasurementSelfReport.UpdateUser?.AsSimpleDto()
        };

        return dto;
    }
}