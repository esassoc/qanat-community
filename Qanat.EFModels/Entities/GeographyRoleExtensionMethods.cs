using Newtonsoft.Json;
using Qanat.Models.Security;

namespace Qanat.EFModels.Entities;

public static partial class GeographyRoleExtensionMethods
{
    public static Dictionary<string, Rights> AsGeographyRights(this GeographyRole geographyRole)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, Rights>>(geographyRole.Rights);
    }
    public static Dictionary<string, bool> AsGeographyFlags(this GeographyRole geographyRole)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, bool>>(geographyRole.Flags);
    }
}