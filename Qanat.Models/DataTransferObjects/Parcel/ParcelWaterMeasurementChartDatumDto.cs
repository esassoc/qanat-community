namespace Qanat.Models.DataTransferObjects;

public class ParcelWaterMeasurementChartDatumDto
{
    public string WaterMeasurementTypeName { get; set; }
    public DateTime ReportedDate { get; set; }
    public decimal? ReportedValueInAcreFeet { get; set; }
}