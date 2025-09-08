namespace Qanat.Models.DataTransferObjects
{
    public class StatementTemplateSimpleDto
    {
        public int StatementTemplateID { get; set; }
        public int GeographyID { get; set; }
        public int StatementTemplateTypeID { get; set; }
        public string TemplateName { get; set; }
        public DateTime LastUpdated { get; set; }
        public int UpdateUserID { get; set; }
        public string Description { get; set; }
        public string CustomFieldsContent { get; set; }
        public string CustomLabels { get; set; }
    }
}