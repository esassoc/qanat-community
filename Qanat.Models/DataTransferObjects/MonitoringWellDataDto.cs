namespace Qanat.Models.DataTransferObjects;

public class MonitoringWellDataDto
{
    public int MonitoringWellID { get; set; }
    public int GeographyID { get; set; }
    public string MonitoringWellName { get; set; }
    public string SiteCode { get; set; }
    public string MonitoringWellSourceTypeDisplayName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int NumberOfMeasurements { get; set; }
    public DateTime? EarliestMeasurementDate { get; set; }
    public DateTime? LastMeasurementDate { get; set; }
    public decimal? EarliestMeasurement { get; set; }
    public decimal? LastMeasurement { get; set; }
}