namespace Qanat.Models.DataTransferObjects
{
    public class UploadedGdbSimpleDto
    {
        public int UploadedGdbID { get; set; }
        public int UserID { get; set; }
        public int GeographyID { get; set; }
        public string CanonicalName { get; set; }
        public DateTime UploadDate { get; set; }
        public int? EffectiveYear { get; set; }
        public bool Finalized { get; set; }
        public int SRID { get; set; }
    }
}