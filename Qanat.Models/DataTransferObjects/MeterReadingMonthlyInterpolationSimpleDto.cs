namespace Qanat.Models.DataTransferObjects
{
    public class MeterReadingMonthlyInterpolationSimpleDto
    {
        public int MeterReadingMonthlyInterpolationID { get; set; }
        public int GeographyID { get; set; }
        public int WellID { get; set; }
        public int MeterID { get; set; }
        public int MeterReadingUnitTypeID { get; set; }
        public DateTime Date { get; set; }
        public decimal InterpolatedVolume { get; set; }
        public decimal InterpolatedVolumeInAcreFeet { get; set; }
    }
}