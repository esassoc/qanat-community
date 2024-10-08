namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationUserDetailDto
{
    public int WellRegistrationID { get; set; }
    public GeographyDisplayDto Geography { get; set; }
    public string WellName { get; set; }
    public WellRegistrationStatusSimpleDto WellRegistrationStatus { get; set; }
    public ParcelDisplayDto? Parcel{ get; set; }
    public string StateWCRNumber { get; set; }
    public string CountyWellPermitNumber { get; set; }
    public DateOnly? DateDrilled { get; set; }
    public int? WellDepth { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public int? CreateUserID { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}