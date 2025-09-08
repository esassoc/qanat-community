//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementSelfReportStatus]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterMeasurementSelfReportStatusExtensionMethods
    {
        public static WaterMeasurementSelfReportStatusSimpleDto AsSimpleDto(this WaterMeasurementSelfReportStatus waterMeasurementSelfReportStatus)
        {
            var dto = new WaterMeasurementSelfReportStatusSimpleDto()
            {
                WaterMeasurementSelfReportStatusID = waterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID,
                WaterMeasurementSelfReportStatusName = waterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusName,
                WaterMeasurementSelfReportStatusDisplayName = waterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusDisplayName
            };
            return dto;
        }
    }
}