using System;

namespace Qanat.Models.DataTransferObjects;

public class MonitoringWellMeasurementDataDto
{
    public int MonitoringWellMeasurementID { get; set; }
    public int MonitoringWellID { get; set; }
    public int GeographyID { get; set; }
    public int ExtenalUniqueID { get; set; }
    public decimal Measurement { get; set; }
    public DateTime MeasurementDate { get; set; }
    public string MonitoringWellName { get; set; }
    public string SiteCode { get; set; }
}