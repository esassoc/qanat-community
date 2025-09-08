using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Services.Authorization
{
    public class WithScenarioPlannerRolePermissionAttribute(PermissionEnum permission, RightsEnum rights) : BaseAuthorizationAttribute
    {
        public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
        {
            return HasPermission(userDto);
        }

        public bool HasPermission(UserDto userDto)
        {
            return PermissionChecker.HasScenarioPlannerPermission(userDto, permission, rights);
        }

        public override string InvalidPermissionMessage()
        {
            return $"Scenario Planner Role - {permission} - {rights}";
        }
    }
}