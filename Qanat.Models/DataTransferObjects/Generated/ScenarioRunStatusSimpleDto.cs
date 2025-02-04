//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRunStatus]

namespace Qanat.Models.DataTransferObjects
{
    public partial class ScenarioRunStatusSimpleDto
    {
        public int ScenarioRunStatusID { get; set; }
        public string ScenarioRunStatusName { get; set; }
        public string ScenarioRunStatusDisplayName { get; set; }
        public int? GETRunStatusID { get; set; }
        public bool IsTerminal { get; set; }
    }
}