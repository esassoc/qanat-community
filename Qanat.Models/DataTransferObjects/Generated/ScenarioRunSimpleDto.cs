//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRun]

namespace Qanat.Models.DataTransferObjects
{
    public partial class ScenarioRunSimpleDto
    {
        public int ScenarioRunID { get; set; }
        public int ScenarioRunStatusID { get; set; }
        public int ModelID { get; set; }
        public int ScenarioID { get; set; }
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? GETRunID { get; set; }
        public string GETErrorMessage { get; set; }
        public string ActionName { get; set; }
    }
}