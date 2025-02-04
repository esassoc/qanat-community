//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRunOutputFile]
namespace Qanat.EFModels.Entities
{
    public partial class ScenarioRunOutputFile
    {
        public int PrimaryKey => ScenarioRunOutputFileID;
        public ScenarioRunOutputFileType ScenarioRunOutputFileType => ScenarioRunOutputFileType.AllLookupDictionary[ScenarioRunOutputFileTypeID];

        public static class FieldLengths
        {

        }
    }
}