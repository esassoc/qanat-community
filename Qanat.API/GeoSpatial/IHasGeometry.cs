using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Qanat.API.GeoSpatial
{
    public interface IHasGeometry
    {
        [JsonIgnore]
        Geometry Geometry { get; set; }
    }
}