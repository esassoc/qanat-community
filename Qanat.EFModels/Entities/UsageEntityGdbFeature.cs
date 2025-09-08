using NetTopologySuite.Geometries;
using Qanat.Common.GeoSpatial;

namespace Qanat.EFModels.Entities;

public class UsageEntityGdbFeature : IHasGeometry
{
    public Geometry Geometry { get; set; }
}

public class ETSGSAUsageEntityGdbFeature : UsageEntityGdbFeature
{
    public string uniqueid { get; set; }
    public string fieldid { get; set; }
    public string apn { get; set; }
    public string crop4_landiq { get; set; }
    public string crop3_landiq { get; set; }
    public string crop2_landiq { get; set; }
    public string crop1_landiq { get; set; }
    public double acres { get; set; }
}