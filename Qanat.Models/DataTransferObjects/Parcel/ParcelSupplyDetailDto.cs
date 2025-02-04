namespace Qanat.Models.DataTransferObjects;

public class ParcelSupplyDetailDto
{
    public int ParcelSupplyID { get; set; }
    public int GeographyID { get; set; }
    public int ParcelID { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal TransactionAmount { get; set; }
    public int? WaterTypeID { get; set; }
    public int? UserID { get; set; }
    public string UserComment { get; set; }
    public string UploadedFileName { get; set; }
    public ParcelMinimalDto Parcel { get; set; }
    public WaterTypeSimpleDto WaterType { get; set; }
}