namespace Qanat.Models.DataTransferObjects;

public class StatementTemplateDto
{
    public int StatementTemplateID { get; set; }
    public int GeographyID { get; set; }
    public StatementTemplateTypeSimpleDto StatementTemplateType { get; set; }
    public string TemplateTitle { get; set; }
    public DateTime LastUpdated { get; set; }
    public int UpdateUserID { get; set; }
    public string UpdateUserFullName { get; set; }
    public string InternalDescription { get; set; }
    public Dictionary<string, string> CustomFieldsContent { get; set; }
    public Dictionary<string, string> CustomLabels { get; set; }
}
