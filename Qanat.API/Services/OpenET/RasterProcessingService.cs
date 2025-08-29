using Hangfire;
using MaxRev.Gdal.Core;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using Qanat.Common.GeoSpatial;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qanat.API.Services.OpenET;

public class RasterProcessingService
{
    private readonly QanatDbContext _dbContext;
    private readonly FileService _fileService;

    public static readonly string UsageLocationNoValidGeometryErrorMessage = "No valid geometry found for Usage Location.";
    public static readonly string UsageLocationOutsideGSABoundaryErrorMessage = "Usage Location is outside of the buffered GSA boundary.";

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
    public async Task<List<RasterProcessingResult>> ProcessRasterByFileCanonicalNameForUsageLocations(GeographyDto geography, int? openETSyncHistoryID, int waterMeasurementTypeID, int unitTypeID, DateTime reportedDate, string fileCanonicalName, List<int> usageLocationIDs = null, bool fromManualUpload = false, bool dryRun = true)
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
            results = await ProcessRasterBytesForUsageLocations(geography, rasterBytes, reportedDate, usageLocationIDs);
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

        await WaterMeasurements.DeleteWaterMeasurements(_dbContext, geography.GeographyID, waterMeasurementTypeID, reportedDate, usageLocationIDs);

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var rasterProcessingResult in results)
        {
            var unitTypeEnum = (UnitTypeEnum)unitTypeID;
            var usageArea = (decimal)rasterProcessingResult.UsageLocationArea;
            var volume = usageArea != 0
                ? WaterMeasurements.ConvertReportedValueToAcreFeet(unitTypeEnum, (decimal)rasterProcessingResult.RasterValue.GetValueOrDefault(0), usageArea)
                : 0;
            var depth = usageArea != 0
                ? volume / usageArea
                : 0;
            var newWaterMeasurement = new WaterMeasurement
            {
                GeographyID = geography.GeographyID,
                UsageLocationID = rasterProcessingResult.UsageLocationID,
                WaterMeasurementTypeID = waterMeasurementTypeID,
                UnitTypeID = unitTypeID,
                ReportedDate = reportedDate,
                ReportedValueInNativeUnits = (decimal)rasterProcessingResult.RasterValue.GetValueOrDefault(0),
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
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

    public async Task<List<RasterProcessingResult>> ProcessRasterBytesForUsageLocations(GeographyDto geography, byte[] rasterBytes, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        var usageLocations = await UsageLocations.ListByGeographyAndReportedDate(_dbContext, geography.GeographyID, reportedDate, true);

        if (usageLocationIDs != null)
        {
            usageLocations = usageLocations.Where(x => usageLocationIDs.Contains(x.UsageLocationID)).ToList();
        }

        var usageLocationDtos = usageLocations.Select(x => new UsageLocationSimpleDto()
        {
            UsageLocationID = x.UsageLocationID,
            Name = x.Name,
            Area = x.Area
        }).ToList();

        var results = await ProcessRasterBytesForUsageLocations(geography, usageLocationDtos, rasterBytes);
        return results;
    }

    public async Task<List<RasterProcessingResult>> ProcessRasterBytesForUsageLocations(GeographyDto geographyDto, List<UsageLocationSimpleDto> usageLocationDtos, byte[] rasterBytes, bool cleanupFiles = true)
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

        foreach (var chunkedUsageLocations in usageLocationDtos.Chunk(100))
        {
            using var rasterDataset = Gdal.Open(rasterFilePath, Access.GA_ReadOnly);
            foreach (var usageLocation in chunkedUsageLocations)
            {
                var result = await GetRasterValueForUsageLocation(geographyDto, bufferedGSABoundary, rasterDataset, usageLocation, tempDirectoryPath);
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

    public async Task<RasterProcessingResult> GetRasterValueForUsageLocation(GeographyDto geography, Geometry geographyBoundaryGSABoundary, Dataset rasterDataset, UsageLocationSimpleDto usageLocation, string tempDirectoryPath = null)
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
        var usageLocationGeometry = await _dbContext.UsageLocationGeometries.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UsageLocationID == usageLocation.UsageLocationID);

        RasterProcessingResult processingResult;
        if (usageLocationGeometry == null || !usageLocationGeometry.GeometryNative.IsValid)
        {
            stopwatch.Stop();
            processingResult = new RasterProcessingResult(usageLocation.UsageLocationID, usageLocation.Name, usageLocation.Area, null, stopwatch.ElapsedMilliseconds, UsageLocationNoValidGeometryErrorMessage);

            return processingResult;
        }

        var coveredByGSABoundary = usageLocationGeometry.Geometry4326.CoveredBy(geographyBoundaryGSABoundary);
        if (!coveredByGSABoundary)
        {
            stopwatch.Stop();
            processingResult = new RasterProcessingResult(usageLocation.UsageLocationID, usageLocation.Name, usageLocation.Area, null, stopwatch.ElapsedMilliseconds, UsageLocationOutsideGSABoundaryErrorMessage);

            return processingResult;
        }

        var tempUsageLocationDirectory = $"{tempDirectoryPath}/{usageLocation.Name}";
        var outputRasterPath = $"{tempUsageLocationDirectory}/ClippedRaster.tif";
        var outputGeoJSONPath = $"{tempUsageLocationDirectory}/UsageLocationGeometry.geojson";

        if (!Directory.Exists(tempUsageLocationDirectory))
        {
            Directory.CreateDirectory(tempUsageLocationDirectory);
        }
        await GeoJsonSerializer.SerializeToFileAsync(usageLocationGeometry.GeometryNative.Buffer(0), outputGeoJSONPath, GeoJsonSerializer.DefaultSerializerOptions);

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
            processingResult = new RasterProcessingResult(usageLocation.UsageLocationID, usageLocation.Name, usageLocation.Area, null, stopwatch.ElapsedMilliseconds, ex.Message);

            return processingResult;
        }
        finally
        {
            CleanupUsageLocationRun(tempUsageLocationDirectory);
        }

        stopwatch.Stop();
        processingResult = new RasterProcessingResult(usageLocation.UsageLocationID, usageLocation.Name, usageLocation.Area, geometryRasterMeanValueRounded, stopwatch.ElapsedMilliseconds, null);

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

    private static void CleanupUsageLocationRun(string tempUsageLocationDirectory)
    {
        if (!string.IsNullOrEmpty(tempUsageLocationDirectory) && Directory.Exists(tempUsageLocationDirectory))
        {
            Directory.Delete(tempUsageLocationDirectory, true);
        }
    }

    public async Task RunCalculations(int geographyID, int waterMeasurementTypeID, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, waterMeasurementTypeID, reportedDate, usageLocationIDs);
    }
}

public record RasterProcessingResult
{
    public int UsageLocationID { get; set; }
    public string UsageLocationName { get; set; }
    public double UsageLocationArea { get; set; }
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

    public RasterProcessingResult(int usageLocationID, string usageLocationName, double usageLocationArea, double? rasterValue, long executionTime, string errorMessage)
    {
        UsageLocationID = usageLocationID;
        UsageLocationName = usageLocationName;
        UsageLocationArea = Math.Round(usageLocationArea, 3, MidpointRounding.ToEven);
        RasterValue = rasterValue;
        ExecutionTime = executionTime;
        ErrorMessage = errorMessage;
    }
}