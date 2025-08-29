namespace Qanat.Models.DataTransferObjects
{
    public class FileResourceSimpleDto
    {
        public int FileResourceID { get; set; }
        public string OriginalBaseFilename { get; set; }
        public string OriginalFileExtension { get; set; }
        public Guid FileResourceGUID { get; set; }
        public string FileResourceCanonicalName { get; set; }
        public int CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
    }
}