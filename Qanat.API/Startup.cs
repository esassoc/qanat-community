using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using Qanat.API.Hangfire;
using Qanat.API.Services;
using Qanat.API.Services.Filters;
using Qanat.API.Services.GET;
using Qanat.API.Services.Middleware;
using Qanat.Common.Services.GDAL;
using Qanat.EFModels.Entities;
using SendGrid.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;
using LogHelper = Qanat.API.Services.Logging.LogHelper;
using System.Text;
using Qanat.API.Services.OpenET;

namespace Qanat.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private string _instrumentationKey;
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddNewtonsoftJson(opt =>
                {
                    if (!_environment.IsProduction())
                    {
                        opt.SerializerSettings.Formatting = Formatting.Indented;
                    }
                    opt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    var resolver = opt.SerializerSettings.ContractResolver;
                    if (resolver != null)
                    {
                        if (resolver is DefaultContractResolver defaultResolver)
                        {
                            defaultResolver.NamingStrategy = null;
                        }
                    }
                    
                }).AddJsonOptions(options =>
            {
                var scale = Math.Pow(10, 4);
                var geometryFactory = new GeometryFactory(new PrecisionModel(scale), 4326);
                options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory(geometryFactory, false));
            });
            
            services.Configure<QanatConfiguration>(Configuration);
            var qanatConfiguration = Configuration.Get<QanatConfiguration>();

            services.AddHttpClient<GETService>(c =>
            {
                c.BaseAddress = new Uri(qanatConfiguration.GETAPIBaseURL);
                c.Timeout = TimeSpan.FromMinutes(30);
                c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", qanatConfiguration.GETAPISubscriptionKey);
                //Allows us to follow a URL and get more information on why a request failed
                c.DefaultRequestHeaders.Add("Ocp-Apim-Trace", "true");
            });

            services.AddScoped(_ =>
            {
                //MK 8/29/2024 - Azure pipelines mangles the newlines in the private key need to have it be a base64 string and then replace the \\n with actual newlines. Probably a better way to solve this.
                var privateKeyBytes = Convert.FromBase64String(qanatConfiguration.GoogleCloud.private_key);
                var privateKey = Encoding.UTF8.GetString(privateKeyBytes);

                var googleCloudCredentials = (GoogleCloudConfiguration) qanatConfiguration.GoogleCloud.Clone();
                googleCloudCredentials.private_key = privateKey.Replace("\\n", "\n");

                var credentialsJSONString = JsonSerializer.Serialize(googleCloudCredentials);
                var googleServiceAccountCredentials = GoogleCredential.FromJson(credentialsJSONString).CreateScoped(DriveService.Scope.Drive);

                return new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = googleServiceAccountCredentials
                });
            });

            services.AddHttpClient<OpenETSyncService>(c =>
            {
                c.BaseAddress = new Uri(qanatConfiguration.OpenET.ApiBaseUrl);
                c.Timeout = TimeSpan.FromMinutes(30);
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(qanatConfiguration.OpenET.ApiKey);
            });

            services.AddScoped(c => new RasterProcessingService(c.GetService<QanatDbContext>(), c.GetService<FileService>()));

            services.AddHttpClient<MonitoringWellCNRAService>(c =>
            {
                c.BaseAddress = new Uri(qanatConfiguration.CNRAFeatureServerBaseUrl);
                c.Timeout = TimeSpan.FromMinutes(30);
            });

            services.AddHttpClient<YoloWellWRIDService>(c =>
            {
                c.BaseAddress = new Uri(qanatConfiguration.YoloWRIDAPIBaseUrl);
                c.Timeout = TimeSpan.FromMinutes(30);
            });

            services.AddHttpClient<GeographyGISBoundaryService>(c =>
            {
                c.BaseAddress = new Uri(qanatConfiguration.CNRAFeatureServerBaseUrl);
                c.Timeout = TimeSpan.FromMinutes(30);
            });

            services.AddHttpClient<GDALAPIService>(c =>
            {
                c.BaseAddress = new Uri(qanatConfiguration.GDALAPIBaseUrl);
                c.Timeout = TimeSpan.FromDays(1);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpClientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

                return httpClientHandler;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(options =>
                    {
                        Configuration.Bind("AzureAdB2C", options);

                        options.TokenValidationParameters.NameClaimType = "name";

                        var validAudiences = new List<string>() { qanatConfiguration.AzureAdB2C.ClientId };

                        if (!string.IsNullOrEmpty(qanatConfiguration.AdminClientFlowClientID))
                        {
                            validAudiences.Add(qanatConfiguration.AdminClientFlowClientID);
                        }

                        if (!string.IsNullOrEmpty(qanatConfiguration.InactiveClientFlowClientID))
                        {
                            validAudiences.Add(qanatConfiguration.InactiveClientFlowClientID);
                        }

                        options.TokenValidationParameters.ValidAudiences = validAudiences;
                    },
                    options => { Configuration.Bind("AzureAdB2C", options);});

            services.AddDbContext<QanatDbContext>(c =>
            {
                c.UseSqlServer(qanatConfiguration.DB_CONNECTION_STRING, x =>
                {
                    x.CommandTimeout((int)TimeSpan.FromMinutes(4).TotalSeconds);
                    x.UseNetTopologySuite();
                });
            });

            services.AddSingleton(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSendGrid(options => { options.ApiKey = qanatConfiguration.SendGridApiKey; });

            services.AddSingleton<SitkaSmtpClientService>();

            services.AddScoped(s => s.GetService<IHttpContextAccessor>().HttpContext);
            services.AddScoped(s => UserContext.GetUserFromHttpContext(s.GetService<QanatDbContext>(), s.GetService<IHttpContextAccessor>().HttpContext));
            services.AddScoped<ImpersonationService>();
            services.AddScoped<FileService>();
            services.AddScoped<IAzureStorage, AzureStorage>();
            services.AddScoped<HierarchyContext>();
            services.AddControllers();

            #region Hangfire
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(qanatConfiguration.DB_CONNECTION_STRING, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            services.AddHangfireServer(x =>
            {
                x.WorkerCount = 1;
            });
            #endregion

            #region Swagger
            // Base swagger services
            services.AddSwaggerGen(options =>
            {
                // extra options here if you wanted
                options.UseAllOfForInheritance(); // this helps inherited types to also be generated on the typescript side
            });
            services.AddSwaggerGenNewtonsoftSupport();
            #endregion

            services.AddHealthChecks().AddDbContextCheck<QanatDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                opts.GetLevel = LogHelper.CustomGetLevel;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseCors(policy =>
            {
                //TODO: don't allow all origins
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.WithExposedHeaders("WWW-Authenticate");
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<EntityNotFoundMiddleware>();
            app.UseMiddleware<LogHelper>();


            #region Hangfire
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorizationFilter(Configuration) }
            });

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            HangfireJobScheduler.ScheduleRecurringJobs();
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz");
            });

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            
        }
        private void OnShutdown()
        {
            Thread.Sleep(1000);
        }
    }
}
