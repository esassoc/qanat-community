using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class GETActionExtensionMethods
{
    public static GETActionDto AsGETActionDto(this GETAction gETAction)
    {
        var gETActionDto = new GETActionDto()
        {
            GETActionID = gETAction.GETActionID,
            GETActionStatus = gETAction.GETActionStatus.AsSimpleDto(),
            Model = gETAction.Model.AsSimpleDto(),
            Scenario = gETAction.Scenario.AsSimpleDto(),
            User = gETAction.User.AsUserDto(),
            CreateDate = gETAction.CreateDate,
            LastUpdateDate = gETAction.LastUpdateDate,
            GETRunID = gETAction.GETRunID,
            GETErrorMessage = gETAction.GETErrorMessage,
            ActionName = gETAction.ActionName,
            RunName = gETAction.ActionName ?? gETAction.ActionNameForGETEngine
        };
        return gETActionDto;
    }
}