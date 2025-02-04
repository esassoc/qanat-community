namespace Qanat.Models.DataTransferObjects;

public class WaterAccountSearchSummaryDto
{
    public int TotalResults { get; set; }
    public List<WaterAccountSearchResultWithMatchedFieldsDto> WaterAccountSearchResults { get; set; }
}