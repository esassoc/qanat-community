using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurementSelfReportLineItemExtensionMethods
{
    public static WaterMeasurementSelfReportLineItemSimpleDto AsSimpleDto(this WaterMeasurementSelfReportLineItem waterMeasurementSelfReportLineItem)
    {
        var dto = new WaterMeasurementSelfReportLineItemSimpleDto()
        {
            WaterMeasurementSelfReportLineItemID = waterMeasurementSelfReportLineItem.WaterMeasurementSelfReportLineItemID,
            WaterMeasurementSelfReportID = waterMeasurementSelfReportLineItem.WaterMeasurementSelfReportID,
            ParcelID = waterMeasurementSelfReportLineItem.ParcelID,
            IrrigationMethodID = waterMeasurementSelfReportLineItem.IrrigationMethodID,
            JanuaryOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.JanuaryOverrideValueInAcreFeet,
            FebruaryOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.FebruaryOverrideValueInAcreFeet,
            MarchOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.MarchOverrideValueInAcreFeet,
            AprilOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.AprilOverrideValueInAcreFeet,
            MayOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.MayOverrideValueInAcreFeet,
            JuneOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.JuneOverrideValueInAcreFeet,
            JulyOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.JulyOverrideValueInAcreFeet,
            AugustOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.AugustOverrideValueInAcreFeet,
            SeptemberOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.SeptemberOverrideValueInAcreFeet,
            OctoberOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.OctoberOverrideValueInAcreFeet,
            NovemberOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.NovemberOverrideValueInAcreFeet,
            DecemberOverrideValueInAcreFeet = waterMeasurementSelfReportLineItem.DecemberOverrideValueInAcreFeet,
            CreateDate = waterMeasurementSelfReportLineItem.CreateDate,
            CreateUserID = waterMeasurementSelfReportLineItem.CreateUserID,
            UpdateDate = waterMeasurementSelfReportLineItem.UpdateDate,
            UpdateUserID = waterMeasurementSelfReportLineItem.UpdateUserID
        };
        return dto;
    }

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