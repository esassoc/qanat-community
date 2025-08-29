namespace Qanat.Models.DataTransferObjects;

public class UploadedGdbDto
{
    public int UploadedGdbID { get; set; }
    public UserDto User { get; set; }
    public GeographySimpleDto Geography { get; set; }
    public string CanonicalName { get; set; }
    public DateTime UploadDate { get; set; }
    public int? EffectiveYear { get; set; }
    public bool Finalized { get; set; }
    public int SRID { get; set; }
}