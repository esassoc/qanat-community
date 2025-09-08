namespace Qanat.Models.DataTransferObjects;

public class WaterAccountContactDto
{
    public int WaterAccountContactID { get; set; }
    public int GeographyID { get; set; }
    public string ContactName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }
    public string Address { get; set; }
    public string SecondaryAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string FullAddress { get; set; }
    public bool? PrefersPhysicalCommunication { get; set; }
    public bool AddressValidated { get; set; }
    public string AddressValidationJson { get; set; }
    public List<WaterAccountMinimalDto> WaterAccounts { get; set; }
}
