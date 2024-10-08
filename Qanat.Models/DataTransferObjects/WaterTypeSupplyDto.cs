namespace Qanat.Models.DataTransferObjects;

public class WaterTypeSupplyDto
{
    public int WaterTypeID { get; set; }
    public string WaterTypeName { get; set; }
    public decimal? TotalSupply { get; set; }
}