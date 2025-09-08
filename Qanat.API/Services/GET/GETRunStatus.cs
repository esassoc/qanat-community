namespace Qanat.API.Services.GET;

public class GETRunStatus
{
    public int RunStatusID { get; set; }
    public string RunStatusName { get; set; }
    public string RunStatusDisplayName { get; set; }
    public string RunStatusColor { get; set; }
    public bool IsTerminal { get; set; }
}