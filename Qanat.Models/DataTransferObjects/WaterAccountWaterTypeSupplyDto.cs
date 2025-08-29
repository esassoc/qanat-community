namespace Qanat.Models.DataTransferObjects;

public class WaterAccountWaterTypeSupplyDto
{
    public int WaterAccountID { get; set; }
    public int WaterTypeID { get; set; }
    public string WaterTypeName { get; set; }
    public decimal? TotalSupply { get; set; }
    public int SortOrder { get; set; }
}