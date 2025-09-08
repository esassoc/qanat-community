//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRunStatus]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ScenarioRunStatusExtensionMethods
    {
        public static ScenarioRunStatusSimpleDto AsSimpleDto(this ScenarioRunStatus scenarioRunStatus)
        {
            var dto = new ScenarioRunStatusSimpleDto()
            {
                ScenarioRunStatusID = scenarioRunStatus.ScenarioRunStatusID,
                ScenarioRunStatusName = scenarioRunStatus.ScenarioRunStatusName,
                ScenarioRunStatusDisplayName = scenarioRunStatus.ScenarioRunStatusDisplayName,
                GETRunStatusID = scenarioRunStatus.GETRunStatusID,
                IsTerminal = scenarioRunStatus.IsTerminal
            };
            return dto;
        }
    }
}