using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services.Authorization;

public class WithRoleFlagAttribute : BaseAuthorizationAttribute
{
    private readonly FlagEnum _flag;
    private readonly bool _value;

    public WithRoleFlagAttribute(FlagEnum flag, bool value = true)
    {
        _flag = flag;
        _value = value;
    }

    public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
    {
        return PermissionChecker.HasFlag(userDto, _flag, _value);
    }

    public override string InvalidPermissionMessage()
    {
        return $"Flags - {_flag}";
    }
}