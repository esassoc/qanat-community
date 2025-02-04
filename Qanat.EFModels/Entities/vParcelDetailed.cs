using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;
using System.ComponentModel.DataAnnotations;

namespace Qanat.EFModels.Entities;

public class vParcelDetailed
{
    public int GeographyID { get; set; }
    
    [Key]
    public int ParcelID { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string ParcelNumber { get; set; }

    public double ParcelArea { get; set; }

    [Required]
    [StringLength(500)]
    [Unicode(false)]
    public string OwnerName { get; set; }

    [Required]
    [StringLength(500)]
    [Unicode(false)]
    public string OwnerAddress { get; set; }

    public int ParcelStatusID { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string ParcelStatusDisplayName { get; set; }

    public int? WaterAccountID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string WaterAccountName { get; set; }

    public int? WaterAccountNumber { get; set; }

    [StringLength(7)]
    [Unicode(false)]
    public string WaterAccountPIN { get; set; }

    public string CustomAttributes { get; set; }

    public string ZoneIDs { get; set; }

    public List<WellLinkDisplayDto> WellsOnParcel { get; } = [];

    public List<WellLinkDisplayDto> IrrigatedByWells { get; } = [];
}
