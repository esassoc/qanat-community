using System.Text.Json.Serialization;

namespace Qanat.Models.DataTransferObjects;

public class RasterMetadataDateIngested : OpenETGeneralJsonResponse
{
    [JsonPropertyName("build_date")]
    public string BuildDate { get; set; }
}