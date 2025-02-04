namespace Qanat.Models.DataTransferObjects;

public class ParcelHistoryDto
{
    public int ParcelHistoryID { get; set; }
    public int GeographyID { get; set; }
    public int ParcelID { get; set; }
    public int EffectiveYear { get; set; }
    public DateTime UpdateDate { get; set; }
    public int UpdateUserID { get; set; }
    public string UpdateUserFullName { get; set; }
    public decimal ParcelArea { get; set; }
    public string OwnerName { get; set; }
    public string OwnerAddress { get; set; }
    public int ParcelStatusID { get; set; }
    public bool IsReviewed { get; set; }
    public bool IsManualOverride { get; set; }
    public DateTime? ReviewDate { get; set; }
    public int? WaterAccountID { get; set; }
    public WaterAccountDisplayDto WaterAccount { get; set; }
    public ParcelStatusSimpleDto ParcelStatus { get; set; }
}