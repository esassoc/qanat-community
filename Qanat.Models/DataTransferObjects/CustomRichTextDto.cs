namespace Qanat.Models.DataTransferObjects
{
    public class CustomRichTextDto
    {
        public int CustomRichTextID { get; set; }
        public CustomRichTextTypeSimpleDto CustomRichTextType { get; set; }
        public string CustomRichTextTitle { get; set; }
        public string CustomRichTextContent { get; set; }
        public GeographySimpleDto Geography { get; set; }
        public bool IsEmptyContent { get; set; }
    }
}