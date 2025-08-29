namespace Qanat.Models.DataTransferObjects
{
    public class WaterMeasurementSimpleDto
    {
        public int WaterMeasurementID { get; set; }
        public int GeographyID { get; set; }
        public int UsageLocationID { get; set; }
        public int? WaterMeasurementTypeID { get; set; }
        public int? UnitTypeID { get; set; }
        public DateTime ReportedDate { get; set; }
        public decimal? ReportedValueInNativeUnits { get; set; }
        public decimal ReportedValueInAcreFeet { get; set; }
        public decimal? ReportedValueInFeet { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool FromManualUpload { get; set; }
        public string Comment { get; set; }
    }
}