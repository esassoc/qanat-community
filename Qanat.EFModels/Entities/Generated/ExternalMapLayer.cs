using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ExternalMapLayer")]
[Index("ExternalMapLayerDisplayName", "GeographyID", Name = "AK_ExternalMapLayers_Unique_ExternalMapLayerDisplayName_GeographyID", IsUnique = true)]
public partial class ExternalMapLayer
{
    [Key]
    public int ExternalMapLayerID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string ExternalMapLayerDisplayName { get; set; }

    public int ExternalMapLayerTypeID { get; set; }

    [Required]
    [StringLength(500)]
    [Unicode(false)]
    public string ExternalMapLayerURL { get; set; }

    public bool LayerIsOnByDefault { get; set; }

    public bool IsActive { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string ExternalMapLayerDescription { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PopUpField { get; set; }

    public int? MinZoom { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ExternalMapLayers")]
    public virtual Geography Geography { get; set; }
}
