namespace Qanat.Models.DataTransferObjects;

public class WaterAccountSearchResultWithMatchedFieldsDto
{
    public WaterAccountSearchResultDto WaterAccount { get; set; }
    public Dictionary<WaterAccountSearchMatchEnum,bool> MatchedFields { get; set; }
}