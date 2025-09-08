//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRun]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ScenarioRunExtensionMethods
    {
        public static ScenarioRunSimpleDto AsSimpleDto(this ScenarioRun scenarioRun)
        {
            var dto = new ScenarioRunSimpleDto()
            {
                ScenarioRunID = scenarioRun.ScenarioRunID,
                ScenarioRunStatusID = scenarioRun.ScenarioRunStatusID,
                ModelID = scenarioRun.ModelID,
                ScenarioID = scenarioRun.ScenarioID,
                UserID = scenarioRun.UserID,
                CreateDate = scenarioRun.CreateDate,
                LastUpdateDate = scenarioRun.LastUpdateDate,
                GETRunID = scenarioRun.GETRunID,
                GETErrorMessage = scenarioRun.GETErrorMessage,
                ActionName = scenarioRun.ActionName
            };
            return dto;
        }
    }
}