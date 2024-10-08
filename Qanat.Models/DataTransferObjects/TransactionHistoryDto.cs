namespace Qanat.Models.DataTransferObjects;

public class TransactionHistoryDto
{
    public DateTime TransactionDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string CreateUserFullName { get; set; }
    public string WaterTypeName { get; set; }
    public decimal? TransactionVolume { get; set; }
    public double? TransactionDepth { get; set; }
    public string UploadedFileName { get; set; }
    public int AffectedParcelsCount { get; set; }
    public double AffectedAcresCount { get; set; }
}