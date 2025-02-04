using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hangfire;
using MaxRev.Gdal.Core;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using Qanat.Common.GeoSpatial;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.API.Services.OpenET;

public class RasterProcessingService
{
    private readonly QanatDbContext _dbContext;
    private readonly FileService _fileService;

    public RasterProcessingService(QanatDbContext dbContext, FileService fileService)
    {
        _dbContext = dbContext;
        _fileService = fileService;

        if (!GdalBase.IsConfigured)
        {
            GdalBase.ConfigureAll();
        }
    }

    //MK 9/19/2024 -- The nullable openETSyncHistoryID is a bit of a code smell. It'd be better to try and find a way to separate the concerns more, but this works for now.
    [AutomaticRetry(Attempts = 3)]
    public async Task<List<RasterProcessingResult>> ProcessRasterByFileCanonicalNameForAllUsageEntities(GeographyDto geography, int? openETSyncHistoryID, int waterMeasurementTypeID, int unitTypeID, DateTime reportedDate, string fileCanonicalName, bool fromManualUpload = false, bool dryRun = true)
    {
        if (_fileService == null)
        {
            throw new ArgumentException("FileService is required to process raster by file canonical name.");
        }

        var rasterStream = await _fileService.GetFileStreamFromBlobStorage(fileCanonicalName);
        rasterStream.Seek(0, SeekOrigin.Begin);

        byte[] rasterBytes;
        using (var memoryStream = new MemoryStream())
        {
            await rasterStream.CopyToAsync(memoryStream);
            rasterBytes = memoryStream.ToArray(); // Get the entire byte array

            if (rasterBytes.Length == 0 || rasterStream.Length != rasterBytes.Length)
            {
                throw new Exception("Failed to read bytes from raster file.");
            }
        }

        if (!dryRun && openETSyncHistoryID.HasValue)
        {
            await OpenETSyncHistories.UpdateOpenETSyncRasterCalculationMetadata(_dbContext, openETSyncHistoryID.Value, OpenETRasterCalculationResultTypeEnum.InProgress, null);
        }

        List<RasterProcessingResult> results;
        try
        {
            results = await ProcessRasterBytesForAllUsageEntities(geography, rasterBytes);
            if (dryRun)
            {
                return results;
            }
        }
        catch (Exception ex)
        {
            if (!dryRun && openETSyncHistoryID.HasValue)
            {
                await OpenETSyncHistories.UpdateOpenETSyncRasterCalculationMetadata(_dbContext, openETSyncHistoryID.Value, OpenETRasterCalculationResultTypeEnum.Failed, ex.Message);
            }
            throw;
        }

        await WaterMeasurements.DeleteWaterMeasurements(_dbContext, geography.GeographyID, waterMeasurementTypeID, reportedDate);

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var rasterProcessingResult in results)
        {
            var unitTypeEnum = (UnitTypeEnum)unitTypeID;
            var newWaterMeasurement = new WaterMeasurement
            {
                GeographyID = geography.GeographyID,
                WaterMeasurementTypeID = waterMeasurementTypeID,
                UnitTypeID = unitTypeID,
                ReportedDate = reportedDate,
                UsageEntityName = rasterProcessingResult.UsageEntityName,
                UsageEntityArea = (decimal)rasterProcessingResult.UsageEntityArea,
                ReportedValue = (decimal) rasterProcessingResult.RasterValue.GetValueOrDefault(0),
                ReportedValueInAcreFeet = WaterMeasurements.ConvertReportedValueToAcreFeet(unitTypeEnum, (decimal) rasterProcessingResult.RasterValue.GetValueOrDefault(0), (decimal) rasterProcessingResult.UsageEntityArea),
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = fromManualUpload,
                Comment = !string.IsNullOrEmpty(rasterProcessingResult.ErrorMessage)
                    ? rasterProcessingResult.ErrorMessage
                    : $"Computed from raster data. FileCanonicalName: '{fileCanonicalName}'"
            };

            newWaterMeasurements.Add(newWaterMeasurement);
        }

        var newWaterMeasurementDtos = await WaterMeasurements.CreateWaterMeasurements(_dbContext, newWaterMeasurements);
        if (newWaterMeasurementDtos.Count != results.Count)
        {
            var errorMessage = $"Failed to create all expected water measurements for the raster. Created {newWaterMeasurementDtos.Count} when expecting {results.Count}.";
            if (openETSyncHistoryID.HasValue)
            {
                await OpenETSyncHistories.UpdateOpenETSyncRasterCalculationMetadata(_dbContext, openETSyncHistoryID.Value, OpenETRasterCalculationResultTypeEnum.Failed, errorMessage);
            }

            throw new Exception(errorMessage);
        }

