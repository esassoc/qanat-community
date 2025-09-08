namespace Qanat.Models.DataTransferObjects;

public class MeterReadingDto
{
    public int MeterReadingID { get; set; }
    public int MeterID { get; set; }
    public string MeterSerialNumber { get; set; }
    public int WellID { get; set; }
    public DateTime ReadingDate { get; set; }
    public decimal PreviousReading { get; set; }
    public decimal CurrentReading { get; set; }
    public decimal Volume { get; set; }
    public decimal VolumeInAcreFeet { get; set; }
    public MeterReadingUnitTypeSimpleDto MeterReadingUnitType { get; set; }
    public string ReaderInitials { get; set; }
    public string Comment { get; set; }
}