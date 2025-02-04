namespace Qanat.Models.DataTransferObjects;

public class ParcelActivityDto
{
    public int? ParcelID { get; set; }
    public DateTime EffectiveDate { get; set; }
    public int TransactionTypeID { get; set; }
    public int? WaterTypeID { get; set; }
    public decimal TransactionAmount { get; set; }
    public List<ParcelSupplyDetailDto> ParcelSupplies { get; set; }
    public string ParcelActivityKey => $"{EffectiveDate:yyyyMMdd}_{ParcelID}_{TransactionTypeID}_{(WaterTypeID ?? 0)}";
    public int? ParcelCount { get; set; }
    public double? ParcelArea { get; set; }
}