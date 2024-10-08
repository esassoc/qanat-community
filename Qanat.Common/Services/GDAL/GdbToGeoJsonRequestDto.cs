namespace Qanat.Common.Services.GDAL;

public class GdbToGeoJsonRequestDto
{
    public string BlobContainer { get; set; }
    public string CanonicalName { get; set; }
    public List<GdbLayerOutput> GdbLayerOutputs { get; set; }
}