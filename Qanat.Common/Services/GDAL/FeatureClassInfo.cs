namespace Qanat.Common.Services.GDAL;

public class FeatureClassInfo
{
    public string LayerName { get; set; }
    public string FeatureType { get; set; }
    public int FeatureCount { get; set; }
    public List<string> Columns { get; set; }
}