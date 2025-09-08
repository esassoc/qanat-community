using System;

namespace Qanat.Swagger.Models;

public class MeterReadingConsumerDto
{
    public int MeterReadingID { get; set; }
    public string MeterSerialNumber { get; set; }
    public int WellID { get; set; }
    public int? ParcelID { get; set; }
    public DateTime ReadingDate { get; set; }
    public decimal PreviousReading { get; set; }
    public decimal CurrentReading { get; set; }
    public decimal VolumeInAcreFeet { get; set; }
    public string ReaderInitials { get; set; }
    public string Comment { get; set; }
    public int GeographyID { get; set; }
}