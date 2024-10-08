using System.Collections.Generic;
using Qanat.Common.Services.GDAL;

namespace Qanat.Models.DataTransferObjects;

public class UploadParcelLayerInfoDto
{
    public int UploadedGdbID { get; set; }
    public List<FeatureClassInfo> FeatureClasses { get; set; }
}