using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qanat.API.Services.GoogleDrive;
using Qanat.Common.Services.GDAL;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;

namespace Qanat.API.Services.OpenET;

public class OpenETSyncService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenETSyncService> _logger;
    private readonly QanatDbContext _qanatDbContext;
    private readonly GDALAPIService _gdalApiService;
    private readonly DriveService _googleDriveService;
    private readonly FileService _fileService;

    public OpenETSyncService(HttpClient httpClient, ILogger<OpenETSyncService> logger, QanatDbContext qanatDbContext, GDALAPIService gdalApiService, DriveService driveService, FileService fileService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _qanatDbContext = qanatDbContext;
        _gdalApiService = gdalApiService;
        _googleDriveService = driveService;
        _fileService = fileService;
    }

    public async Task SyncOpenETRasterCompositeForGeographyYearMonthDataTypeID(int geographyID, int year, int month, int openETDataTypeID, int openETSyncHistoryID)
    {
        var geography = _qanatDbContext.Geographies.AsNoTracking()
            .Include(x => x.GeographyBoundary)
            .Single(x => x.GeographyID == geographyID);

        //MK 9/5/2024 -- This method needs to be run from a user-less context (aka a backgrounded hangfire task), but FileResource requires a user. Use a hardcoded value representing the System Admin.
        var adminUser = Users.GetByUserID(_qanatDbContext, Users.QanatSystemAdminUserID);
        if (adminUser == null)
        {
            throw new OpenETException($"Could not find System Admin while attempting to sync.");
        }

        var openETDataType = OpenETDataType.AllLookupDictionary[openETDataTypeID];
        var otherSyncInProgress = _qanatDbContext.OpenETSyncHistories.AsNoTracking()
            .Include(x => x.OpenETSync)
            .Any(x => x.OpenETSync.Year == year
                   && x.OpenETSync.Month == month
                   && x.OpenETSync.OpenETDataTypeID == openETDataTypeID
                   && (x.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.Created || x.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.InProgress)
                   && x.OpenETSyncHistoryID != openETSyncHistoryID);

        if (otherSyncInProgress)
        {
            return;
        }

        var openETSyncHistory = OpenETSyncHistories.CreateNew(_qanatDbContext, year, month, openETDataTypeID, geography.GeographyID);
        var targetWKT = await _gdalApiService.GdalSrsInfoGetWktForCoordinateSystem(geography.CoordinateSystem);

        if (!await RasterUpdatedSinceMinimumLastUpdatedDate(geography, month, year, openETDataTypeID, openETSyncHistory, targetWKT))
        {
            return; // no new data
        }

        await OpenETSyncHistories.UpdateOpenETSyncEntityByID(_qanatDbContext, openETSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.InProgress);

        var geometryAsDoubleArray = geography.GetBoundaryAsDoubleArray(targetWKT);
        var rasterExportCompositePostBody = new OpenETRasterExportCompositePostBody()
        {
            DateRange = new[]
                {
                    $"{openETSyncHistory.OpenETSync.ReportedDate:yyyy-MM-dd}",
                    $"{openETSyncHistory.OpenETSync.ReportedDate.AddMonths(1).AddDays(-1):yyyy-MM-dd}"
                },
            Geometry = geometryAsDoubleArray,
            Model = "ensemble",
            Variable = openETDataType.OpenETDataTypeVariableName,
            ReferenceET = "gridMET",
            Reducer = "mean",
            Units = "in",
            Encrypt = false,
            Cog = false,
        };

        try
        {
            const string exportCompositeRoute = "raster/export/composite";
            var exportCompositeResponse = await _httpClient.PostAsJsonAsync(exportCompositeRoute, rasterExportCompositePostBody);
            var exportCompositeResponseBody = await exportCompositeResponse.Content.ReadAsStringAsync();

            if (!exportCompositeResponse.IsSuccessStatusCode)
            {
                throw new OpenETException($"Call to {exportCompositeRoute} failed. Status Code: {exportCompositeResponse.StatusCode} Message: {exportCompositeResponseBody}");
            }

            var exportResult = JsonSerializer.Deserialize<OpenETRasterExportCompositeResult>(exportCompositeResponseBody);
            if (exportResult == null || string.IsNullOrEmpty(exportResult.TrackingID))
            {
                throw new OpenETException($"Deserializing OpenET API Export Composite failed.");
            }

            GoogleDriveFile googleDriveFile = null;
            FileResource fileResource = null;

            var timeBetweenAttempts = 1000 * 5; //5 seconds
            var attempts = 0;
            var maxAttempts = 37; //~3 minute of total time

            while (attempts < maxAttempts)
            {
                //MK 8/29/2024 -- Give it a little time to finish before checking again. Wish they provided webhooks... Doing this upfront to give a delay for the first response.
                Thread.Sleep(timeBetweenAttempts);

                var googleDriveFiles = await _googleDriveService.ListFiles();
                googleDriveFile = googleDriveFiles.Files.SingleOrDefault(x => x.Name.Contains(exportResult.Name));

                if (googleDriveFile != null)
                {
                    var memoryStream = new MemoryStream();
                    await _googleDriveService.Files.Get(googleDriveFile.Id).DownloadAsync(memoryStream);

                    fileResource = await _fileService.CreateFileResource(_qanatDbContext, memoryStream, googleDriveFile.Name, adminUser.UserID);

                    var updatedSyncHistory = await OpenETSyncHistories.UpdateOpenETSyncEntityByID(_qanatDbContext, openETSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.Succeeded, null, googleDriveFile.Id, fileResource.FileResourceID);

                    break;
                }

                attempts++;
            }

            if (googleDriveFile == null || fileResource == null || attempts == maxAttempts)
            {
                throw new OpenETException($"OpenET API Export Composite failed after hitting the max of {maxAttempts} attempts.");
            }
        }
        catch (TaskCanceledException ex)
        {
            await OpenETSyncHistories.UpdateOpenETSyncEntityByID(_qanatDbContext, openETSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.Failed, "OpenET API did not respond");
            _logger.Log(LogLevel.Error, ex, "Error communicating with OpenET API.");
        }
        catch (Exception ex)
        {
            await OpenETSyncHistories.UpdateOpenETSyncEntityByID(_qanatDbContext, openETSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.Failed, ex.Message);
            _logger.Log(LogLevel.Error, ex, "Error communicating with OpenET API.");
        }
    }

    private async Task<bool> RasterUpdatedSinceMinimumLastUpdatedDate(Geography geography, int month, int year, int openETDataTypeID, OpenETSyncHistory openETSyncHistory, string targetWkt)
    {
        var geometryArray = geography.GetBoundaryAsStringArray(targetWkt);
        var openETSync = openETSyncHistory.OpenETSync;
        var openETRasterMetadataPostRequestBody = new OpenETRasterMetadataPostRequestBody(openETSync.OpenETDataType.OpenETDataTypeVariableName, geometryArray);

        try
        {
            const string rasterMetadataRoute = "raster/metadata";
            var rasterMetadataResponse = await _httpClient.PostAsJsonAsync(rasterMetadataRoute, openETRasterMetadataPostRequestBody);
            var rasterMetadataBody = await rasterMetadataResponse.Content.ReadAsStringAsync();
            if (!rasterMetadataResponse.IsSuccessStatusCode)
            {
                throw new OpenETException($"Call to {rasterMetadataRoute} was unsuccessful. Status Code: {rasterMetadataResponse.StatusCode} Message: {rasterMetadataBody}");
            }

            var rasterMetadataResult = JsonSerializer.Deserialize<RasterMetadataDateIngested>(rasterMetadataBody);
            if (string.IsNullOrEmpty(rasterMetadataResult.BuildDate) || !DateTime.TryParse(rasterMetadataResult.BuildDate, out var responseDate))
            {
                await OpenETSyncHistories.UpdateOpenETSyncEntityByID(_qanatDbContext, openETSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.DataNotAvailable);
                return false;
            }

            var openETSyncHistoriesThatWereSuccessful = _qanatDbContext.OpenETSyncHistories.AsNoTracking()
                .Include(x => x.OpenETSync)
                .Where(x => x.OpenETSync.GeographyID == openETSync.GeographyID
                         && x.OpenETSync.Year == year
                         && x.OpenETSync.Month == month
                         && x.OpenETSync.OpenETDataTypeID == openETDataTypeID
                         && x.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.Succeeded
                         && !string.IsNullOrEmpty(x.GoogleDriveRasterFileID)
                         && x.RasterFileResourceID.HasValue);

            if (!openETSyncHistoriesThatWereSuccessful.Any())
            {
                return true;
            }

            var mostRecentSyncHistory = openETSyncHistoriesThatWereSuccessful.OrderByDescending(x => x.UpdateDate).First();
            if (responseDate > mostRecentSyncHistory.UpdateDate)
            {
                return true;
            }

            await OpenETSyncHistories.UpdateOpenETSyncEntityByID(_qanatDbContext, openETSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.NoNewData, null, openETSyncHistory.GoogleDriveRasterFileID, openETSyncHistory.RasterFileResourceID);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            await OpenETSyncHistories.UpdateOpenETSyncEntityByID(_qanatDbContext, openETSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.Failed, "OpenET API did not respond");
            _logger.Log(LogLevel.Error, ex, "Error communicating with OpenET API.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, "Error when attempting to check raster metadata date ingested.");
            await OpenETSyncHistories.UpdateOpenETSyncEntityByID(_qanatDbContext, openETSyncHistory.OpenETSyncHistoryID, OpenETSyncResultTypeEnum.Failed, ex.Message);
            return false;
        }
    }
}