namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationGridRowDto
{
    public int WellRegistrationID { get; set; }
    public int GeographyID { get; set; }
    public int? WellID { get; set; }
    public string WellName { get; set; }
    public int WellRegistrationStatusID { get; set; }
    public int? ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string CreateUserName { get; set; }
    public string CreateUserEmail { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<WellRegistrationGridRowIrrigatedParcelDto> IrrigatedParcels { get; set; }
    public string LandownerName { get; set; }
    public string LandownerBusinessName { get; set; }
    public string LandownerStreetAddress { get; set; }
    public string LandownerCity { get; set; }
    public string LandownerState { get; set; }
    public string LandownerZipCode { get; set; }
    public string LandownerPhone { get; set; }
    public string LandownerEmail { get; set; }
    public string OwnerOperatorName { get; set; }
    public string OwnerOperatorBusinessName { get; set; }
    public string OwnerOperatorStreetAddress { get; set; }
    public string OwnerOperatorCity { get; set; }
    public string OwnerOperatorState { get; set; }
    public string OwnerOperatorZipCode { get; set; }
    public string OwnerOperatorPhone { get; set; }
    public string OwnerOperatorEmail { get; set; }
    public string StateWCRNumber { get; set; }
    public string CountyWellPermitNumber { get; set; }
    public DateOnly? DateDrilled { get; set; }
    public bool AgriculturalWaterUse { get; set; }
    public string AgriculturalWaterUseDescription { get; set; }
    public bool StockWateringWaterUse { get; set; }
    public string StockWateringWaterUseDescription { get; set; }
    public bool DomesticWaterUse { get; set; }
    public string DomesticWaterUseDescription { get; set; }
    public bool PublicMunicipalWaterUse { get; set; }
    public string PublicMunicipalWaterUseDescription { get; set; }
    public bool PrivateMunicipalWaterUse { get; set; }
    public string PrivateMunicipalWaterUseDescription { get; set; }
    public bool OtherWaterUse { get; set; }
    public string OtherWaterUseDescription { get; set; }
    public WellRegistrationMetadatumSimpleDto WellRegistrationMetadatum { get; set; }

    public class WellRegistrationGridRowIrrigatedParcelDto
    {
        public string ParcelNumber { get; set; }
        public int ParcelID { get; set; }
    }
}