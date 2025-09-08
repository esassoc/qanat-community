using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services.Authorization;

public class WithGeographyRoleFlagAttribute : BaseAuthorizationAttribute
{
    private readonly FlagEnum _flag;
    private readonly bool _value;
    private readonly bool _forAnyGeography;

    public WithGeographyRoleFlagAttribute(FlagEnum flag, bool value = true, bool forAnyGeography = false)
    {
        _flag = flag;
        _value = value;
        _forAnyGeography = forAnyGeography;
    }

    public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
    {
        var hasGeographyFlag = PermissionChecker.HasGeographyFlag(userDto, _flag, _value, hierarchyContext, _forAnyGeography);
        var hasFlag = PermissionChecker.HasFlag(userDto, _flag, _value);

        return hasFlag
               || hasGeographyFlag;
    }

    public override string InvalidPermissionMessage()
    {
        return $"Flags - {_flag}";
    }
}