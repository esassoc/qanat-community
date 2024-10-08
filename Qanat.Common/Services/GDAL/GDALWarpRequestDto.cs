namespace Qanat.Common.Services.GDAL;

public class GDALWarpRequestDto {
    public string InputTiffCanonicalName { get; set; }
    public string CutLineGeoJSON { get; set; }
    public string BlobContainer { get; set; }
}