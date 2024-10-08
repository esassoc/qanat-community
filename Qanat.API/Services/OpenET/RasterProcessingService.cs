using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MaxRev.Gdal.Core;
using Microsoft.EntityFrameworkCore;
using OSGeo.GDAL;
using Qanat.Common.GeoSpatial;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services.OpenET;

public class RasterProcessingService
{
    private QanatDbContext _dbContext;
    private FileService _fileService;

    public RasterProcessingService(QanatDbContext dbContext, FileService fileService)
    {
        _dbContext = dbContext;
        _fileService = fileService;

        GdalBase.ConfigureAll();
    }


    //MK 9/19/2024 -- The nullable openETSyncHistoryID is a bit of a code smell. It'd be better to try and find a way to separate the concerns more, but this works for now.
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
                UsageEntityArea = rasterProcessingResult.UsageEntityArea,
                ReportedValue = rasterProcessingResult.RasterValue.GetValueOrDefault(0),
                ReportedValueInAcreFeet = WaterMeasurements.ConvertReportedValueToAcreFeet(unitTypeEnum, rasterProcessingResult.RasterValue.GetValueOrDefault(0), rasterProcessingResult.UsageEntityArea),
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
            .Include(x => x.UsageEntityGeometry)
            .Where(x => x.GeographyID == geography.GeographyID && x.UsageEntityGeometry != null)
            .ToListAsync();

        usageEntities = usageEntities.Where(x => x.UsageEntityGeometry.GeometryNative.IsValid).ToList(); //Wanted to defer the IsValid checks till we had the data in memory, not sure filtering to those would be valid in SQL context.

        var usageEntityDtos = usageEntities.Select(x => new UsageEntityWithGeoJSONDto()
        {
            UsageEntityName = x.UsageEntityName,
            Area = x.UsageEntityArea,
            GeoJSON = x.UsageEntityGeometry.GeometryNative.ToGeoJSON()
        }).ToList();

