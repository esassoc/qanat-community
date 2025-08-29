using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using System;
using System.Threading.Tasks;

namespace Qanat.Swagger.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NoAccessBlockAttribute : Attribute, IAsyncActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = 0; // Runs first
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userID = context.HttpContext.User.GetUserID();
            var roleID = context.HttpContext.User.GetRoleID();
            if (userID == null || roleID == null || roleID == (int)RoleEnum.NoAccess)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
