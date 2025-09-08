using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;

namespace Qanat.API.Services;

public class MapboxService
{
    private readonly ILogger<MapboxService> _logger;
    private readonly QanatConfiguration _qanatConfiguration;
    private readonly QanatDbContext _dbContext;
    private readonly HttpClient _httpClient;

    public MapboxService(HttpClient httpClient, ILogger<MapboxService> logger, IOptions<QanatConfiguration> qanatConfiguration, QanatDbContext qanatDbContext, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _qanatConfiguration = qanatConfiguration.Value;
        _dbContext = qanatDbContext;
        _httpClient = httpClient;
    }

    public async Task<MapboxResponseDto> ValidateSingleAddressAsync(string addressLine1, string secondaryAddress, string city, string state, string zipCode)
    {
        var addressString = $"{addressLine1}{(secondaryAddress != null ? $", {secondaryAddress}" : "")}, {city}, {state} {zipCode}";
        var mapboxResponseDto = await ValidateSingleAddressAsync(addressString);

        return mapboxResponseDto;
    }

    /// <summary>
    /// Uses the Mapbox forward geocoding API to validate a single address
    /// </summary>
    /// <param name="addressString"></param>
    public async Task<MapboxResponseDto> ValidateSingleAddressAsync(string addressString)
    {
        var query = new Dictionary<string, string>
        {
            ["access_token"] = _qanatConfiguration.MapboxApiToken,
            ["types"] = "address,secondary_address",
            ["q"] = addressString,
            ["limit"] = "1"
        };
        var queryString = QueryHelpers.AddQueryString("forward", query);
        
        try
        {
            var response = await _httpClient.GetAsync(queryString);

            var jsonString = await response.Content.ReadAsStringAsync();

            var responseText = await response.Content.ReadAsStreamAsync();
            var geocodingResponseJson = await JsonSerializer.DeserializeAsync<GeocodingResponseJson>(responseText);

            var responseDto = GeocodingResponseJsonToMapboxResponseDto(geocodingResponseJson, jsonString);
            return responseDto;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    /// <summary>
    /// Uses the Mapbox batch geocoding API to validate a list of addresses
    /// </summary>
    /// <param name="requestDtos"></param>
    /// <returns>Returns a dictionary of MapboxResponseDtos by the passed EntityIDs</returns>
    public async Task<Dictionary<int, MapboxResponseDto>> BatchValidateAddressesAsync(List<MapboxBatchValidateAddressRequestDto> requestDtos)
    {
        var mapboxResponseDtosByEntityID = new Dictionary<int, MapboxResponseDto>();

        const int maxBatchSize = 1000; // Mapbox batch geocoding API allows a maximum of 1000 requests per batch
        var requestObjectOffset = 0;

        var query = new Dictionary<string, string>
        {
            ["access_token"] = _qanatConfiguration.MapboxApiToken
        };
        var queryString = QueryHelpers.AddQueryString("batch", query);

        while (requestDtos.Count > requestObjectOffset)
        {
            try
            {
                var requestBatch = requestDtos.Skip(requestObjectOffset).Take(maxBatchSize).ToList();
                var requestObjects = requestBatch.Select(x => new BatchGeocodingRequestJson
                {
                    AddressString = x.FullAddress,
                    ResultTypes = ["address", "secondary_address"],
                    ResultLimit = 1
                }).ToList();

                var requestJson = JsonSerializer.Serialize(requestObjects);
                var requestContent = new StringContent(requestJson);

                var mapboxResponse = await _httpClient.PostAsync(queryString, requestContent);
                if (!mapboxResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Call to Mapbox's batch geocoding endpoint failed. Status Code: {mapboxResponse.StatusCode} Message: {mapboxResponse}");
                }

                var responseJson = await mapboxResponse.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(responseJson);
                var root = document.RootElement;

                if (!root.TryGetProperty("batch", out JsonElement batchArray) ||
                    batchArray.ValueKind != JsonValueKind.Array)
                {
                    throw new InvalidOperationException("Expected a 'batch' array in the response.");
                }

                var arrayLength = batchArray.GetArrayLength();
                if (arrayLength == 0)
                {
                    throw new Exception("Mapbox call returned an empty result array.");
                }
                if (arrayLength != requestBatch.Count)
                {
                    throw new Exception("Mapbox call returned a result array that does not match number of passed request objects.");
                }

                // Mapbox returns a list of response objects in the same order as the request objects, so we can map them directly by index
                var i = 0;
                foreach (JsonElement responseObject in batchArray.EnumerateArray())
                {
                    string rawItemJson = responseObject.GetRawText();
                    var parsed = JsonSerializer.Deserialize<GeocodingResponseJson>(rawItemJson);

                    mapboxResponseDtosByEntityID.Add(requestBatch[i].EntityID, GeocodingResponseJsonToMapboxResponseDto(parsed, rawItemJson));

                    i++;
                }

                requestObjectOffset += maxBatchSize;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        return mapboxResponseDtosByEntityID;
    }

    private MapboxResponseDto? GeocodingResponseJsonToMapboxResponseDto(GeocodingResponseJson geocodingResponseJson, string responseString)
    {
        if (geocodingResponseJson == null || geocodingResponseJson.Features == null || geocodingResponseJson.Features.Count == 0)
        {
            return null;
        }

        // since we limit our requests to 1 result, we can safely take just the first feature
        var feature = geocodingResponseJson.Features.First();

        if (feature.Properties == null || feature.Properties.Context == null)
        {
            return null;
        }

        var responseDto = new MapboxResponseDto
        {
            Address = feature.Properties.Context.Address?.Name,
            SecondaryAddress = feature.Properties.Context.SecondaryAddress?.Name,
            City = feature.Properties.Context.City?.Name,
            State = feature.Properties.Context.State?.Name,
            StatePostalCode = feature.Properties.Context.State?.StatePostalCode,
            ZipCode = feature.Properties.Context.ZipCode?.Name,
            Confidence = feature.Properties.MatchCode?.Confidence,
            ResponseJsonString = responseString
        };

        return responseDto;
    }

    // JQ 8/15/25: Putting this here for now, but could be moved to its own service if/when MapboxService gets more users
    public async Task<MapboxBulkResponseDto> BatchValidateWaterAccountContactAddresses(int geographyID, List<int> waterAccountContactIDs)
    {
        // batching to avoid SQL Server's parameter limit (2100 parameters per query) and to keep queries performant
        const int batchSize = 1000;
        var waterAccountContacts = new List<WaterAccountContact>();

        foreach (var batch in waterAccountContactIDs.Chunk(batchSize))
        {
            var batchResults = await _dbContext.WaterAccountContacts
                .Where(x => x.GeographyID == geographyID && batch.Contains(x.WaterAccountContactID))
                .ToListAsync();

            waterAccountContacts.AddRange(batchResults);
        }

        if (waterAccountContacts.Count != waterAccountContactIDs.Count)
        {
            throw new Exception("Some of the provided water account contact IDs were not found within the provided geography.");
        }

        var mapboxRequestDtos = waterAccountContacts.Select(x => new MapboxBatchValidateAddressRequestDto()
        {
            EntityID = x.WaterAccountContactID,
            FullAddress = x.FullAddress
        }).ToList();

        var mapboxResponseDtosByWaterAccountContactID = await BatchValidateAddressesAsync(mapboxRequestDtos);

        var permittedConfidenceLevels = new List<ConfidenceEnum> { ConfidenceEnum.Exact, ConfidenceEnum.High };

        var waterAccountContactsWithAddressValidationUpdates = waterAccountContacts
            .Where(x => mapboxResponseDtosByWaterAccountContactID[x.WaterAccountContactID] != null 
                        && mapboxResponseDtosByWaterAccountContactID[x.WaterAccountContactID].Confidence.HasValue 
                        && permittedConfidenceLevels.Contains(mapboxResponseDtosByWaterAccountContactID[x.WaterAccountContactID].Confidence.Value)).ToList();

        foreach (var waterAccountContact in waterAccountContactsWithAddressValidationUpdates)
        {
            if (!mapboxResponseDtosByWaterAccountContactID.ContainsKey(waterAccountContact.WaterAccountContactID))
            { 
                throw new Exception("Mapbox response does not contain a result for every request object, or there was an error parsing results.");
            }

            waterAccountContact.Address = mapboxResponseDtosByWaterAccountContactID[waterAccountContact.WaterAccountContactID].Address;
            waterAccountContact.SecondaryAddress = mapboxResponseDtosByWaterAccountContactID[waterAccountContact.WaterAccountContactID].SecondaryAddress;
            waterAccountContact.City = mapboxResponseDtosByWaterAccountContactID[waterAccountContact.WaterAccountContactID].City;
            waterAccountContact.State = mapboxResponseDtosByWaterAccountContactID[waterAccountContact.WaterAccountContactID].StatePostalCode;
            waterAccountContact.ZipCode = mapboxResponseDtosByWaterAccountContactID[waterAccountContact.WaterAccountContactID].ZipCode;

            waterAccountContact.AddressValidated = true;
            waterAccountContact.AddressValidationJson = mapboxResponseDtosByWaterAccountContactID[waterAccountContact.WaterAccountContactID].ResponseJsonString;
        }

        await _dbContext.SaveChangesAsync();

        var responseDto = new MapboxBulkResponseDto()
        {
            ValidatedAddressesCount = waterAccountContactsWithAddressValidationUpdates.Count,
            UnchangedAddressesCount = waterAccountContactIDs.Count - waterAccountContactsWithAddressValidationUpdates.Count
        };

        return responseDto;
    }

    private class BatchGeocodingRequestJson
    {
        [JsonPropertyName("q")]
        public string AddressString { get; set; }
        

        [JsonPropertyName("types")]
        public List<string> ResultTypes { get; set; }

        [JsonPropertyName("limit")]
        public int ResultLimit { get; set; }
    }

    private class GeocodingResponseJson
    {
        [JsonPropertyName("features")]
        public List<GeocodingResponseFeatures> Features { get; set; }
    }

    private class GeocodingResponseFeatures
    {
        [JsonPropertyName("properties")]
        public GeocodingResponseProperties Properties { get; set; }
    }

    private class GeocodingResponseProperties
    {
        [JsonPropertyName("match_code")]
        public GeocodingResponseMatchCode MatchCode { get; set; }

        [JsonPropertyName("context")]
        public GeocodingResponseContext Context { get; set; }
    }

    private class GeocodingResponseMatchCode
    {
        [JsonPropertyName("confidence")]
        public ConfidenceEnum Confidence { get; set; }
    }

    public class GeocodingResponseContext
    {
        [JsonPropertyName("address")]
        public GeocodingResponseContextFieldValue Address { get; set; }

        [JsonPropertyName("secondary_address")]
        public GeocodingResponseContextFieldValue SecondaryAddress { get; set; }

        [JsonPropertyName("place")]
        public GeocodingResponseContextFieldValue City { get; set; }

        [JsonPropertyName("region")]
        public GeocodingResponseContextStateValue State { get; set; }

        [JsonPropertyName("postcode")]
        public GeocodingResponseContextFieldValue ZipCode { get; set; }
    }

    public class GeocodingResponseContextFieldValue
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class GeocodingResponseContextStateValue : GeocodingResponseContextFieldValue
    {
        [JsonPropertyName("region_code")]
        public string StatePostalCode { get; set; } 
    }
}

public class MapboxBatchValidateAddressRequestDto
{
    public int EntityID { get; set; }
    public string FullAddress { get; set; }
}

public class MapboxResponseDto
{
    public string Address { get; set; }
    public string SecondaryAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string StatePostalCode { get; set; }
    public string ZipCode { get; set; }
    public ConfidenceEnum? Confidence { get; set; }
    public string ResponseJsonString { get; set; }
}

public class MapboxBulkResponseDto
{
    public int ValidatedAddressesCount { get; set; }
    public int UnchangedAddressesCount { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConfidenceEnum
{
    [EnumMember(Value = "exact")]
    Exact,

    [EnumMember(Value = "high")]
    High,

    [EnumMember(Value = "medium")]
    Medium,

    [EnumMember(Value = "low")]
    Low
}

