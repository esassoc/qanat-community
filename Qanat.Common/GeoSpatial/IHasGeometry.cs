using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace Qanat.Common.GeoSpatial;

public interface IHasGeometry
{
    [JsonIgnore]
    Geometry Geometry { get; set; }
}