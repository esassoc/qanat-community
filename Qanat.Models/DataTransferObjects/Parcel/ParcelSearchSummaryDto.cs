namespace Qanat.Models.DataTransferObjects;

public class ParcelSearchSummaryDto
{
    public int TotalResults { get; set; }
    public List<ParcelSearchResultWithMatchedFieldsDto> ParcelSearchResults { get; set; }
}

public class ParcelSearchResultWithMatchedFieldsDto
{
    public ParcelSearchResultDto Parcel { get; set; }
    public Dictionary<ParcelSearchMatchEnum, bool> MatchedFields { get; set; }
}

public class ParcelSearchResultDto
{
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public int? WaterAccountID { get; set; }
    public int? WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public string ContactName { get; set; }
    public string ContactAddress { get; set; } 
    public string WaterAccountNameAndNumber { get; set; }
}

public enum ParcelSearchMatchEnum
{
    APN = 1,
    ContactAddress = 2,
    ContactName = 3,
    WaterAccountName = 4,
    WaterAccountNumber = 5,
}