namespace Qanat.Models.DataTransferObjects;

public class StatementBatchDto
{
    public int StatementBatchID { get; set; }
    public string StatementBatchName { get; set; }
    public string StatementTemplateName { get; set; }
    public int ReportingPeriodYear { get; set; }
    public DateTime LastUpdated { get; set; }
    public string UpdateUserFullName { get; set; }
    public int NumberOfWaterAccounts { get; set; }
    public bool StatementsGenerated { get; set; }
}