using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MaxRev.Gdal.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSGeo.GDAL;
using Qanat.EFModels.Entities;
using System.Text.Json;
using Qanat.API.Services.OpenET;
using Qanat.Models.DataTransferObjects;
using System.Collections.Generic;

namespace Qanat.Tests.IntegrationTests.Raster;

[TestClass]
public class RasterTests
{
    private static QanatDbContext _dbContext;
    private static RasterProcessingService _rasterProcessing;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        var dbCS = AssemblySteps.Configuration["sqlConnectionString"];
        _dbContext = new QanatDbContext(dbCS);

        _rasterProcessing = new RasterProcessingService(_dbContext, null);

        GdalBase.ConfigureAll();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
    }

    [TestMethod]
    [DataRow(1, 1, 2023, 5, "2023-05-ET.tif")]
    public async Task CanGetRasterDataForUsageLocation(int geographyID, int waterMeasurementTypeID, int year, int month, string rasterFileName)
    {
        var rasterFilePath = $"../../../IntegrationTests/Raster/RasterFiles/{rasterFileName}";
        Assert.IsTrue(File.Exists(rasterFilePath));

        var rasterDataset = Gdal.Open(rasterFilePath, Access.GA_ReadOnly);
        Assert.IsNotNull(rasterDataset);

        var usageLocation = await _dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.UsageLocationGeometry)
            .Include(x => x.Geography).ThenInclude(x => x.GeographyBoundary)
            .Include(x => x.Geography).ThenInclude(x => x.ReportingPeriods)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID);

        var geography = usageLocation.Geography;

        var usageLocationDto = new UsageLocationSimpleDto()
        {
            UsageLocationID = usageLocation.UsageLocationID,
            Name = usageLocation.Name,
            Area = usageLocation.Area
        };

        var bufferedGSABoundary = geography.GeographyBoundary.GSABoundary.Buffer(Geographies.GSABoundaryBuffer).Envelope;
        var result = await _rasterProcessing.GetRasterValueForUsageLocation(geography.AsDto(), bufferedGSABoundary, rasterDataset, usageLocationDto);
        rasterDataset.Dispose();

        var oldResult = await _dbContext.WaterMeasurements.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geography.GeographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.ReportedDate.Year == year && x.ReportedDate.Month == month && x.UsageLocationID == usageLocation.UsageLocationID);

        result.OldValue = (double?)oldResult?.ReportedValueInNativeUnits;

        var prettyPrintedResultJSON = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(prettyPrintedResultJSON);

        Assert.IsTrue(string.IsNullOrEmpty(result.ErrorMessage));
        Assert.IsTrue(result.RasterValue.HasValue);
    }

    [TestMethod]
    [DataRow(1, 1, 2023, 5, "2023-05-ET.tif", 100)]
    [DataRow(7, 21, 2024, 6, "2024_Jun_EaTurGSA_ETa_mm_30m.tif", 100)] //MK 9/18/2024 -- Using the 30m as its smaller and I didn't want to commit the 10mb+ 10m raster... Need to iterate on this and potentially use AzureBlobStorage instead so we don't pollute the repo and upset Ray ;)
    public async Task CanGetRasterDataForRandomSampleOfGeographyUsageLocations(int geographyID, int waterMeasurementTypeID, int year, int month, string rasterFileName, int? take)
    {
        var rasterFilePath = $"../../../IntegrationTests/Raster/RasterFiles/{rasterFileName}";
        Assert.IsTrue(File.Exists(rasterFilePath));

        var waterMeasurements = await _dbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.ReportedDate.Year == year && x.ReportedDate.Month == month)
            .ToListAsync();

        var usageLocations = await _dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.UsageLocationGeometry)
            .Include(x => x.Parcel)
            .Where(x => x.GeographyID == geographyID && x.UsageLocationGeometry != null && x.UsageLocationGeometry.GeometryNative.IsValid)
            .OrderBy(x => Guid.NewGuid())
            .ToListAsync();

        var usageLocationsToSample = take.HasValue
            ? usageLocations.Take(take.Value)
            : usageLocations;

        var geography = usageLocations.First().Geography;

        var rasterFileBytes = await File.ReadAllBytesAsync(rasterFilePath);
        var usageLocationDtos = usageLocationsToSample.Select(x => new UsageLocationSimpleDto()
        {
            UsageLocationID = x.UsageLocationID,
            Name = x.Name,
            Area = x.Area
        }).ToList();

        var results = await _rasterProcessing.ProcessRasterBytesForUsageLocations(geography.AsDto(), usageLocationDtos, rasterFileBytes);

        foreach (var rasterProcessingResult in results)
        {
            var oldResult = waterMeasurements.FirstOrDefault(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.ReportedDate.Year == year && x.ReportedDate.Month == month && x.UsageLocationID == rasterProcessingResult.UsageLocationID);

            rasterProcessingResult.OldValue = (double?)oldResult?.ReportedValueInNativeUnits;
        }

        var averageExecutionTime = results.Average(x => x.ExecutionTime);

        var resultsWithOldValues = results.Where(x => x.OldValue.HasValue).ToList();
        var averageDifference = resultsWithOldValues.Average(x => x.Difference);
        var minDifference = resultsWithOldValues.Min(x => x.Difference);
        var maxDifference = resultsWithOldValues.Max(x => x.Difference);
        var standardDeviation = resultsWithOldValues.Select(x => x.Difference!.Value).StandardDeviation();

        Console.WriteLine($"Total Count: {results.Count}");
        Console.WriteLine($"\tAverage execution time: {averageExecutionTime}ms");
        Console.WriteLine();
        Console.WriteLine($"Count w/ Previous Values: {resultsWithOldValues.Count}");

        var validDifferencePercents = resultsWithOldValues
            .Where(x => x.DifferencePercent.HasValue)
            .Select(x => x.DifferencePercent.GetValueOrDefault(0)).ToList();

        var averageDifferencePercent = validDifferencePercents.Any()
            ? validDifferencePercents.Average()
            : (double?)null;

        Console.WriteLine($"\tAverage Difference Percent: {Math.Round(averageDifferencePercent.GetValueOrDefault(0), 2, MidpointRounding.ToEven)}%");
        Console.WriteLine($"\tAverage Difference: {averageDifference}");
        Console.WriteLine($"\tMin Difference: {minDifference}");
        Console.WriteLine($"\tMax Difference: {maxDifference}");
        Console.WriteLine($"\tStandard Deviation: {standardDeviation}");
        Console.WriteLine($"\tAverage of new values: {resultsWithOldValues.Average(x => x.RasterValue)}");
        Console.WriteLine($"\tAverage of old values: {resultsWithOldValues.Average(x => x.OldValue)}");

        Console.WriteLine();

        var resultsWithErrors = results.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).ToList();
        if (resultsWithErrors.Any())
        {
            Console.WriteLine($"Count w/ Errors: {resultsWithErrors.Count}");
            Console.WriteLine();

            var prettyPrintedResultsWithErrorsJSON = JsonSerializer.Serialize(resultsWithErrors, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine(prettyPrintedResultsWithErrorsJSON);

            List<string> skipErrorList =
            [
                RasterProcessingService.UsageLocationNoValidGeometryErrorMessage,
                RasterProcessingService.UsageLocationOutsideGSABoundaryErrorMessage
            ];

            if (resultsWithErrors.Any(x => !skipErrorList.Contains(x.ErrorMessage)))
            {
                Assert.Fail($"{resultsWithErrors.Count} results had blocking errors, printed to console.");
            }
        }

        var prettyPrintedResultJSON = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(prettyPrintedResultJSON);

        Assert.IsTrue(results.Where(x => string.IsNullOrEmpty(x.ErrorMessage)).All(x => x.RasterValue.HasValue));
        Assert.AreEqual(usageLocationDtos.Count, results.Count);
    }
}

public static class DoubleExtensions
{
    public static double StandardDeviation(this IEnumerable<double> values)
    {
        if (!values.Any())
        {
            return 0;
        }

        var avg = values.Average();
        return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
    }
}