using System;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.EFModels.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Qanat.API.Models;

namespace Qanat.API.Services;

public class MonitoringWellCNRAService
{
    private readonly ILogger<MonitoringWellCNRAService> _logger;
    private readonly QanatConfiguration _qanatConfiguration;
    private readonly QanatDbContext _qanatDbContext;
    private readonly HttpClient _httpClient;

    public MonitoringWellCNRAService(HttpClient httpClient, ILogger<MonitoringWellCNRAService> logger, IOptions<QanatConfiguration> qanatConfiguration, QanatDbContext qanatDbContext, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _qanatConfiguration = qanatConfiguration.Value;
        _qanatDbContext = qanatDbContext;
        _httpClient = httpClient;
    }

    public async Task<MonitoringWellJson> RetrieveMeasurements(Geometry boundary, string resultOffset)
    {
        var points = GetGeometryAsString(boundary);
        var query = new Dictionary<string, string>
        {
            ["where"] = "1=1",
            ["outFields"] = "WSE,WELL_NAME,MSMT_DATE,SITE_CODE,OBJECTID",
            ["f"] = "geojson",
            ["resultOffset"] = resultOffset,
            ["geometry"] = points,
            ["geometryType"] = "esriGeometryEnvelope",
            ["spatialRel"] = "esriSpatialRelIntersects",
            ["inSR"] = "4326"
        };

        var queryString = QueryHelpers.AddQueryString("/arcgis/rest/services/Geoscientific/i08_GroundwaterElevationSeasonal_Points/FeatureServer/0/query", query);
        try
        {
            var response = await _httpClient.GetAsync(queryString);

            var responseText = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<MonitoringWellJson>(responseText);

            return json;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public string GetGeometryAsString(Geometry geometry)
    {
        var points = geometry.EnvelopeInternal.MinX + "," + geometry.EnvelopeInternal.MinY + "," +
                     geometry.EnvelopeInternal.MaxX + "," + geometry.EnvelopeInternal.MaxY;
        return points;
    }
}

