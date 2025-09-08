namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementSelfReportFileResourceDto
{
    public int WaterMeasurementSelfReportFileResourceID { get; set; }
    public int WaterMeasurementSelfReportID { get; set; }
    public string FileDescription { get; set; }
    public FileResourceSimpleDto FileResource { get; set; }
}
