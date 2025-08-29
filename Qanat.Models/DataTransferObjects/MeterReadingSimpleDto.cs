namespace Qanat.Models.DataTransferObjects
{
    public class MeterReadingSimpleDto
    {
        public int MeterReadingID { get; set; }
        public int GeographyID { get; set; }
        public int WellID { get; set; }
        public int MeterID { get; set; }
        public int MeterReadingUnitTypeID { get; set; }
        public DateTime ReadingDate { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; }
        public decimal Volume { get; set; }
        public decimal VolumeInAcreFeet { get; set; }
        public string ReaderInitials { get; set; }
        public string Comment { get; set; }
    }
}