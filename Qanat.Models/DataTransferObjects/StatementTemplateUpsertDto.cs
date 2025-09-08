using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class StatementTemplateUpsertDto
{
    [Required]
    public int GeographyID { get; set; }

    [Required(ErrorMessage = "The Statement Template Type field is required.")]
    public int StatementTemplateTypeID { get; set; }

    [Required(ErrorMessage = "The Template Title field is required.")]
    [MaxLength(100, ErrorMessage = "Template Title cannot exceed 100 characters.")]
    public string TemplateTitle { get; set; }

    public string InternalDescription { get; set; }

    [Required(ErrorMessage = "The customizable text fields are required.")]
    public Dictionary<string, string> CustomFieldsContent { get; set; }

    [Required]
    public Dictionary<string, string> CustomLabels { get; set; }
}