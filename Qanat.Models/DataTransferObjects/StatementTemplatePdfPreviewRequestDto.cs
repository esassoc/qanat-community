using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class StatementTemplatePdfPreviewRequestDto
{
    [Required(ErrorMessage = "Please select a Water Account to preview.")]
    public int WaterAccountID { get; set; }

    [Required(ErrorMessage = "Please select a Reporting Period to preview.")]
    public int ReportingPeriodYear { get; set; }

    [Required(ErrorMessage = "Please select a Template Type to preview.")]
    public int StatementTemplateTypeID { get; set; }
    
    public string StatementTemplateTitle { get; set; }

    public Dictionary<string, string> CustomFields { get; set; }
    public Dictionary<string, string> CustomLabels { get; set; }
}