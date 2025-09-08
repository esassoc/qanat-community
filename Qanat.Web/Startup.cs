using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Linq;

namespace Qanat.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; set; }

        public Startup(IWebHostEnvironment environment)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding response compression which greatly reduces size of static content delivery.
            // https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-5.0
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                // Appending any extra MIME types to compress.
                // Default list is here: https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-5.0#mime-types-1
                // NOTE: It's not recommended to compress images or other binary files
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "image/svg+xml"
                });
                options.EnableForHttps = true; 
            });
            var logger = GetSerilogLogger();
            services.AddSingleton(logger);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime, Serilog.ILogger logger)
        {
            loggerFactory.AddSerilog(logger);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var options = new RewriteOptions().AddRedirectToHttps(301, 9001);
                app.UseRewriter(options);
            }
            
            app.Use(async (context, next) =>
            {
                if (context.Request.Host.Host.Contains("waterplatform.edf.org", StringComparison.OrdinalIgnoreCase))
                {
                    // Redirect to the www domain
                    var wwwUri = new UriBuilder
                    {
                        Scheme = context.Request.Scheme,
                        Host = "groundwateraccounting.org",
                        Path = context.Request.Path,
                        Query = context.Request.QueryString.ToString()
                    };
                    context.Response.Redirect(wwwUri.Uri.ToString(), permanent: true);
                    return;
                }
                await next();

                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseResponseCompression();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }

        private Serilog.ILogger GetSerilogLogger()
        {
            var serilogLogger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration);

            return serilogLogger.CreateLogger();
        }
    }
    
}
