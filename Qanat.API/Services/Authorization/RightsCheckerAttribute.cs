using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RightsCheckerAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var dbContext = context.HttpContext.RequestServices.GetService(typeof(QanatDbContext)) as QanatDbContext;
        var hierarchyContext = context.HttpContext.RequestServices.GetService(typeof(HierarchyContext)) as HierarchyContext;
        if (dbContext == null)
        {
            throw new ApplicationException("Could not find injected DB Context. WithRightsAttribute.cs needs your help!");
        }

        var user = UserContext.GetUserFromHttpContext(dbContext, context.HttpContext);

        if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            var allAuthorizationAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes().OfType<IAuthorizationAttribute>().ToList();
                
            var authAttributes = allAuthorizationAttributes.Where(x => x.GetType().IsSubclassOf(typeof(BaseAuthorizationAttribute))).ToList();
            ProcessAuthorizationRights(context, user, authAttributes, hierarchyContext);

            var conditionalAuthAttributes = allAuthorizationAttributes.Where(x => x.GetType().IsSubclassOf(typeof(ConditionalAuthorizationAttribute))).ToList();
            ProcessAuthorizationRights(context, user, conditionalAuthAttributes, hierarchyContext, false);
        }
    }

    private static void ProcessAuthorizationRights(AuthorizationFilterContext context, UserDto user,
        List<IAuthorizationAttribute> withRightsAttributes, HierarchyContext hierarchyContext, bool requireUser = true)
    {
        if (!withRightsAttributes.Any())
        {
            return;
        }

        if (requireUser && user == null)
        {
            //If we don't have a user and we didn't bail out above due to no required rights, we know we are unauthorized.
            context.Result = new UnauthorizedResult();
            return;
        }

        foreach (var attribute in withRightsAttributes)
        {
            var withRightsAttribute = attribute;
            {
                var hasPermission = withRightsAttribute.HasPermission(user, context, hierarchyContext);
                if (hasPermission)
                {
                    return;
                }
            }
        }

        context.Result =
            new ObjectResult(
                $"User does not have permission to access this resource: \n{string.Join("\n OR \n", withRightsAttributes.Select(x => x.InvalidPermissionMessage()))}")
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
    }

}