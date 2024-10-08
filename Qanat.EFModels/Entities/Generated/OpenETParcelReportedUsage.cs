using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities
{
    [Table("OpenETParcelReportedUsage")]
    public partial class OpenETParcelReportedUsage
    {
        [Key]
        public int OpenETParcelReportedUsageID { get; set; }
        public int GeographyID { get; set; }
        public int OpenETDataTypeID { get; set; }
        [Required]
        [StringLength(20)]
        [Unicode(false)]
        public string ParcelNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ReportedDate { get; set; }
        [Column(TypeName = "decimal(20, 4)")]
        public decimal ReportedValue { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdateDate { get; set; }

        [ForeignKey("GeographyID")]
        [InverseProperty("OpenETParcelReportedUsages")]
        public virtual Geography Geography { get; set; }
    }
}
