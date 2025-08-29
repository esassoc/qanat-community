namespace Qanat.Models.DataTransferObjects;

public class MeterReadingGridDto
{   
    public int GeographyID { get; set; }
    public string GeographyDisplayName { get; set; }
    public int WellID { get; set; }
    public string WellName { get; set; }
    public int MeterID { get; set; }
    public string SerialNumber { get; set; }
    public int MeterReadingUnitTypeID { get; set; }
    public string MeterReadingUnitTypeDisplayName { get; set; }
    public DateTime ReadingDate { get; set; }
    public decimal PreviousReading { get; set; }
    public decimal CurrentReading { get; set; }
    public decimal Volume { get; set; }
    public decimal VolumeInAcreFeet { get; set; }
    public string ReaderInitials { get; set; }
    public string Comment { get; set; }
}