using Qanat.Common.Services.GDAL;

namespace Qanat.Models.DataTransferObjects;

public class GDBColumnsDto
{
    public int FileResourceID { get; set; }
    public List<FeatureClassInfo> FeatureClasses { get; set; }
}