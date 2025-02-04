//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRunOutputFileType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ScenarioRunOutputFileTypeExtensionMethods
    {
        public static ScenarioRunOutputFileTypeSimpleDto AsSimpleDto(this ScenarioRunOutputFileType scenarioRunOutputFileType)
        {
            var dto = new ScenarioRunOutputFileTypeSimpleDto()
            {
                ScenarioRunOutputFileTypeID = scenarioRunOutputFileType.ScenarioRunOutputFileTypeID,
                ScenarioRunOutputFileTypeName = scenarioRunOutputFileType.ScenarioRunOutputFileTypeName,
                ScenarioRunOutputFileTypeExtension = scenarioRunOutputFileType.ScenarioRunOutputFileTypeExtension
            };
            return dto;
        }
    }
}