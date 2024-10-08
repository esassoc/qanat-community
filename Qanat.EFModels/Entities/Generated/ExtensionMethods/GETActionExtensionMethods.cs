//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GETAction]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GETActionExtensionMethods
    {
        public static GETActionSimpleDto AsSimpleDto(this GETAction gETAction)
        {
            var dto = new GETActionSimpleDto()
            {
                GETActionID = gETAction.GETActionID,
                GETActionStatusID = gETAction.GETActionStatusID,
                ModelID = gETAction.ModelID,
                ScenarioID = gETAction.ScenarioID,
                UserID = gETAction.UserID,
                CreateDate = gETAction.CreateDate,
                LastUpdateDate = gETAction.LastUpdateDate,
                GETRunID = gETAction.GETRunID,
                GETErrorMessage = gETAction.GETErrorMessage,
                ActionName = gETAction.ActionName
            };
            return dto;
        }
    }
}