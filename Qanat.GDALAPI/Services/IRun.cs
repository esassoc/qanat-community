using Qanat.Common;

namespace Qanat.GDALAPI.Services;

public interface IRun
{
    public ProcessUtilityResult Ogr2Ogr(List<string> arguments);
}