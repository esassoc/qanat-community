namespace Qanat.EFModels.Entities;

public partial class Well
{
    public double? Longitude => LocationPoint4326?.Coordinate.X;
    public double? Latitude => LocationPoint4326?.Coordinate.Y;
}