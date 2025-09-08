using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountParcelDto
{
    public int WaterAccountParcelID { get; set; }
    public int GeographyID { get; set; }
    public WaterAccountMinimalDto WaterAccount { get; set; }
    public ParcelSimpleDto Parcel { get; set; }
    public ReportingPeriodSimpleDto ReportingPeriod { get; set; }
}

public class ParcelMinimalAndReportingPeriodSimpleDto
{
    public ParcelMinimalDto Parcel { get; set; }
    public ReportingPeriodSimpleDto ReportingPeriod { get; set; }
}

public class WaterAccountMinimalAndReportingPeriodSimpleDto
{
    public WaterAccountMinimalDto WaterAccount { get; set; }
    public ReportingPeriodSimpleDto ReportingPeriod { get; set; }
}

public class UpdateWaterAccountParcelsByParcelDto
{
    [Required]
    public List<WaterAccountReportingPeriodDto> ReportingPeriodWaterAccounts { get; set; }
}

public class WaterAccountReportingPeriodDto
{
    [Required]
    public int ReportingPeriodID { get; set; }

    public int? WaterAccountID { get; set; }
}