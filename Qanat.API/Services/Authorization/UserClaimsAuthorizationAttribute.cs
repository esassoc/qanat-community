using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services.Authorization
{
    public class UserClaimsAuthorizationAttribute : BaseAuthorizationAttribute
    {
        public string GlobalID { get; set; }
        public UserClaimsAuthorizationAttribute()
        {
        }

        public override bool HasPermission(UserDto userDto, AuthorizationFilterContext context, HierarchyContext hierarchyContext)
        {
            if (context.RouteData.Values.ContainsKey("globalID"))
            {
                GlobalID = context.RouteData.Values["globalID"].ToString();

                var successfulParse = Guid.TryParse(GlobalID, out var claimsGlobalID);
                if (!successfulParse)
                {
                    return false;
                }

                if (userDto != null)
                {
                    if (userDto.UserGuid == claimsGlobalID)  // claims ID matches
                    {
                        return true;
                    }

                    // this code is needed to handle impersonation
                    var dbContext = context.HttpContext.RequestServices.GetService(typeof(QanatDbContext)) as QanatDbContext;
                    if (dbContext == null)
                    {
                        return false;
                    }

                    var claimsUser = Users.GetByUserGuid(dbContext, claimsGlobalID);
                    if (claimsUser == null)
                    {
                        return false;
                    }
                    return userDto.UserGuid == claimsUser.ImpersonatedUserGuid;  // being impersontated
                }
            }
            return false;
        }

        public override string InvalidPermissionMessage()
        {
            return $"User Claims required, Could not find a user with the globalID: '{GlobalID}'";
        }
    }
}