using NetTopologySuite.Geometries;
using Qanat.Common.GeoSpatial;
using Qanat.Common.JsonConverters;
using System.Text.Json.Serialization;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationGdbFeature : IHasGeometry
{
    public Geometry Geometry { get; set; }
    public string APN { get; set; }
    [JsonConverter(typeof(LenientStringConverter))]
    public string UsageLocationName { get; set; }
    public string UsageLocationType { get; set; }
    public string Crop4 { get; set; }
    public string Crop3 { get; set; }
    public string Crop2 { get; set; }
    public string Crop1 { get; set; }
    public double Area { get; set; }
}