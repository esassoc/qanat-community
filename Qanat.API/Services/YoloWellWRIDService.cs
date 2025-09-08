using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.EFModels.Entities;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Util;

namespace Qanat.API.Services;

public class YoloWellWRIDService
{
    private readonly ILogger<YoloWellWRIDService> _logger;
    private readonly QanatConfiguration _qanatConfiguration;
    private readonly QanatDbContext _qanatDbContext;
    private readonly HttpClient _httpClient;

    public YoloWellWRIDService(HttpClient httpClient, ILogger<YoloWellWRIDService> logger,
        IOptions<QanatConfiguration> qanatConfiguration, QanatDbContext qanatDbContext,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _qanatConfiguration = qanatConfiguration.Value;
        _qanatDbContext = qanatDbContext;
        _httpClient = httpClient;
    }

    public async Task RetrieveScadaWellsAndMeasurements()
    {
        var bearerToken = await CreateBearerToken();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");

        try
        {
            var wellResponse = await _httpClient.GetAsync("/api/ScadaWell");

            var wellResponseText = await wellResponse.Content.ReadAsStreamAsync();
            var wellsJson = await JsonSerializer.DeserializeAsync<List<ScadaWellJson>>(wellResponseText);


            var wellMeasurementResponse = await _httpClient.GetAsync("/api/ScadaWellData");

            var wellMeasurementResponseText = await wellMeasurementResponse.Content.ReadAsStreamAsync();
            var wellMeasurementsJson = await JsonSerializer.DeserializeAsync<List<ScadaWellMeasurementJson>>(wellMeasurementResponseText);

            await SaveWellsAndMeasurements(wellsJson, wellMeasurementsJson);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    private async Task<string> CreateBearerToken()
    {
        try
        {
            var query = new Dictionary<string, string>
            {
                ["UserName"] = _qanatConfiguration.YoloWRIDAPIUsername,
                ["Password"] = _qanatConfiguration.YoloWRIDAPIPassword
            };

            var postUrl = QueryHelpers.AddQueryString("/api/Token", query);

            var response = await _httpClient.PostAsync(postUrl, null);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    private async Task SaveWellsAndMeasurements(List<ScadaWellJson> wellsJson, List<ScadaWellMeasurementJson> wellMeasurementsJson)
    {
        var geographyID = _qanatDbContext.Geographies.AsNoTracking().Single(x => x.GeographyName == "Yolo").GeographyID;
        var yoloWRIDMonitorngWellSourceTypeID = (int)MonitoringWellSourceTypeEnum.YoloWRID;

        var updatedMonitoringWells = wellsJson.Select(x => new MonitoringWell()
        {
            GeographyID = geographyID,
            SiteCode = x.WellName,
            MonitoringWellName = x.OwnerName,
            MonitoringWellSourceTypeID = yoloWRIDMonitorngWellSourceTypeID,
            Geometry = GeometryHelper.CreateLocationPoint4326FromLatLong(x.Latitude, x.Longitude)
        }).ToList();

        var allInMonitoringWellsDB = _qanatDbContext.MonitoringWells;
        var existingMonitoringWells = allInMonitoringWellsDB
            .Where(x => x.MonitoringWellSourceTypeID == yoloWRIDMonitorngWellSourceTypeID).ToList();

        existingMonitoringWells.Merge(updatedMonitoringWells, allInMonitoringWellsDB, (x, y) => x.SiteCode == y.SiteCode,
            (x, y) =>
            {
                x.MonitoringWellName = y.MonitoringWellName;
                x.Geometry = y.Geometry;
            });

        await _qanatDbContext.SaveChangesAsync();

        var monitoringWellIDByScadaWellID = wellsJson.ToDictionary(x => x.ScadaWellID,
            x => existingMonitoringWells.Single(y => y.SiteCode == x.WellName).MonitoringWellID);

        var updatedMonitoringWellMeasurements = wellMeasurementsJson.Select(x => new MonitoringWellMeasurement()
        {
            MonitoringWellID = monitoringWellIDByScadaWellID[x.ScadaWellID],
            GeographyID = geographyID,
            ExtenalUniqueID = x.ScadaWellMeasurementID,
            Measurement = x.Measurement,
            MeasurementDate = x.Timestamp
        }).ToList();

        var allInMonitoringWellMeasurementsDB = _qanatDbContext.MonitoringWellMeasurements;
        var existingMonitoringWellMeasurements = allInMonitoringWellMeasurementsDB
            .Include(x => x.MonitoringWell)
            .Where(x => x.MonitoringWell.MonitoringWellSourceTypeID == yoloWRIDMonitorngWellSourceTypeID).ToList();

        existingMonitoringWellMeasurements.Merge(updatedMonitoringWellMeasurements, allInMonitoringWellMeasurementsDB, 
            (x, y) => 
                x.ExtenalUniqueID == y.ExtenalUniqueID && 
                x.MonitoringWellID == y.MonitoringWellID &&
                x.Measurement == y.Measurement && 
                x.MeasurementDate == y.MeasurementDate
            );

        await _qanatDbContext.SaveChangesAsync();
    }
}

public class ScadaWellJson
{
    [JsonPropertyName("id")]
    public int ScadaWellID { get; set; }

    [JsonPropertyName("ownerName")]
    public string OwnerName { get; set; }

    [JsonPropertyName("wellName")]
    public string WellName { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}

public class ScadaWellMeasurementJson
{
    [JsonPropertyName("id")]
    public int ScadaWellMeasurementID { get; set; }

    [JsonPropertyName("scadaWellId")]
    public int ScadaWellID { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("dtw12hAvg")]
    public decimal Measurement { get; set; }
}