        var results = await ProcessRasterBytesForUsageEntities(geography, usageEntityDtos, rasterBytes);
        return results;
    }

    public async Task<List<RasterProcessingResult>> ProcessRasterBytesForUsageEntities(GeographyDto geography, List<UsageEntityWithGeoJSONDto> usageEntityDto, byte[] rasterBytes, bool cleanupFiles = true)
    {
        var runID = Guid.NewGuid();
        var tempDirectoryPath = $"{Path.GetTempPath()}raster-file-processing/runs/{runID}";
        if (!Directory.Exists(tempDirectoryPath))
        {
            Directory.CreateDirectory(tempDirectoryPath);
        }

        var rasterFilePath = $"{tempDirectoryPath}/{Guid.NewGuid()}.tif";
        var tempRasterFile = File.Create(rasterFilePath);
        await tempRasterFile.WriteAsync(rasterBytes);
        tempRasterFile.Close();

        var rasterDataset = Gdal.Open(rasterFilePath, Access.GA_ReadOnly);

        var results = new List<RasterProcessingResult>();
        foreach (var usageEntity in usageEntityDto)
        {
            var result = await GetRasterValueForUsageEntity(geography, rasterDataset, usageEntity, tempDirectoryPath);
            results.Add(result);
        }

        rasterDataset.Dispose();
        if (cleanupFiles)
        {
            File.Delete(rasterFilePath);
        }

        await tempRasterFile.DisposeAsync();

        if (cleanupFiles)
        {
            Directory.Delete(tempDirectoryPath, true);
        }

        return results;
    }

    public async Task<RasterProcessingResult> GetRasterValueForUsageEntity(GeographyDto geography, Dataset rasterDataset, UsageEntityWithGeoJSONDto usageEntity, string tempDirectoryPath = null)
    {
        var cleanupRun = false;
        if (string.IsNullOrEmpty(tempDirectoryPath))
        {
            var runID = Guid.NewGuid();
            tempDirectoryPath = $"{Path.GetTempPath()}raster-file-processing/runs/{runID}";
            cleanupRun = true;

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

        RasterProcessingResult processingResult;
        var stopwatch = Stopwatch.StartNew();
        if (string.IsNullOrEmpty(usageEntity.GeoJSON))
        {
            stopwatch.Stop();
            processingResult = new RasterProcessingResult
            {
                UsageEntityName = usageEntity.UsageEntityName,
                UsageEntityArea = (decimal) usageEntity.Area,
                RasterValue = null,
                ExecutionTime = stopwatch.ElapsedMilliseconds,
                ErrorMessage = "No geometry found for usage entity"
            };

            return processingResult;
        }

        var tempUsageEntityDirectory = $"{tempDirectoryPath}\\{usageEntity.UsageEntityName}";
        var outputRasterPath = $"{tempUsageEntityDirectory}\\ClippedRaster.tif";
        var outputGeoJSONPath = $"{tempUsageEntityDirectory}\\UsageEntityGeometry.geojson";

        if (!Directory.Exists(tempUsageEntityDirectory))
        {
            Directory.CreateDirectory(tempUsageEntityDirectory);
        }

        await File.WriteAllTextAsync(outputGeoJSONPath, usageEntity.GeoJSON);

        // Set up the warp options to clip the raster to the geometry
        var warpOptions = new GDALWarpAppOptions(new[]
        {
            "-cutline", outputGeoJSONPath, "-crop_to_cutline", "-cutline_srs", $"EPSG:{geography.CoordinateSystem}",
            "-t_srs", $"EPSG:{geography.CoordinateSystem}",
            "-wo", "CUTLINE_ALL_TOUCHED=TRUE",
            "-tr", "1", "1",
        });

        decimal? geometryRasterMeanValueRounded = null;
        try
        {
            // Execute the warp to create a clipped raster
            var warpedDataset = Gdal.Warp(outputRasterPath, new[] { rasterDataset }, warpOptions, null, null);

            var infoOptions = new GDALInfoOptions(new[]
            {
                "-stats",
                "-json"
            });

            //var gdalInfoInputRaster = Gdal.GDALInfo(rasterDataset, infoOptions);

            var gdalInfo = Gdal.GDALInfo(warpedDataset, infoOptions);
            warpedDataset.Dispose();

            using var doc = JsonDocument.Parse(gdalInfo);
            var root = doc.RootElement;
            var bands = root.GetProperty("bands");
            var band = bands[0];
            var metadata = band.GetProperty("metadata");
            var obj = metadata.GetProperty(""); //not sure why it is an empty key, but it is. 
            var meanAsString = obj.GetProperty("STATISTICS_MEAN");

            var parsed = decimal.TryParse(meanAsString.GetString(), out decimal meanAsDecimal);
            geometryRasterMeanValueRounded = parsed
                ? Math.Round(meanAsDecimal, 4, MidpointRounding.ToEven)
                : null;

            doc!.Dispose();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            processingResult = new RasterProcessingResult
            {
                UsageEntityName = usageEntity.UsageEntityName,
                UsageEntityArea = (decimal)usageEntity.Area,
                RasterValue = null,
                ExecutionTime = stopwatch.ElapsedMilliseconds,
                ErrorMessage = ex.Message
            };

            return processingResult;
        }

        stopwatch.Stop();
        processingResult = new RasterProcessingResult
        {
            UsageEntityName = usageEntity.UsageEntityName,
            UsageEntityArea = (decimal)usageEntity.Area,
            RasterValue = geometryRasterMeanValueRounded,
            ExecutionTime = stopwatch.ElapsedMilliseconds,
            ErrorMessage = null
        };

        if (cleanupRun)
        {
            Directory.Delete(tempDirectoryPath, true);
        }
        else if (Directory.Exists(tempUsageEntityDirectory))
        {
            Directory.Delete(tempUsageEntityDirectory, true);
        }

        return processingResult;
    }

    public async Task RunCalculations(int geographyID, int waterMeasurementTypeID, DateTime reportedDate)
    {
        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, waterMeasurementTypeID, reportedDate);
    }
}

public class RasterProcessingResult
{
    public string UsageEntityName { get; set; }
    public decimal UsageEntityArea { get; set; }
    public decimal? RasterValue { get; set; }
    public decimal? OldValue { get; set; }
    public decimal? Difference => RasterValue.GetValueOrDefault(0) - OldValue.GetValueOrDefault(0); 
    public decimal? DifferencePercent => OldValue.GetValueOrDefault(0) == 0 
        ? null 
        : Difference / OldValue.GetValueOrDefault(1) * 100;
    public decimal ExecutionTime { get; set; }
    public string? ErrorMessage { get; set; }
}