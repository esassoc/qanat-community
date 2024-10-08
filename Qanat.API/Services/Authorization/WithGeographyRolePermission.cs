using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Services.Authorization;

public class WithGeographyRolePermission : BaseAuthorizationAttribute
{
    private readonly PermissionEnum _permission;
    private readonly RightsEnum _rights;

    public WithGeographyRolePermission(PermissionEnum permission, RightsEnum rights)
    {
        _permission = permission;
        _rights = rights;
    }

    public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
    {
        return HasPermission(userDto, hierarchyContext);
    }

    public bool HasPermission(UserDto userDto, HierarchyContext hierarchyContext)
    {
        var hasPermission = PermissionChecker.HasPermission(userDto, _permission, _rights);
        var hasGeographyPermission =
            PermissionChecker.HasGeographyPermission(userDto, _permission, _rights, hierarchyContext);
        return hasPermission
               || hasGeographyPermission;
    }

    public override string InvalidPermissionMessage()
    {
        return $"GeographyRole - {_permission} - {_rights}";
    }
}