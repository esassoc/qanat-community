using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class GeographyForAdminEditorsDto
{
    [Required]
    public int GeographyID { get; set; }

    [Required]
    [StringLength(100)]
    public string GeographyDisplayName { get; set; }

    [Required]
    public int? DefaultReportingPeriodID { get; set; }

    [Required]
    [StringLength(100)]
    public string APNRegexPattern { get; set; }

    [Required]
    [StringLength(50)]
    public string APNRegexDisplay { get; set; }

    [Required]
    [StringLength(200)]
    public string LandownerDashboardSupplyLabel { get; set; }

    [Required]
    [StringLength(200)]
    public string LandownerDashboardUsageLabel { get; set; }
    [EmailAddress]
    public string ContactEmail { get; set; }
    [MinLength(10)]
    [MaxLength(10)]
    public string ContactPhoneNumber { get; set; }
    [Required]
    public bool DisplayUsageGeometriesAsField { get; set; }
    [Required]
    public bool AllowLandownersToRequestAccountChanges { get; set; }
    [Required]
    public bool ShowSupplyOnWaterBudgetComponent { get; set; }

    [Required]
    public bool AllowWaterMeasurementSelfReporting { get; set; }

    public string WaterBudgetSlotAHeader { get; set; }
    public int? WaterBudgetSlotAWaterMeasurementTypeID { get; set; }

    public string WaterBudgetSlotBHeader { get; set; }
    public int? WaterBudgetSlotBWaterMeasurementTypeID { get; set; }

    public string WaterBudgetSlotCHeader { get; set; }
    public int? WaterBudgetSlotCWaterMeasurementTypeID { get; set; }
    public BoundingBoxDto BoundingBox { get; set; }
    public List<UserDto> WaterManagers { get; set; }
}