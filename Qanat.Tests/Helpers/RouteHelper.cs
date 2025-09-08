using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Qanat.Tests.Helpers;

public static class RouteHelper
{
    public static string GetRouteTemplateFor(Type controllerType, MethodInfo methodInfo)
    {
        // Extract controller route
        var controllerRouteAttr = controllerType.GetCustomAttribute<RouteAttribute>();
        var controllerRoute = controllerRouteAttr?.Template ?? "";

        // Extract action route, a bit annoying because they don't have the same base class, or shared interface.
        var actionRouteAttr = methodInfo.GetCustomAttributes().FirstOrDefault(attr =>
            attr is RouteAttribute ||
            attr is HttpGetAttribute ||
            attr is HttpPostAttribute ||
            attr is HttpPutAttribute ||
            attr is HttpDeleteAttribute);

        if (actionRouteAttr == null)
        {
            throw new InvalidOperationException($"No route attribute found on {methodInfo.Name}");
        }

        var actionRoute = actionRouteAttr switch
        {
            RouteAttribute routeAttr => routeAttr.Template,
            HttpGetAttribute getAttr => getAttr.Template,
            HttpPostAttribute postAttr => postAttr.Template,
            HttpPutAttribute putAttr => putAttr.Template,
            HttpDeleteAttribute deleteAttr => deleteAttr.Template,
            _ => throw new InvalidOperationException("Unsupported route attribute type")
        };

        // Combine routes
        var fullRouteTemplate = $"{controllerRoute}/{actionRoute}".Trim('/');
        return fullRouteTemplate;
    }

    public static string GetRouteFor<TController>(Expression<Func<TController, object>> action)
    {
        var methodCall = action.Body as MethodCallExpression ?? throw new ArgumentException("Expression must be a method call", nameof(action));
        var methodInfo = methodCall.Method;

        var fullRoute = GetRouteTemplateFor(typeof(TController), methodInfo);

        // Extract and replace parameters dynamically
        var parameters = methodCall.Method.GetParameters();
        var argumentValues = methodCall.Arguments.Select(arg => Expression.Lambda(arg).Compile().DynamicInvoke()).ToArray();

        if (parameters.Length != argumentValues.Length)
        {
            throw new InvalidOperationException("Mismatch between method parameters and extracted values.");
        }

        var queryParameters = new List<string>();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var value = argumentValues[i];

            // Check if the parameter has a [FromQuery] attribute
            var fromQueryAttr = parameter.GetCustomAttribute<FromQueryAttribute>();
            if (fromQueryAttr != null)
            {
                if (value is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        queryParameters.Add($"{parameter.Name}={Uri.EscapeDataString(item?.ToString() ?? string.Empty)}");
                    }
                }
                else
                {
                    queryParameters.Add($"{parameter.Name}={Uri.EscapeDataString(value?.ToString() ?? string.Empty)}");
                }
            }
            else
            {
                var placeholder = $"{{{parameter.Name}}}";
                fullRoute = fullRoute.Replace(placeholder, value?.ToString());
            }
        }

        // Append query parameters to the route if any exist
        if (queryParameters.Any())
        {
            fullRoute += "?" + string.Join("&", queryParameters);
        }

        return fullRoute;
    }
}