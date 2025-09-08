using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("CustomRichText")]
[Index("CustomRichTextTypeID", "GeographyID", Name = "AK_CustomRichText_Unique_CustomRichTextTypeID_GeographyID", IsUnique = true)]
public partial class CustomRichText
{
    [Key]
    public int CustomRichTextID { get; set; }

    public int CustomRichTextTypeID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string CustomRichTextTitle { get; set; }

    [Unicode(false)]
    public string CustomRichTextContent { get; set; }

    public int? GeographyID { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("CustomRichTexts")]
    public virtual Geography Geography { get; set; }
}
