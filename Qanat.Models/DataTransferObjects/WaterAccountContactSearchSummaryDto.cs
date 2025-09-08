namespace Qanat.Models.DataTransferObjects;

public class WaterAccountContactSearchSummaryDto
{
    public int TotalResults { get; set; }
    public List<WaterAccountContactSearchResultWithMatchedFieldsDto> SearchResults { get; set; }
}

public class WaterAccountContactSearchResultWithMatchedFieldsDto
{
    public WaterAccountContactSearchResultDto WaterAccountContact { get; set; }
    public Dictionary<WaterAccountContactSearchMatchEnum, bool> MatchedFields { get; set; }
}

public class WaterAccountContactSearchResultDto
{
    public int WaterAccountContactID { get; set; }
    public string ContactName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }
    public string FullAddress { get; set; }
}

public enum WaterAccountContactSearchMatchEnum
{
    ContactName = 1,
    Email = 2,
    PhoneNumber = 3,
    FullAddress = 4
}