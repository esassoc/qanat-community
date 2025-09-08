//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioPlannerRole]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ScenarioPlannerRoleExtensionMethods
    {
        public static ScenarioPlannerRoleSimpleDto AsSimpleDto(this ScenarioPlannerRole scenarioPlannerRole)
        {
            var dto = new ScenarioPlannerRoleSimpleDto()
            {
                ScenarioPlannerRoleID = scenarioPlannerRole.ScenarioPlannerRoleID,
                ScenarioPlannerRoleName = scenarioPlannerRole.ScenarioPlannerRoleName,
                ScenarioPlannerRoleDisplayName = scenarioPlannerRole.ScenarioPlannerRoleDisplayName,
                ScenarioPlannerRoleDescription = scenarioPlannerRole.ScenarioPlannerRoleDescription,
                SortOrder = scenarioPlannerRole.SortOrder,
                Rights = scenarioPlannerRole.Rights,
                Flags = scenarioPlannerRole.Flags
            };
            return dto;
        }
    }
}