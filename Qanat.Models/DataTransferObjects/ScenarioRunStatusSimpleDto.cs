namespace Qanat.Models.DataTransferObjects
{
    public class ScenarioRunStatusSimpleDto
    {
        public int ScenarioRunStatusID { get; set; }
        public string ScenarioRunStatusName { get; set; }
        public string ScenarioRunStatusDisplayName { get; set; }
        public int? GETRunStatusID { get; set; }
        public bool IsTerminal { get; set; }
    }
}