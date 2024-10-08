using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Qanat.Tests.IntegrationTests.OpenET;

[TestClass]
public class OpenETConnectionTest
{
    private readonly HttpClient _httpClient;

    public OpenETConnectionTest()
    {
        var openETAPIBaseURL = AssemblySteps.Configuration["OpenETApiBaseUrl"];
        var openETAPIKey = AssemblySteps.Configuration["OpenETApiKey"];

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(openETAPIBaseURL),
            Timeout = TimeSpan.FromMinutes(30)
        };

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(openETAPIKey);
    }

    [TestMethod]
    public async Task CanGetAccountStatusAndEnsureMonthlyRequestLimitDoesNotIncrease()
    {
        var firstStatusResult = await GetAccountStatus();
        Assert.IsNotNull(firstStatusResult);
        Assert.IsNotNull(firstStatusResult.MonthlyRequests);
        Assert.IsNotNull(firstStatusResult.MonthlyExportEECUSeconds);

        var secondStatusResult = await GetAccountStatus();
        Assert.IsNotNull(secondStatusResult);
        Assert.IsNotNull(secondStatusResult.MonthlyRequests);
        Assert.IsNotNull(secondStatusResult.MonthlyExportEECUSeconds);

        Assert.AreEqual(firstStatusResult.MonthlyRequests, secondStatusResult.MonthlyRequests);
        Assert.AreEqual(firstStatusResult.MonthlyExportEECUSeconds, secondStatusResult.MonthlyExportEECUSeconds);

        //Pretty print json 
        var secondStatusResultAsJson = JsonSerializer.Serialize(secondStatusResult, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(secondStatusResultAsJson);
    }

    private async Task<OpenETAccountStatusDto> GetAccountStatus()
    {
        var response = await _httpClient.GetAsync("/account/status");
        response.EnsureSuccessStatusCode();
        var responseContentAsString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OpenETAccountStatusDto>(responseContentAsString);
    }
}

public class OpenETAccountStatusDto
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Tier")]
    public int Tier { get; set; }

    [JsonPropertyName("Monthly Requests")]
    public string MonthlyRequests { get; set; }

    [JsonPropertyName("Monthly Export EECU Seconds")]
    public string MonthlyExportEECUSeconds { get; set; }

    [JsonPropertyName("Max Area Acres")]
    public int MaxAreaAcres { get; set; }

    [JsonPropertyName("Max Polygons")]
    public int MaxPolygons { get; set; }

    [JsonPropertyName("Max Field IDS")]
    public int MaxFieldIDs { get; set; }

    [JsonPropertyName("Encryption")]
    public bool Encryption { get; set; }

    [JsonPropertyName("Cloud Project ID")]
    public string CloudProjectID { get; set; }
}