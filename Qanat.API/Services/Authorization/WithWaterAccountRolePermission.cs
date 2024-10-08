using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Services.Authorization;

public class WithWaterAccountRolePermission : BaseAuthorizationAttribute
{
    private readonly PermissionEnum _permission;
    private readonly RightsEnum _rights;

    public WithWaterAccountRolePermission(PermissionEnum permission, RightsEnum rights)
    {
        _permission = permission;
        _rights = rights;
    }

    public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
    {
        var hasPermission = PermissionChecker.HasPermission(userDto, _permission, _rights);
        var hasGeographyPermission = PermissionChecker.HasGeographyPermission(userDto, _permission, _rights, hierarchyContext);
        var hasWaterAccountPermission = PermissionChecker.HasWaterAccountPermission(userDto, _permission, _rights, hierarchyContext);
        return hasPermission 
               || hasGeographyPermission
               || hasWaterAccountPermission;
    }

    public override string InvalidPermissionMessage()
    {
        return $"WaterAccountRole - {_permission} - {_rights}";
    }
}