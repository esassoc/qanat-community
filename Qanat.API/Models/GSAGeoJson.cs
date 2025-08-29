using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Qanat.API.Models;

public class GSAGeoJson
{
    public string type { get; set; }
    public List<GSAFeature> features { get; set; }
}

public class GSAFeature
{
    public string type { get; set; }
    public int id { get; set; }
    public Geometry geometry { get; set; }
}