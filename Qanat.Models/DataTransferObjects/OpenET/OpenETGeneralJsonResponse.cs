using System.Text.Json.Serialization;

namespace Qanat.Models.DataTransferObjects;

public class OpenETGeneralJsonResponse
{
    [JsonPropertyName("ERROR")]
    public string ErrorMessage { get; set; }
    [JsonPropertyName("SOLUTION")]
    public string SuggestedSolution { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("type")]
    public string ResponseType { get; set; }
}