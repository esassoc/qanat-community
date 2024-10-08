using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class GeographyAllocationPlanConfigurationDto
{
    public int? GeographyAllocationPlanConfigurationID { get; set; }

    [Required]
    public int? GeographyID { get; set; }

    [Required]
    [DisplayName("Zone Group")]
    public int? ZoneGroupID { get; set; }

    [Required]
    [DisplayName("Start Year")]
    public int? StartYear { get; set; }

    [Required]
    [DisplayName("End Year")]
    public int? EndYear { get; set; }

    [Required]
    [DisplayName("Enabled")]
    public bool? IsActive { get; set; }

    [Required]
    [DisplayName("Show Page on Water Dashboard")]
    public bool? IsVisibleToLandowners { get; set; }

    [Required]
    [DisplayName("Allow public to view the Allocation Plans")]
    public bool? IsVisibleToPublic { get; set; }

    [DisplayName("Allocation Plans Description")]
    public string AllocationPlansDescription { get; set; }

    [Required]
    [DisplayName("Water Supply Types")]
    public List<int> WaterTypeIDs { get; set; }
}