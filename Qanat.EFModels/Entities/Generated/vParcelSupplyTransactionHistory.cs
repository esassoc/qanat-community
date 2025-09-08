using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vParcelSupplyTransactionHistory
{
    public int GeographyID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EffectiveDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime TransactionDate { get; set; }

    public int? WaterTypeID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string WaterTypeName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string UploadedFileName { get; set; }

    [StringLength(201)]
    [Unicode(false)]
    public string CreateUserFullName { get; set; }

    public int? AffectedParcelsCount { get; set; }

    public double? AffectedAcresCount { get; set; }

    [Column(TypeName = "decimal(38, 4)")]
    public decimal? TransactionVolume { get; set; }

    public double? TransactionDepth { get; set; }
}
