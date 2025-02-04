//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRunFileResource]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ScenarioRunFileResourceExtensionMethods
    {
        public static ScenarioRunFileResourceSimpleDto AsSimpleDto(this ScenarioRunFileResource scenarioRunFileResource)
        {
            var dto = new ScenarioRunFileResourceSimpleDto()
            {
                ScenarioRunFileResourceID = scenarioRunFileResource.ScenarioRunFileResourceID,
                ScenarioRunID = scenarioRunFileResource.ScenarioRunID,
                FileResourceID = scenarioRunFileResource.FileResourceID
            };
            return dto;
        }
    }
}