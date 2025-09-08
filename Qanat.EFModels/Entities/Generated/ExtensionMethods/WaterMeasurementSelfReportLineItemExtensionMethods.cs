//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementSelfReportLineItem]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterMeasurementSelfReportLineItemExtensionMethods
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
    }
}