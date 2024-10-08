namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationContactWithStateDto
{
    public int WellRegistrationContactID { get; set; }
    public int WellRegistrationID { get; set; }
    public int WellRegistrationContactTypeID { get; set; }
    public string ContactName { get; set; }
    public string BusinessName { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public int StateID { get; set; }
    public string StateName { get; set; }
    public string ZipCode { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}