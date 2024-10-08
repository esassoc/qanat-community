namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationDetailedDto
{
    public int WellRegistrationID { get; set; }
    public int GeographyID { get; set; }
    public GeographyDisplayDto Geography { get; set; }
    public int? WellID { get; set; }
    public string WellName { get; set; }
    public int WellStatusRegistrationID { get; set; }
    public WellRegistrationStatusSimpleDto WellRegistrationStatus { get; set; }
    public int? ParcelID { get; set; }
    public ParcelDisplayDto Parcel { get; set; }
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
    public List<ParcelDisplayDto> IrrigatedParcels { get; set; }
    public WellRegistrationContactWithStateDto LandownerContact { get; set; }
    public WellRegistrationContactWithStateDto OwnerOperatorContact { get; set; }
    public WellRegistrationMetadatumSimpleDto WellRegistrationMetadatum { get; set; }
    public List<WellRegistrationWaterUseDisplayDto> WellRegistrationWaterUses { get; set; }
    public List<WellRegistrationFileResourceDto> WellRegistrationFileResources { get; set; }
}