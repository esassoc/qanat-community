namespace Qanat.Models.DataTransferObjects
{
    public class CustomRichTextSimpleDto
    {
        public int CustomRichTextID { get; set; }
        public int CustomRichTextTypeID { get; set; }
        public string CustomRichTextTitle { get; set; }
        public string CustomRichTextContent { get; set; }
        public int? GeographyID { get; set; }
    }
}