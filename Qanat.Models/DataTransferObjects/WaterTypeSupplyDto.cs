namespace Qanat.Models.DataTransferObjects;

public class WaterTypeSupplyDto
{
    public int WaterTypeID { get; set; }
    public string WaterTypeName { get; set; }
    public int SortOrder { get; set; }
    public string WaterTypeColor { get; set; }
    public decimal? TotalSupply { get; set; }
    public decimal? TotalSupplyDepth { get; set; }
}