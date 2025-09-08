using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Qanat.API.Services.Filters;

public class UseMethodNameAsOperationIdFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var api in context.ApiDescriptions)
        {
            var path = "/" + api.RelativePath.TrimEnd('/');
            if (!swaggerDoc.Paths.TryGetValue(path, out var pathItem)) continue;

            var method = pathItem.Operations.FirstOrDefault(op =>
                op.Key.ToString().Equals(api.HttpMethod, StringComparison.OrdinalIgnoreCase));
            if (method.Value != null)
            {
                var controller = api.ActionDescriptor.RouteValues["controller"];
                var action = api.ActionDescriptor.RouteValues["action"];
                method.Value.OperationId = $"{action}_{controller}";
            }
        }
    }
}