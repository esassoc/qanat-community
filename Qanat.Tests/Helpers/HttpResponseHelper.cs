using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.Helpers;

public static class HttpResponseHelper
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true // Allows case-insensitive matching for JSON properties
    };

    public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage response)
    {
        var contentString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(contentString))
        {
            return default;
        }

        var t = JsonSerializer.Deserialize<T>(contentString, _jsonOptions);
        return t;
    }

    public static async Task<T> DeserializeIfSuccessAsync<T>(this HttpResponseMessage response) where T : class
    {
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var contentString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(contentString))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(contentString, _jsonOptions);
    }
}