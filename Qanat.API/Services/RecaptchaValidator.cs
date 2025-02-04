using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qanat.API.Services;

public class RecaptchaValidator
{
    public RecaptchaValidator()
    {
    }

    public static async Task<bool> IsValidResponseAsync(string response, string secret, string verifyURL, double scoreThreshold)
    {
        var parameters = new Dictionary<string, string> { { "secret", secret }, { "response", response } };
        var encodedContent = new FormUrlEncodedContent(parameters);

        HttpClient httpClient = new HttpClient();
        var httpResponse = await httpClient.PostAsync(verifyURL, encodedContent);
        if (httpResponse.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }

        var recaptchaResponse = await httpResponse.Content.ReadAsStringAsync();
        var recaptchaResponseJson = JsonSerializer.Deserialize<RecaptchaJson>(recaptchaResponse);

        switch (recaptchaResponseJson.Success.ToString().ToLower())
        {
            case "true":
                return recaptchaResponseJson.Score > scoreThreshold;
            case "false":
                return false;

            default:
                if (recaptchaResponseJson.ErrorCodes != null)
                {
                    throw new InvalidProgramException(
                        $"Recaptcha verification failed! Error codes \"{string.Join(",", recaptchaResponseJson.ErrorCodes)}\" from Recaptcha validation call.");
                }
                throw new InvalidProgramException(
                    $"Unknown status response \"{recaptchaResponse}\" from Recaptcha validation call.");
        }
    }
}

public class RecaptchaJson
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("score")]
    public double Score { get; set; }
    [JsonPropertyName("errorcodes")]
    public string ErrorCodes { get; set; }
}