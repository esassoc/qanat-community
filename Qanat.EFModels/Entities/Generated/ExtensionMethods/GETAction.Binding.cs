//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GETAction]
namespace Qanat.EFModels.Entities
{
    public partial class GETAction
    {
        public int PrimaryKey => GETActionID;
        public GETActionStatus GETActionStatus => GETActionStatus.AllLookupDictionary[GETActionStatusID];
        public Model Model => Model.AllLookupDictionary[ModelID];
        public Scenario Scenario => Scenario.AllLookupDictionary[ScenarioID];

        public static class FieldLengths
        {
            public const int GETErrorMessage = 1000;
            public const int ActionName = 500;
        }
    }
}