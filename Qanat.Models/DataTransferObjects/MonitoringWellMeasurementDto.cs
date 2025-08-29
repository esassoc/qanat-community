namespace Qanat.Models.DataTransferObjects;

public class MonitoringWellMeasurementDto
{
    public int MonitoringWellMeasurementID { get; set; }
    public MonitoringWellSimpleDto MonitoringWell { get; set; }
    public GeographySimpleDto Geography { get; set; }
    public int ExtenalUniqueID { get; set; }
    public decimal Measurement { get; set; }
    public DateTime MeasurementDate { get; set; }
}