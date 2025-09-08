using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Services.Authorization
{
    public class WithRolePermissionAttribute : BaseAuthorizationAttribute
    {
        private readonly PermissionEnum _permission;
        private readonly RightsEnum _rights;

        public WithRolePermissionAttribute(PermissionEnum permission, RightsEnum rights)
        {
            _permission = permission;
            _rights = rights;
        }

        public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
        {
            return HasPermission(userDto);
        }

        public bool HasPermission(UserDto userDto)
        {
            return PermissionChecker.HasPermission(userDto, _permission, _rights);
        }

        public override string InvalidPermissionMessage()
        {
            return $"Role - {_permission} - {_rights}";
        }
    }
}