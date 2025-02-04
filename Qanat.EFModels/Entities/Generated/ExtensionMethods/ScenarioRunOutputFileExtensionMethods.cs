//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRunOutputFile]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ScenarioRunOutputFileExtensionMethods
    {
        public static ScenarioRunOutputFileSimpleDto AsSimpleDto(this ScenarioRunOutputFile scenarioRunOutputFile)
        {
            var dto = new ScenarioRunOutputFileSimpleDto()
            {
                ScenarioRunOutputFileID = scenarioRunOutputFile.ScenarioRunOutputFileID,
                ScenarioRunOutputFileTypeID = scenarioRunOutputFile.ScenarioRunOutputFileTypeID,
                ScenarioRunID = scenarioRunOutputFile.ScenarioRunID,
                FileResourceID = scenarioRunOutputFile.FileResourceID
            };
            return dto;
        }
    }
}