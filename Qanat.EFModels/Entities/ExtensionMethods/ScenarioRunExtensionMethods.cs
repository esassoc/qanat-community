using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class ScenarioRunExtensionMethods
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

        public static ScenarioRunDto AsDto(this ScenarioRun scenarioRun)
        {
            var scenarioRunDto = new ScenarioRunDto()
            {
                ScenarioRunID = scenarioRun.ScenarioRunID,
                ScenarioRunStatus = scenarioRun.ScenarioRunStatus.AsSimpleDto(),
                Model = scenarioRun.Model.AsSimpleDto(),
                Scenario = scenarioRun.Scenario.AsSimpleDto(),
                User = scenarioRun.User.AsUserDto(),
                CreateDate = scenarioRun.CreateDate,
                LastUpdateDate = scenarioRun.LastUpdateDate,
                GETRunID = scenarioRun.GETRunID,
                GETErrorMessage = scenarioRun.GETErrorMessage,
                ActionName = scenarioRun.ActionName,
                RunName = scenarioRun.ActionName ?? scenarioRun.ActionNameForGETEngine
            };

            return scenarioRunDto;
        }
    }
}