//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ModelScenario]
namespace Qanat.EFModels.Entities
{
    public partial class ModelScenario
    {
        public int PrimaryKey => ModelScenarioID;
        public Model Model => Model.AllLookupDictionary[ModelID];
        public Scenario Scenario => Scenario.AllLookupDictionary[ScenarioID];

        public static class FieldLengths
        {

        }
    }
}