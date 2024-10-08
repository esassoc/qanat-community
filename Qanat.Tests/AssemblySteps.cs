using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System;
using System.IO;
using Microsoft.SqlServer.Dac;

namespace Qanat.Tests;

[TestClass]
public static class AssemblySteps
{
    public static IConfigurationRoot Configuration => new ConfigurationBuilder().AddJsonFile(@"environment.json").Build();

    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext testContext)
    { 
        await SetupDatabase();
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
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
    }
}