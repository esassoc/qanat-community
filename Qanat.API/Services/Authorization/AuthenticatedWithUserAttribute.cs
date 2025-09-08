using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services.Authorization
{
    public class AuthenticatedWithUserAttribute : BaseAuthorizationAttribute
    {
        public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
        {
            return userDto != null;
        }

        public override string InvalidPermissionMessage()
        {
            return "Authentication required";
        }
    }
}