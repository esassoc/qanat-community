using System.Text.Json.Serialization;

namespace Qanat.Models.DataTransferObjects;

public class OpenETRasterExportCompositeResult
{
    [JsonPropertyName("tracking_id")]
    public string TrackingID { get; set; }

    [JsonPropertyName("encrypted")]
    public bool Encrypted { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("destination")]
    public string Destination { get; set; }
}