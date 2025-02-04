using Newtonsoft.Json;

namespace Qanat.Models.DataTransferObjects;

public class ReportingPeriodDto
{
    public int ReportingPeriodID { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool ReadyForAccountHolders { get; set; }

    public bool IsDefaultReportingPeriod => Geography?.DefaultReportingPeriodID == ReportingPeriodID;

    public DateTime CreateDate { get; set; }
    public UserSimpleDto CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public UserSimpleDto UpdateUser { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [JsonIgnore]
    public GeographySimpleDto Geography { get; set; }
}