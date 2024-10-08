using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Helpers;

namespace Qanat.API.Services
{
    public static class UserContext
    {
        public static UserDto GetUserFromHttpContext(QanatDbContext dbContext, HttpContext httpContext)
        {

            var claimsPrincipal = httpContext.User;
            if (!claimsPrincipal.Claims.Any())
            {
                return null;
            }

            if (claimsPrincipal.Claims.All(c => c.Type != ClaimsConstants.Sub) || claimsPrincipal.Claims.Any(c => c.Type == ClaimsConstants.IsClient)) // No claims or a Client Token
            {
                if (claimsPrincipal.Claims.All(c => c.Type != ClaimsConstants.ClientID))
                {
                    return null;
                }

                var clientID = Guid.Parse(claimsPrincipal.Claims.Single(c => c.Type == ClaimsConstants.ClientID).Value);
                var clientUser = Users.GetByUserGuid(dbContext, clientID);
                return clientUser;
            }

            var userGuid = Guid.Parse(claimsPrincipal.Claims.Single(c => c.Type == ClaimsConstants.Sub).Value);
            var keystoneUser = Users.GetByUserGuid(dbContext, userGuid);

            keystoneUser = ImpersonationService.RetrieveImpersonatedUserIfImpersonating(dbContext, keystoneUser);

            return keystoneUser;
        }
    }
}