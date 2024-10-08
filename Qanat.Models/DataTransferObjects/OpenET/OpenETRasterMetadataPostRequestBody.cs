using System.Text.Json.Serialization;

namespace Qanat.Models.DataTransferObjects;

public class OpenETRasterMetadataPostRequestBody
{
    public OpenETRasterMetadataPostRequestBody(string variable, string[] geometry) : this("ensemble", variable, "gridMET", geometry, "monthly")
    {
    }

    public OpenETRasterMetadataPostRequestBody(string model, string variable, string referenceET, string[] geometry, string interval)
    {
        Model = model;
        Variable = variable;
        ReferenceET = referenceET;
        Geometry = geometry;
        Interval = interval;
    }

    [JsonPropertyName("interval")]
    public string Interval { get; set; }
    [JsonPropertyName("geometry")]
    public string[] Geometry { get; set; }
    [JsonPropertyName("reference_et")]
    public string ReferenceET { get; set; }
    [JsonPropertyName("variable")]
    public string Variable { get; set; }
    [JsonPropertyName("model")]
    public string Model { get; set; }
}