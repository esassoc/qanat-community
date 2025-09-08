using NetTopologySuite.Geometries;
using Qanat.Common.GeoSpatial;

namespace Qanat.EFModels.Entities;

public class ReferenceWellStaging : IHasGeometry
{
    public Geometry Geometry { get; set; }
    public string? StateWCRNumber { get; set; }
    public string? CountyPermitNumber { get; set; }
    public int? WellDepth { get; set; }
}