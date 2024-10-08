namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationMinimalDto
{
    public int WellRegistrationID { get; set; }
    public int GeographyID { get; set; }
    public string WellName { get; set; }
    public int WellRegistrationStatusID { get; set; }
    public int? ParcelID { get; set; }
    public string StateWCRNumber { get; set; }
    public string CountyWellPermitNumber { get; set; }
    public DateOnly? DateDrilled { get; set; }
    public int? WellDepth { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public Guid? CreateUserGuid { get; set; }
    public int? CreateUserID { get; set; }
    public string CreateUserEmail { get; set; }
    public int? FairyshrimpWellID { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}