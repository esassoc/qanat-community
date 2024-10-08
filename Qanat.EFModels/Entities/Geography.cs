using NetTopologySuite.Operation.Buffer;
using Qanat.Common.GeoSpatial;

namespace Qanat.EFModels.Entities;

public partial class Geography
{
    public double[] GetBoundaryAsDoubleArray(string targetWKT)
    {
        var envelope = GeographyBoundary.BoundingBox.ProjectToSrid(CoordinateSystem, targetWKT).ProjectTo4326(targetWKT).EnvelopeInternal;
        var geometryArray = new[]
        {
            envelope.MinX, envelope.MaxY,
            envelope.MaxX, envelope.MaxY,
            envelope.MaxX, envelope.MinY,
            envelope.MinX, envelope.MinY
        };

        return geometryArray;
    }

    public string[] GetBoundaryAsStringArray(string targetWKT)
    {
        var doubles = GetBoundaryAsDoubleArray(targetWKT);
        var geometryArray = doubles.Select(x => x.ToString()).ToArray();
        return geometryArray;
    }
}