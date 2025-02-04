using System.Runtime.CompilerServices;

namespace Qanat.Models.DataTransferObjects;

public partial class WaterMeasurementSelfReportSimpleDto
{
    public string WaterAccountNumberAndName { get; set; }
    public string WaterMeasurementTypeName { get; set; }
    public string WaterMeasurementSelfReportStatusDisplayName { get; set; }
    public decimal? TotalVolume { get; set; }
    public string CreateUserFullName { get; set; }
    public string UpdateUserFullName { get; set; }
}

public partial class WaterMeasurementSelfReportLineItemSimpleDto
{
    public string ParcelNumber { get; set; }
    public double? ParcelArea { get; set; }
    public string IrrigationMethodName { get; set; }
    public decimal? LineItemTotal { get; set; }
}