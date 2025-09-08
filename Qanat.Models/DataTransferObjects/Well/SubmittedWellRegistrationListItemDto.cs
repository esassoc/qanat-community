namespace Qanat.Models.DataTransferObjects;

public class SubmittedWellRegistrationListItemDto
{
    public int WellRegistrationID { get; set; }
    public string WellName { get; set; }
    public int? ParcelID { get; set; }
    public string APN { get; set; }
    public DateTime? DateSubmitted { get; set; }
    public string CreatedBy { get; set; }
    public int? CreatedByUserID { get; set; }
}