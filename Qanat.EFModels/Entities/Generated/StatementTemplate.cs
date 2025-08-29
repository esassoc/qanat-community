using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("StatementTemplate")]
[Index("GeographyID", "TemplateTitle", Name = "AK_StatementTemplate_GeographyID_TemplateName", IsUnique = true)]
public partial class StatementTemplate
{
    [Key]
    public int StatementTemplateID { get; set; }

    public int GeographyID { get; set; }

    public int StatementTemplateTypeID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string TemplateTitle { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastUpdated { get; set; }

    public int UpdateUserID { get; set; }

    [Unicode(false)]
    public string InternalDescription { get; set; }

    [Required]
    [Unicode(false)]
    public string CustomFieldsContent { get; set; }

    [Unicode(false)]
    public string CustomLabels { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("StatementTemplates")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("StatementTemplate")]
    public virtual ICollection<StatementBatch> StatementBatches { get; set; } = new List<StatementBatch>();

    [ForeignKey("UpdateUserID")]
    [InverseProperty("StatementTemplates")]
    public virtual User UpdateUser { get; set; }
}
