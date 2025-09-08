//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRun]
namespace Qanat.EFModels.Entities
{
    public partial class ScenarioRun
    {
        public int PrimaryKey => ScenarioRunID;
        public ScenarioRunStatus ScenarioRunStatus => ScenarioRunStatus.AllLookupDictionary[ScenarioRunStatusID];
        public Model Model => Model.AllLookupDictionary[ModelID];
        public Scenario Scenario => Scenario.AllLookupDictionary[ScenarioID];

        public static class FieldLengths
        {
            public const int GETErrorMessage = 1000;
            public const int ActionName = 500;
        }
    }
}