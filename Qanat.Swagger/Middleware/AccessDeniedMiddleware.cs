using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Qanat.Swagger.Middleware;

public class AccessDeniedMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            context.Response.ContentType = "application/json";
            var error = new { message = "Your API Key does not have access to this API." };
            var json = JsonSerializer.Serialize(error);
            await context.Response.WriteAsync(json);
        }
    }
}