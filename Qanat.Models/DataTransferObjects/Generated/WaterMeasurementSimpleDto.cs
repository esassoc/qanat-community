//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurement]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WaterMeasurementSimpleDto
    {
        public int WaterMeasurementID { get; set; }
        public int GeographyID { get; set; }
        public int? WaterMeasurementTypeID { get; set; }
        public int? UnitTypeID { get; set; }
        public string UsageEntityName { get; set; }
        public DateTime ReportedDate { get; set; }
        public decimal ReportedValue { get; set; }
        public decimal? ReportedValueInAcreFeet { get; set; }
        public decimal? UsageEntityArea { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool FromManualUpload { get; set; }
        public string Comment { get; set; }
    }
}