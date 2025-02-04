using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class WaterMeasurementSelfReportExtensionMethods
{
    public static WaterMeasurementSelfReportSimpleDto AsSimpleDtoWithExtras(this WaterMeasurementSelfReport waterMeasurementSelfReport)
    {
        var simpleDto = AsSimpleDto(waterMeasurementSelfReport);
        simpleDto.WaterAccountNumberAndName = waterMeasurementSelfReport.WaterAccount?.WaterAccountNumberAndName();
        simpleDto.WaterMeasurementTypeName = waterMeasurementSelfReport.WaterMeasurementType?.WaterMeasurementTypeName;
        simpleDto.WaterMeasurementSelfReportStatusDisplayName = waterMeasurementSelfReport.WaterMeasurementSelfReportStatus?.WaterMeasurementSelfReportStatusDisplayName;
        simpleDto.TotalVolume = waterMeasurementSelfReport.WaterMeasurementSelfReportLineItems.Sum(x => x.TotalAcreFeet);
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
            ReportingYear = waterMeasurementSelfReport.ReportingYear,
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

public static partial class WaterMeasurementSelfReportLineItemExtensionMethods
{
    public static WaterMeasurementSelfReportLineItemSimpleDto AsSimpleDtoWithExtras(this WaterMeasurementSelfReportLineItem lineItem)
    {
        var simpleDto = AsSimpleDto(lineItem);

        simpleDto.ParcelNumber = lineItem.Parcel?.ParcelNumber;
        simpleDto.ParcelArea = lineItem.Parcel?.ParcelArea;
        simpleDto.IrrigationMethodName = lineItem.IrrigationMethod?.Name;
        simpleDto.LineItemTotal = lineItem.JanuaryOverrideValueInAcreFeet.GetValueOrDefault(0) 
            + lineItem.FebruaryOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.MarchOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.AprilOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.MayOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.JuneOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.JulyOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.AugustOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.SeptemberOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.OctoberOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.NovemberOverrideValueInAcreFeet.GetValueOrDefault(0)
            + lineItem.DecemberOverrideValueInAcreFeet.GetValueOrDefault(0);

        return simpleDto;
    }
}