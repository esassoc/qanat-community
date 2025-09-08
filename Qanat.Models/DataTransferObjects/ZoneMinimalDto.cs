using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ZoneMinimalDto
{
    public int ZoneID { get; set; }
    public int? ZoneGroupID { get; set; }
    public string ZoneName { get; set; }
    public string ZoneSlug { get; set; }
    [MaxLength(500)]
    public string ZoneDescription { get; set; }
    public string ZoneColor { get; set; }
    public string ZoneAccentColor { get; set; }
    public decimal? PrecipMultiplier { get; set; }
    public int SortOrder { get; set; }
    public string ZoneGroupName { get; set; }
    public bool ZoneGroupDisplayToAccountHolders { get; set; }
}