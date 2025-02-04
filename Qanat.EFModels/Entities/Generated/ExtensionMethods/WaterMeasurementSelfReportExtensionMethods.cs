//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementSelfReport]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterMeasurementSelfReportExtensionMethods
    {
        public static WaterMeasurementSelfReportSimpleDto AsSimpleDto(this WaterMeasurementSelfReport waterMeasurementSelfReport)
        {
            var dto = new WaterMeasurementSelfReportSimpleDto()
            {
                WaterMeasurementSelfReportID = waterMeasurementSelfReport.WaterMeasurementSelfReportID,
                GeographyID = waterMeasurementSelfReport.GeographyID,
                WaterAccountID = waterMeasurementSelfReport.WaterAccountID,
                WaterMeasurementTypeID = waterMeasurementSelfReport.WaterMeasurementTypeID,
                ReportingYear = waterMeasurementSelfReport.ReportingYear,
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
    }
}