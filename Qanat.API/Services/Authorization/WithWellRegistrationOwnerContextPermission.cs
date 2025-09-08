using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services.Authorization;

public class WithWellRegistrationOwnerContextPermission : BaseAuthorizationAttribute
{
    public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
    {
        var isOwner = hierarchyContext.WellRegistrationDto.CreateUserGuid == userDto.UserGuid || hierarchyContext.WellRegistrationDto.CreateUserID == userDto.UserID;
        var hasGeographyPermission = PermissionChecker.HasGeographyFlag(userDto, FlagEnum.CanReviewWells, true, hierarchyContext);
        var hasGlobalPermission = PermissionChecker.HasFlag(userDto, FlagEnum.CanReviewWells, true);
        return isOwner || hasGeographyPermission || hasGlobalPermission;
    }

    public override string InvalidPermissionMessage()
    {
        return $"Your user doesn't have rights to edit this Well Registration";
    }
}