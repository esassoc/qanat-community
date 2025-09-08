namespace Qanat.Models.DataTransferObjects;

public class StatementBatchWaterAccountDto
{
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public string ContactName { get; set; }
    public string FullAddress { get; set; }
    public Guid? FileResourceGuid { get; set; }
}