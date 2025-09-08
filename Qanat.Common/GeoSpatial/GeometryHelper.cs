using NetTopologySuite.Geometries;

namespace Qanat.Common.GeoSpatial;

public static class GeometryHelper
{
    public static Geometry CreateLocationPoint4326FromLatLong(double latitude, double longitude)
    {
        return new Point(longitude, latitude) { SRID = 4326 };
    }
}