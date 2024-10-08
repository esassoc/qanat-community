using System.Text.Json.Serialization;

namespace Qanat.Models.DataTransferObjects;

public class OpenETRasterExportCompositePostBody
{
    [JsonPropertyName("cog")]
    public bool Cog { get; set; }

    [JsonPropertyName("date_range")]
    public string[] DateRange { get; set; }

    [JsonPropertyName("encrypt")]
    public bool Encrypt { get; set; }

    [JsonPropertyName("geometry")]
    public double[] Geometry { get; set; }

    [JsonPropertyName("asset_id")]
    public string AssetId { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("variable")]
    public string Variable { get; set; }

    [JsonPropertyName("reference_et")]
    public string ReferenceET { get; set; }

    [JsonPropertyName("reducer")]
    public string Reducer { get; set; }

    [JsonPropertyName("units")]
    public string Units { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("drive_folder")]
    public string? DriveFolder { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("bucket")]
    public string? Bucket { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("resample")]
    public int? Resample { get; set; }

}