        if (openETSyncHistoryID.HasValue)
        {
            await OpenETSyncHistories.UpdateOpenETSyncRasterCalculationMetadata(_dbContext, openETSyncHistoryID.Value, OpenETRasterCalculationResultTypeEnum.Succeeded, null);
        }

        return results;
    }

    public async Task<List<RasterProcessingResult>> ProcessRasterBytesForAllUsageEntities(GeographyDto geography, byte[] rasterBytes)
    {
        var usageEntities = await _dbContext.UsageEntities.AsNoTracking()
            .Where(x => x.GeographyID == geography.GeographyID)
            .ToListAsync();

        var usageEntityDtos = usageEntities.Select(x => new UsageEntitySimpleDto()
        {
            UsageEntityID = x.UsageEntityID,
            UsageEntityName = x.UsageEntityName,
            UsageEntityArea = x.UsageEntityArea
        }).ToList();

        var results = await ProcessRasterBytesForUsageEntities(geography, usageEntityDtos, rasterBytes);
        return results;
    }

    public async Task<List<RasterProcessingResult>> ProcessRasterBytesForUsageEntities(GeographyDto geographyDto, List<UsageEntitySimpleDto> usageEntityDto, byte[] rasterBytes, bool cleanupFiles = true)
    {
        var runID = Guid.NewGuid();
        var tempDirectoryPath = $"{Path.GetTempPath()}raster-file-processing/runs/{runID}";
        if (!Directory.Exists(tempDirectoryPath))
        {
            Directory.CreateDirectory(tempDirectoryPath);
        }

        var rasterFilePath = $"{tempDirectoryPath}/{Guid.NewGuid()}.tif";
        await using (var tempRasterFile = File.Create(rasterFilePath))
        {
            await tempRasterFile.WriteAsync(rasterBytes);
        }

        var results = new List<RasterProcessingResult>();
        var geographyBoundary = await _dbContext.GeographyBoundaries.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyDto.GeographyID);

        var bufferedGSABoundary = geographyBoundary.GSABoundary.Buffer(Geographies.GSABoundaryBuffer).Envelope;

        foreach (var chunk in usageEntityDto.Chunk(100))
        {
            using var rasterDataset = Gdal.Open(rasterFilePath, Access.GA_ReadOnly);
            foreach (var usageEntity in chunk)
            {
                var result = await GetRasterValueForUsageEntity(geographyDto, bufferedGSABoundary, rasterDataset, usageEntity, tempDirectoryPath);
                results.Add(result);
            }
        }

        if (cleanupFiles)
        {
            File.Delete(rasterFilePath);
            Directory.Delete(tempDirectoryPath, true);
        }

        return results;
    }

    public async Task<RasterProcessingResult> GetRasterValueForUsageEntity(GeographyDto geography, Geometry geographyBoundaryGSABoundary, Dataset rasterDataset, UsageEntitySimpleDto usageEntity, string tempDirectoryPath = null)
    {
        if (string.IsNullOrEmpty(tempDirectoryPath))
        {
            var runID = Guid.NewGuid();
            tempDirectoryPath = $"{Path.GetTempPath()}raster-file-processing/runs/{runID}";

            if (!Directory.Exists($"{Path.GetTempPath()}raster-file-processing"))
            {
                Directory.CreateDirectory($"{Path.GetTempPath()}raster-file-processing");
            }

            if (!Directory.Exists($"{Path.GetTempPath()}raster-file-processing/runs"))
            {
                Directory.CreateDirectory($"{Path.GetTempPath()}raster-file-processing/runs");
            }

            if (!Directory.Exists(tempDirectoryPath))
            {
                Directory.CreateDirectory(tempDirectoryPath);
            }
        }

        var stopwatch = Stopwatch.StartNew();
        var usageEntityGeometry = await _dbContext.UsageEntityGeometries.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UsageEntityID == usageEntity.UsageEntityID);

        RasterProcessingResult processingResult;
        if (usageEntityGeometry == null || !usageEntityGeometry.GeometryNative.IsValid)
        {
            stopwatch.Stop();
            processingResult = new RasterProcessingResult(usageEntity.UsageEntityName, usageEntity.UsageEntityArea, null, stopwatch.ElapsedMilliseconds, "No valid geometry found for usage entity.");

            return processingResult;
        }

        var coveredByGSABoundary = usageEntityGeometry.Geometry4326.CoveredBy(geographyBoundaryGSABoundary);
        if (!coveredByGSABoundary)
        {
            stopwatch.Stop();
            processingResult = new RasterProcessingResult(usageEntity.UsageEntityName, usageEntity.UsageEntityArea, null, stopwatch.ElapsedMilliseconds, "Usage entity is outside of the buffered GSA boundary.");

            return processingResult;
        }

        var tempUsageEntityDirectory = $"{tempDirectoryPath}/{usageEntity.UsageEntityName}";
        var outputRasterPath = $"{tempUsageEntityDirectory}/ClippedRaster.tif";
        var outputGeoJSONPath = $"{tempUsageEntityDirectory}/UsageEntityGeometry.geojson";

        if (!Directory.Exists(tempUsageEntityDirectory))
        {
            Directory.CreateDirectory(tempUsageEntityDirectory);
        }
        await GeoJsonSerializer.SerializeToFileAsync(usageEntityGeometry.GeometryNative.Buffer(0), outputGeoJSONPath, GeoJsonSerializer.DefaultSerializerOptions);

        // Set up the warp options to clip the raster to the geometry
        using var warpOptions = new GDALWarpAppOptions([
//            "GDAL_CACHEMAX", "500",
//            "-wm", "500",
            "-cutline", outputGeoJSONPath, "-crop_to_cutline", "-cutline_srs", $"EPSG:{geography.CoordinateSystem}",
            "-t_srs", $"EPSG:{geography.CoordinateSystem}",
            "-wo", "CUTLINE_ALL_TOUCHED=TRUE",
//            "-wo", "OPTIMIZE-SIZE=YES",
            "-tr", "1", "1"
        ]);

        double? geometryRasterMeanValueRounded;
        try
        {
            using var warpedDataset = Gdal.Warp(outputRasterPath, [rasterDataset], warpOptions, null, null);

            using var infoOptions = new GDALInfoOptions([
                "-stats",
                "-json"
            ]);

            var gdalInfo = Gdal.GDALInfo(warpedDataset, infoOptions);
            geometryRasterMeanValueRounded = GetGeometryRasterMeanValueRounded(gdalInfo);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            processingResult = new RasterProcessingResult(usageEntity.UsageEntityName, usageEntity.UsageEntityArea, null, stopwatch.ElapsedMilliseconds,
                ex.Message);

            return processingResult;
        }
        finally
        {
            CleanupUsageEntityRun(tempUsageEntityDirectory);
        }

        stopwatch.Stop();
        processingResult = new RasterProcessingResult(usageEntity.UsageEntityName, usageEntity.UsageEntityArea, geometryRasterMeanValueRounded, stopwatch.ElapsedMilliseconds,null);

        return processingResult;
    }

    private static double? GetGeometryRasterMeanValueRounded(string gdalInfo)
    {
        using var doc = JsonDocument.Parse(gdalInfo);
        var root = doc.RootElement;
        var bands = root.GetProperty("bands");
        var band = bands[0];
        var metadata = band.GetProperty("metadata");
        var obj = metadata.GetProperty(""); //not sure why it is an empty key, but it is. 
        var meanAsString = obj.GetProperty("STATISTICS_MEAN");

        var parsed = double.TryParse(meanAsString.GetString(), out var meanAsDecimal);
        double? geometryRasterMeanValueRounded = parsed
            ? Math.Round(meanAsDecimal, 4, MidpointRounding.ToEven)
            : null;
        return geometryRasterMeanValueRounded;
    }

    private static void CleanupUsageEntityRun(string tempUsageEntityDirectory)
    {
        if (!string.IsNullOrEmpty(tempUsageEntityDirectory) && Directory.Exists(tempUsageEntityDirectory))
        {
            Directory.Delete(tempUsageEntityDirectory, true);
        }
    }

    public async Task RunCalculations(int geographyID, int waterMeasurementTypeID, DateTime reportedDate)
    {
        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, waterMeasurementTypeID, reportedDate);
    }
}

public record RasterProcessingResult
{
    public string UsageEntityName { get; set; }
    public double UsageEntityArea { get; set; }
    public double? RasterValue { get; set; }
    public double? OldValue { get; set; }
    public double? Difference => RasterValue.GetValueOrDefault(0) - OldValue.GetValueOrDefault(0);
    public double? DifferencePercent => OldValue.GetValueOrDefault(0) == 0
        ? null
        : Difference / OldValue.GetValueOrDefault(1) * 100;
    public long ExecutionTime { get; set; }
    public string? ErrorMessage { get; set; }

    public RasterProcessingResult()
    {
    }

    public RasterProcessingResult(string usageEntityName, double usageEntityArea, double? rasterValue, long executionTime, string errorMessage)
    {
        UsageEntityName = usageEntityName;
        UsageEntityArea = Math.Round(usageEntityArea, 3, MidpointRounding.ToEven);
        RasterValue = rasterValue;
        ExecutionTime = executionTime;
        ErrorMessage = errorMessage;
    }


}