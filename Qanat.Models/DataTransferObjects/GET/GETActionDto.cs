namespace Qanat.Models.DataTransferObjects;

public partial class GETActionDto
{
    public int GETActionID { get; set; }
    public GETActionStatusSimpleDto GETActionStatus { get; set; }
    public ModelSimpleDto Model { get; set; }
    public ScenarioSimpleDto Scenario { get; set; }
    public UserDto User { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public int? GETRunID { get; set; }
    public string GETErrorMessage { get; set; }
    public string ActionName { get; set; }
    public string RunName { get; set; }
}

public partial class ScenarioRunDto
{
    public int ScenarioRunID { get; set; }
    public ScenarioRunStatusSimpleDto ScenarioRunStatus { get; set; }
    public ModelSimpleDto Model { get; set; }
    public ScenarioSimpleDto Scenario { get; set; }
    public UserDto User { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public int? GETRunID { get; set; }
    public string GETErrorMessage { get; set; }
    public string ActionName { get; set; }
    public string RunName { get; set; }
}