//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementSelfReportLineItem]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WaterMeasurementSelfReportLineItemSimpleDto
    {
        public int WaterMeasurementSelfReportLineItemID { get; set; }
        public int WaterMeasurementSelfReportID { get; set; }
        public int ParcelID { get; set; }
        public int IrrigationMethodID { get; set; }
        public decimal? JanuaryOverrideValueInAcreFeet { get; set; }
        public decimal? FebruaryOverrideValueInAcreFeet { get; set; }
        public decimal? MarchOverrideValueInAcreFeet { get; set; }
        public decimal? AprilOverrideValueInAcreFeet { get; set; }
        public decimal? MayOverrideValueInAcreFeet { get; set; }
        public decimal? JuneOverrideValueInAcreFeet { get; set; }
        public decimal? JulyOverrideValueInAcreFeet { get; set; }
        public decimal? AugustOverrideValueInAcreFeet { get; set; }
        public decimal? SeptemberOverrideValueInAcreFeet { get; set; }
        public decimal? OctoberOverrideValueInAcreFeet { get; set; }
        public decimal? NovemberOverrideValueInAcreFeet { get; set; }
        public decimal? DecemberOverrideValueInAcreFeet { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserID { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUserID { get; set; }
    }
}