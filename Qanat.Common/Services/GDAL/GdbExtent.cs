using NetTopologySuite.Geometries;

namespace Qanat.Common.Services.GDAL;

public class GdbExtent
{
    public GdbExtent()
    {

    }

    public GdbExtent(double minX, double minY, double maxX, double maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    public GdbExtent(Envelope envelope)
    {
        MinX = envelope.MinX;
        MinY = envelope.MinY;
        MaxX = envelope.MaxX;
        MaxY = envelope.MaxY;
    }

    public double MinX { get; set; }
    public double MinY { get; set; }
    public double MaxX { get; set; }
    public double MaxY { get; set; }
}