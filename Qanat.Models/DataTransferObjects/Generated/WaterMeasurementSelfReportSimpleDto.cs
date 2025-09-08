//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementSelfReport]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WaterMeasurementSelfReportSimpleDto
    {
        public int WaterMeasurementSelfReportID { get; set; }
        public int GeographyID { get; set; }
        public int WaterAccountID { get; set; }
        public int WaterMeasurementTypeID { get; set; }
        public int ReportingYear { get; set; }
        public int WaterMeasurementSelfReportStatusID { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserID { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUserID { get; set; }
    }
}