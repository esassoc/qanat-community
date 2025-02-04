using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using Qanat.Common.GeoSpatial;

namespace Qanat.Models.DataTransferObjects;

public class ParcelWithGeometryDto : IHasGeometry
{
    public int ParcelID { get; set; }
    public int GeographyID { get; set; }
    public string ParcelNumber { get; set; }
    public double ParcelArea { get; set; }
    public WaterAccountDisplayDto WaterAccount { get; set; }
    public ParcelStatusSimpleDto ParcelStatus { get; set; }
    [JsonIgnore]
    public Geometry Geometry { get; set; }
    public string GeometryWKT => Geometry.AsText();
}