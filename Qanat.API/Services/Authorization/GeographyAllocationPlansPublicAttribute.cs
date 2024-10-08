using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services.Authorization;

public class GeographyAllocationPlansPublicAttribute : ConditionalAuthorizationAttribute
{

    public GeographyAllocationPlansPublicAttribute()
    {
    }

    public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
    {
        if (userDto == null && !hierarchyContext.GeographyDto.AllocationPlansVisibleToPublic)
        {
            return false;
        }
        return true;
    }

    public override string InvalidPermissionMessage()
    {
        return $"You do not have access to this page.";
    }
}