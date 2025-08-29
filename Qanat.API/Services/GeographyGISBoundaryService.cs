using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Models;
using Qanat.API.Services.OpenET;
using Qanat.EFModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
namespace Qanat.API.Services;

public class GeographyGISBoundaryService
{
    private readonly ILogger<OpenETSyncService> _logger;
    private readonly QanatConfiguration _qanatConfiguration;
    private readonly QanatDbContext _qanatDbContext;
    private readonly HttpClient _httpClient;

    public GeographyGISBoundaryService(HttpClient httpClient, ILogger<OpenETSyncService> logger,
        IOptions<QanatConfiguration> qanatConfiguration, QanatDbContext qanatDbContext,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _qanatConfiguration = qanatConfiguration.Value;
        _qanatDbContext = qanatDbContext;
        _httpClient = httpClient;
    }

    public async Task RefreshGeographyGSABoundaries()
    {
        var gsaIDs = _qanatDbContext.Geographies.Include(x => x.GeographyBoundary).AsNoTracking().Where(x => x.GSACanonicalID.HasValue)
            .Select(x => (int)x.GSACanonicalID).ToList();

        var gsaGeoJson = await RetrieveGSABoundaries(gsaIDs);
        var gsaBoundaryDictionary = gsaGeoJson.features.ToDictionary(x => x.id, x => x.geometry);

        var currentDateTime = DateTime.UtcNow;
        var demoGeography = _qanatDbContext.GeographyBoundaries.Include(x => x.Geography).SingleOrDefault(x => x.Geography.IsDemoGeography);

        await _qanatDbContext.GeographyBoundaries.Include(x => x.Geography).ForEachAsync(x =>
        {
            if (!x.Geography.GSACanonicalID.HasValue || !gsaBoundaryDictionary.ContainsKey(x.Geography.GSACanonicalID.Value)) return;

            x.GSABoundary = gsaBoundaryDictionary[x.Geography.GSACanonicalID.Value];
            x.GSABoundaryLastUpdated = currentDateTime;

            // hooking Demo geography up to RRB, since their boundaries should be about the same
            // todo: don't hardcode geographyID
            if (x.GeographyID == 3 && demoGeography != null)
            {
                demoGeography.GSABoundary = gsaBoundaryDictionary[x.Geography.GSACanonicalID.Value];
                x.GSABoundaryLastUpdated = currentDateTime;
            }
        });

        await _qanatDbContext.SaveChangesAsync();
        await _qanatDbContext.Database.ExecuteSqlRawAsync("EXECUTE dbo.pMakeValidGeographyBoundaries");
    }

    private async Task<GSAGeoJson> RetrieveGSABoundaries(List<int> gsaIDs)
    {
        var query = new Dictionary<string, string>
        {
            ["objectIDs"] = string.Join(',', gsaIDs),
            ["outFields"] = "OBJECTID, GSA_NAME",
            ["geometryType"] = "esriGeometryPolygon",
            ["f"] = "geojson"
        };

        var queryString = QueryHelpers.AddQueryString("arcgis/rest/services/Boundaries/i03_Groundwater_Sustainability_Agencies/MapServer/0/query?where=", query);
        try
        {
            // todo: should be using FeatureCollection instead of GSAGeoJson
            var response = await _httpClient.GetAsync(queryString);
            var featureCollection = await response.Content.ReadFromJsonAsync<GSAGeoJson>();
            return featureCollection;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}