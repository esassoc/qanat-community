namespace Qanat.Models.DataTransferObjects
{
    public class CustomPageDto
    {
        public int CustomPageID { get; set; }
        public string CustomPageDisplayName { get; set; }
        public string CustomPageVanityUrl { get; set; }
        public string CustomPageContent { get; set; }
        public MenuItemSimpleDto MenuItem { get; set; }
        public int? SortOrder { get; set; }
        public bool IsEmptyContent => string.IsNullOrEmpty(CustomPageContent);
    }
}