using Hangfire;
using Hangfire.Logging;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Dac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API;
using Qanat.API.Services;
using Qanat.EFModels.Entities;
using Qanat.Models.Security;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Qanat.Models.Helpers;
using VerifyTests;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Qanat.Tests;

[TestClass]
public static class AssemblySteps
{
    public static IConfigurationRoot Configuration => new ConfigurationBuilder()
        .AddJsonFile(@"environment.json")
        .Build();

    public static QanatDbContext QanatDbContext { get; set; }
    public static HttpClient AdminHttpClient { get; set; }
    public static HttpClient NormalHttpClient { get; set; }
    public static HttpClient UnauthorizedHttpClient { get; set; }


    public static JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = new PascalCaseNamingPolicy(),
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        WriteIndented = true,
        Converters = { new RightsConverter(), new UtcDateTimeConverter() }
    };

    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext testContext)
    {
        await SetupDatabase();
        SetupLogging();
        await SetupAPI();
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
    }

    private static async Task SetupDatabase()
    {
        var stopwatch = Stopwatch.StartNew();

        var dbCS = Configuration["sqlConnectionString"];
        var dbOptions = new DbContextOptionsBuilder<QanatDbContext>();
        dbOptions.UseSqlServer(dbCS, x => x.UseNetTopologySuite());
        var dbContext = new QanatDbContext(dbOptions.Options);

        var databaseName = "QanatDB";
        var bacpacFilePath = Configuration["bacpacFilePath"];
        var dacpacFilePath = Configuration["dacpacFilePath"];

        var restoreBuild = string.IsNullOrEmpty(Configuration["restoreBuildForTests"]) || Configuration["restoreBuildForTests"] == "True";
        var runRestoreBuild = restoreBuild && !string.IsNullOrEmpty(bacpacFilePath) && File.Exists(bacpacFilePath) && !string.IsNullOrEmpty(dacpacFilePath) && File.Exists(dacpacFilePath);
        if (runRestoreBuild)
        {
            var deleted = await dbContext.Database.EnsureDeletedAsync();

            var dacServices = new DacServices(dbCS);

            var bacpac = BacPackage.Load(bacpacFilePath);
            var importOptions = new DacImportOptions();
            dacServices.ImportBacpac(bacpac, databaseName, importOptions);

            var dacpac = DacPackage.Load(dacpacFilePath);
            var dacDeployOptions = new DacDeployOptions
            {
                BlockOnPossibleDataLoss = false
            };

            dacServices.Deploy(dacpac, databaseName, true, dacDeployOptions);
        }

        stopwatch.Stop();

        if (runRestoreBuild)
        {
            Console.WriteLine($"Test database restored and built successfully in {stopwatch.Elapsed.TotalSeconds} seconds.");
        }

        QanatDbContext = new QanatDbContext(dbCS);
    }

    private static void SetupLogging()
    {
        // Override Serilog's minimum level for DataProtection to suppress logs
        Environment.SetEnvironmentVariable("Serilog:MinimumLevel:Override:Microsoft.AspNetCore.DataProtection", "Fatal");
        Environment.SetEnvironmentVariable("Serilog:MinimumLevel:Override:Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager", "Fatal");

        // Alternatively, reconfigure Serilog directly:
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft.AspNetCore.DataProtection", LogEventLevel.Fatal)
            .MinimumLevel.Override("Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager", LogEventLevel.Fatal)
            .WriteTo.Console()
            .CreateLogger();
    }

    private static async Task SetupAPI()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddEnvironmentVariables();
                conf.Build();

                var inMemorySettings = new Dictionary<string, string>
                {
                    ["Logging:LogLevel:IsDefault"] = "Error",
                    ["Logging:LogLevel:Microsoft"] = "Error",
                    ["Logging:LogLevel:Microsoft.AspNetCore"] = "Error",
                    // Disable DataProtection logging completely
                    ["Logging:LogLevel:Microsoft.AspNetCore.DataProtection"] = "None",
                    ["Logging:LogLevel:Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"] = "None",
                    ["CNRAFeatureServerBaseUrl"] = "https://gis.water.ca.gov/",
                    ["GDALAPIBaseUrl"] = "http://host.docker.internal:7631",
                };

                conf.AddInMemoryCollection(inMemorySettings);
            });

            // Configure logging providers and delegate filter
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Error);
                logging.AddFilter((provider, category, logLevel) =>
                {
                    // Filter out any logs from DataProtection
                    if (category.StartsWith("Microsoft.AspNetCore.DataProtection"))
                    {
                        return false;
                    }

                    return logLevel >= LogLevel.Error;
                });
            });


            builder.ConfigureTestServices(services =>
            {
                services.Configure<QanatConfiguration>(Configuration);
                var qanatConfiguration = Configuration.Get<QanatConfiguration>();

                var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<QanatDbContext>));
                if (dbDescriptor != null)
                {
                    services.Remove(dbDescriptor);
                }

                services.AddScoped(s =>
                {
                    var dbContext = new QanatDbContext(Configuration["sqlConnectionString"]);
                    return dbContext;
                });

                var hangfireDescriptors = services
                    .Where(d => d.ServiceType.Assembly.FullName.Contains("Hangfire"))
                    .ToList();

                foreach (var descriptor in hangfireDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddHangfire(configuration => configuration
                                         .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                         .UseSimpleAssemblyNameTypeSerializer()
                                         .UseRecommendedSerializerSettings()
                                         .UseLogProvider(new NullLogProvider()) // disable Hangfire logging
                                         .UseSqlServerStorage(Configuration["sqlConnectionString"], new Hangfire.SqlServer.SqlServerStorageOptions
                                         {
                                             CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                             SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                             QueuePollInterval = TimeSpan.Zero,
                                             UseRecommendedIsolationLevel = true,
                                             DisableGlobalLocks = true
                                         }));

                services.AddHangfireServer(options =>
                {
                    options.WorkerCount = 1;
                });

                //Remove any existing Authentication & Authorization registrations
                var authDescriptors = services.Where(d => d.ServiceType == typeof(IAuthenticationSchemeProvider) ||
                                                          d.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>)).ToList();
                foreach (var descriptor in authDescriptors)
                {
                    services.Remove(descriptor);
                }

                var authorizationDescriptors = services.Where(d => d.ServiceType == typeof(IAuthorizationPolicyProvider)).ToList();
                foreach (var descriptor in authorizationDescriptors)
                {
                    services.Remove(descriptor);
                }

                //Re-add authentication with the correct Authority URL
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = Configuration["IdentityManagement_Authority"];
                        options.Audience = Configuration["IdentityManagement_Audience"];
                        options.TokenValidationParameters.ValidAudiences = new List<string>
                        {
                            Configuration["IdentityManagement_AdminClientFlowClientID"],
                            Configuration["IdentityManagement_NormalClientFlowClientID"],
                            Configuration["IdentityManagement_InactiveClientFlowClientID"]
                        };

                        // Force key refresh if validation fails
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                Console.WriteLine($"[JWT] Authentication failed: {context.Exception}");

                                if (context.Exception.Message.Contains("IDX10503")) // Signature key issue
                                {
                                    Console.WriteLine("[JWT] Attempting to refresh configuration...");
                                    var configurationManager = context.Options.ConfigurationManager;
                                    configurationManager!.RequestRefresh(); // Forces an immediate metadata refresh
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });

                services.AddAuthorization();

            });
        });

        var authorityURL = Configuration["IdentityManagement_URL"];
        var scope = Configuration["IdentityManagement_Scope"];
        var grantType = Configuration["IdentityManagement_GrantType"];

        var adminClientFlowClientIdentifier = Configuration["IdentityManagement_AdminClientFlowClientID"];
        var adminClientFlowClientSecret = Configuration["IdentityManagement_AdminClientFlowClientSecret"];

        var normalClientFlowClientIdentifier = Configuration["IdentityManagement_NormalClientFlowClientID"];
        var normalClientFlowClientSecret = Configuration["IdentityManagement_NormalClientFlowClientSecret"];

        var inactiveClientFlowClientIdentifier = Configuration["IdentityManagement_InactiveClientFlowClientID"];
        var inactiveClientFlowClientSecret = Configuration["IdentityManagement_InactiveClientFlowClientSecret"];

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(authorityURL!);

        var adminBearerToken = await GetB2CAccessToken(httpClient, adminClientFlowClientIdentifier, adminClientFlowClientSecret, scope, grantType);
        AdminHttpClient = webApplicationFactory.CreateClient();
        AdminHttpClient.SetBearerToken(adminBearerToken);
        AdminHttpClient.Timeout = TimeSpan.FromMinutes(3);

        var normalBearerToken = await GetB2CAccessToken(httpClient, normalClientFlowClientIdentifier, normalClientFlowClientSecret, scope, grantType);
        NormalHttpClient = webApplicationFactory.CreateClient();
        NormalHttpClient.SetBearerToken(normalBearerToken);
        NormalHttpClient.Timeout = TimeSpan.FromMinutes(3);

        var inactiveBearerToken = await GetB2CAccessToken(httpClient, inactiveClientFlowClientIdentifier, inactiveClientFlowClientSecret, scope, grantType);
        UnauthorizedHttpClient = webApplicationFactory.CreateClient();
        UnauthorizedHttpClient.SetBearerToken(inactiveBearerToken);
        UnauthorizedHttpClient.Timeout = TimeSpan.FromMinutes(3);

        //First request is always a bit slower, get that out of the way here.
        await AdminHttpClient.GetAsync("");
    }

    private static async Task<string> GetB2CAccessToken(HttpClient httpClient, string clientID, string clientSecret, string scope, string grantType)
    {
        var postValue = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientID),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("scope", scope),
            new KeyValuePair<string, string>("grant_type", grantType),
        });

        var httpResponse = await httpClient.PostAsync("", postValue);
        var responseContent = await httpResponse.Content.ReadAsStringAsync();
        var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<B2CTokenResponse>(responseContent);
        return tokenResponse?.access_token;
    }

    internal class B2CTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int not_before { get; set; }
        public int expires_in { get; set; }
        public int expires_on { get; set; }
        public string resource { get; set; }
    }

    internal class NullLogProvider : ILogProvider
    {
        public ILog GetLogger(string name) => new NullLogger();

        private class NullLogger : ILog
        {
            public bool Log(Hangfire.Logging.LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
            {
                return false;
            }
        }
    }

}

public class PascalCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        return char.ToUpper(name[0]) + name.Substring(1);
    }
}

public static class VerifyInitialization
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.UseUtf8NoBom();
        VerifierSettings.ScrubLinesWithReplace(line => line.Replace("\r\n", "\n"));
    }
}