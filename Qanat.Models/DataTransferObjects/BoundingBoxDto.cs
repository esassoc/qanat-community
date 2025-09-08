using NetTopologySuite.Geometries;

namespace Qanat.Models.DataTransferObjects
{
    public class BoundingBoxDto
    {
        public double Left { get; set; }
        public double Bottom { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }

        public BoundingBoxDto()
        {
            Left = -119.11015104115182;
            Top = 35.442022035628575;
            Right = -119.45272037350193;
            Bottom = 35.27608156273151;
        }

        public BoundingBoxDto(IReadOnlyCollection<Point> pointList)
        {
            if (pointList.Any())
            {
                Left = pointList.Min(x => x.X);
                Right = pointList.Max(x => x.X);
                Bottom = pointList.Min(x => x.Y);
                Top = pointList.Max(x => x.Y);
            }
            else
            {
                Left = -119.11015104115182;
                Top = 35.442022035628575;
                Right = -119.45272037350193;
                Bottom = 35.27608156273151;
            }
        }


        public BoundingBoxDto(IEnumerable<Geometry> geometries) : this(geometries.SelectMany(GetPointsFromDbGeometry).ToList())
        {
        }

        public static List<Point> GetPointsFromDbGeometry(Geometry geometry)
        {
            var pointList = new List<Point>();
            var envelope = geometry.EnvelopeInternal;
            pointList.Add(new Point(envelope.MinX, envelope.MinY));
            pointList.Add(new Point(envelope.MaxX, envelope.MaxY));
            return pointList;
        }
    }
}