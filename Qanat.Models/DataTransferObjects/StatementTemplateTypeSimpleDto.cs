namespace Qanat.Models.DataTransferObjects
{
    public class StatementTemplateTypeSimpleDto
    {
        public int StatementTemplateTypeID { get; set; }
        public string StatementTemplateTypeName { get; set; }
        public string StatementTemplateTypeDisplayName { get; set; }
        public string CustomFieldDefaultParagraphs { get; set; }
        public string CustomLabelDefaults { get; set; }
    }
}