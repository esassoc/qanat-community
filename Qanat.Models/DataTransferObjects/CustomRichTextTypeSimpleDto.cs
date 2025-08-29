namespace Qanat.Models.DataTransferObjects
{
    public class CustomRichTextTypeSimpleDto
    {
        public int CustomRichTextTypeID { get; set; }
        public string CustomRichTextTypeName { get; set; }
        public string CustomRichTextTypeDisplayName { get; set; }
        public int? ContentTypeID { get; set; }
    }
}