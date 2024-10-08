using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Helpers;

namespace Qanat.API.Services.Authorization
{
    public class StopImpersonationAuthorizationAttribute : BaseAuthorizationAttribute
    {
        public StopImpersonationAuthorizationAttribute()
        {
        }

        public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
        {
            var claimsPrincipal = context.HttpContext.User;
            if (userDto != null && claimsPrincipal != null && claimsPrincipal.Claims.Any())
            {
                var dbContext = context.HttpContext.RequestServices.GetService(typeof(QanatDbContext)) as QanatDbContext;
                if (dbContext == null)
                {
                    return false;
                }

                var subClaim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.Sub);
                if (subClaim == null)
                {
                    return false;
                }

                var claimsGlobalID = Guid.Parse(subClaim.Value);
                var claimsUser = Users.GetByUserGuid(dbContext, claimsGlobalID);

                if (claimsUser != null)
                {
                    return userDto.UserGuid == claimsUser.ImpersonatedUserGuid;
                }
            }
            return false;
        }

        public override string InvalidPermissionMessage()
        {
            return "Invalid Permissions.";
        }
    }
}