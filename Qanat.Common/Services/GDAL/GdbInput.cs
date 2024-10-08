using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Qanat.Common.Services.GDAL;

public class GdbInput
{
    public string? BlobContainer { get; set; }
    public string? CanonicalName { get; set; }
    [JsonIgnore]
    public byte[]? FileContents { get; set; }
    public IFormFile? File { get; set; }
    public string LayerName { get; set; }
    public string GeometryTypeName { get; set; }
    public int CoordinateSystemID { get; set; }
}