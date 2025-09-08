using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class ScenarioRunExtensionMethods
{
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