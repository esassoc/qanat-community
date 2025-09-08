using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class CopyWaterAccountParcelsFromReportingPeriodDto
{
    [Required]
    public int FromReportingPeriodID { get; set; }
}