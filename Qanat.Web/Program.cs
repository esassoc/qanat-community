using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Qanat.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => { logging.ClearProviders(); })
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); // Same as env.IsDevelopment()

                    options.Listen(IPAddress.Any, 80);
                    // 1/23 CG & MK - This is done so that Azure wont load the cert, it will only be used locally.
                    if (env == Microsoft.Extensions.Hosting.Environments.Development)
                    {
                        options.Listen(IPAddress.Any, 443, configure => { configure.UseHttps(new X509Certificate2("dev_cert.pfx", "password#1")); });
                    }
                })
                .Build();
            return host;
        }
    }
}
