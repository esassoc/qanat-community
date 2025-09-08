namespace Qanat.Common.Services.GDAL;

public class GdbLayerOutput
{
    public string FeatureLayerName { get; set; }
    public string Filter { get; set; }
    public int NumberOfSignificantDigits { get; set; }
    public List<string> Columns { get; set; }
    public int CoordinateSystemID { get; set; }
    public GdbExtent? Extent { get; set; }
}