using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.Swagger.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class GeographyAccessAttribute(string geographyIDParamName = "geographyID") : Attribute, IAsyncActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = 1; // Runs after NoAccessBlock

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue(geographyIDParamName, out var geographyIDObj) || geographyIDObj is not int geographyID)
            {
                context.Result = new BadRequestObjectResult($"Missing or invalid '{geographyIDParamName}' parameter.");
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetService(typeof(QanatDbContext)) as QanatDbContext;
            var userID = context.HttpContext.User.GetUserID();
            if (userID == null || dbContext == null)
            {
                context.Result = new NotFoundResult();
                return;
            }
            var user = await dbContext.Users.FindAsync(userID);
            if (user == null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            // Permissions check first
            if (user.RoleID == (int)RoleEnum.SystemAdmin || dbContext.GeographyUsers.Any(x => x.UserID == user.UserID && x.GeographyID == geographyID))
            {
                // Geography existence check only for authorized users
                var geographyExists = await dbContext.Geographies.FindAsync(geographyID) != null;
                if (!geographyExists)
                {
                    context.Result = new NotFoundResult();
                    return;
                }
                await next();
                return;
            }
            context.Result = new NotFoundResult();
        }
    }
}
