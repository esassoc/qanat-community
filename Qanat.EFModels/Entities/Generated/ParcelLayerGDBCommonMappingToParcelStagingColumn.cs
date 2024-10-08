using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ParcelLayerGDBCommonMappingToParcelStagingColumn")]
[Index("OwnerAddress", Name = "AK_ParcelLayerGDBCommonMappingToParcelColumn_OwnerAddress", IsUnique = true)]
[Index("OwnerName", Name = "AK_ParcelLayerGDBCommonMappingToParcelColumn_OwnerName", IsUnique = true)]
[Index("ParcelNumber", Name = "AK_ParcelLayerGDBCommonMappingToParcelColumn_ParcelNumber", IsUnique = true)]
public partial class ParcelLayerGDBCommonMappingToParcelStagingColumn
{
    [Key]
    public int ParcelLayerGDBCommonMappingToParcelColumnID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string ParcelNumber { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string OwnerName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string OwnerAddress { get; set; }
}
