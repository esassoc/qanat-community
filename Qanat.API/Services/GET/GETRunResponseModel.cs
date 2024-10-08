using System.Text.Json.Serialization;

namespace Qanat.API.Services.GET;

public class GETRunResponseModel
{
    [JsonPropertyName("RunId")]
    public int RunID { get; set; }
    public GETRunStatus RunStatus { get; set; }
    public string Message { get; set; }
}