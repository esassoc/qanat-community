using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetTopologySuite.IO.Converters;
using Qanat.EFModels.Entities;
using Qanat.Swagger;
using Qanat.Swagger.Filters;
using Qanat.Swagger.Logging;
using Scalar.AspNetCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Qanat.Swagger.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddEnvironmentVariables()
    .AddJsonFile(builder.Configuration["SECRET_PATH"], optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: true);

// Logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(context.Configuration);
});

// Services
builder.Services.Configure<QanatSwaggerConfiguration>(builder.Configuration);
var configuration = builder.Configuration.Get<QanatSwaggerConfiguration>();

builder.Services.AddDbContext<QanatDbContext>(c =>
{
    c.UseSqlServer(configuration.DB_CONNECTION_STRING, x =>
    {
        x.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
        x.UseNetTopologySuite();
    });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory(false));
    options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddOpenApi(options =>
{
    options.AddScalarTransformers(); // Required for extensions to work

    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Groundwater Accounting Platform API",
            Version = "1.0",
            Description =
                "Before you start using the Groundwater Accounting Platform API, you will need to obtain an API key from the project team. The Groundwater Accounting Platform REST API provides resource-oriented urls to fetch data as JSON.",
            Contact = new OpenApiContact
            {
                Name = "Contact Us",
                Email = "info@groundwateraccounting.org"
            },
            License = new OpenApiLicense
            {
                Name = "License",
                Url = new Uri("https://groundwateraccounting.org/license")
            },
            TermsOfService = new Uri("https://groundwateraccounting.org/terms-of-service")
        };
        return Task.CompletedTask;
    });

    options.AddDocumentTransformer<ApiKeySecuritySchemeTransformer>();
});

// Add authentication for API key
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "ApiKeyScheme";
    options.DefaultChallengeScheme = "ApiKeyScheme";
})
.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", _ => { });

builder.Services.AddHealthChecks().AddDbContextCheck<QanatDbContext>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Middleware
app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
    opts.GetLevel = LogHelper.CustomGetLevel;
});
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(); // Use default API error handler
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(policy =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.WithExposedHeaders("WWW-Authenticate");
});
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AccessDeniedMiddleware>(); 
app.UseMiddleware<LogHelper>();
app.MapControllers();
app.MapHealthChecks("/healthz");

app.MapOpenApi();
app.MapScalarApiReference("/docs", options =>
{
    options.Title = "Groundwater Accounting Platform API";
    options.ShowSidebar = true;
    options.HideModels = true;
    //options.CustomCss = "* { font-family: 'Monaco'; }";

    options.AddPreferredSecuritySchemes("ApiKeyScheme"); // Make this the default auth method

    options.DefaultHttpClient =
        new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.R, ScalarClient.Httr);

    var scalarServers = new List<ScalarServer>();
    if (app.Environment.IsDevelopment())
    {
        scalarServers.Add(new ScalarServer("https://host.docker.internal:7610", "Local Sandbox"));
    }
    else if (app.Environment.IsStaging())
    {
        scalarServers.Add(new ScalarServer("https://api-qa.groundwateraccounting.org", "Staging"));
    }
    else if (app.Environment.IsProduction())
    {
        scalarServers.Add(new ScalarServer("https://api.groundwateraccounting.org", "Production"));
    };
    options.Servers = scalarServers;
});

app.Run();

internal sealed class ApiKeySecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.All(authScheme => authScheme.Name == "ApiKeyScheme"))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["ApiKeyScheme"] = new()
                {
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme",
                    In = ParameterLocation.Header,
                    Name = "X-API-KEY"
                }
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "ApiKeyScheme",
                            Type = ReferenceType.SecurityScheme
                            
                        }
                    }] = Array.Empty<string>()
                });
            }
        }
    }
}