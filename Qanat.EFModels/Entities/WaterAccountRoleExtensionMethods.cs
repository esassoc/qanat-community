using Newtonsoft.Json;
using Qanat.Models.Security;

namespace Qanat.EFModels.Entities;

public static partial class WaterAccountRoleExtensionMethods
{
    public static Dictionary<string, Rights> AsWaterAccountRights(this WaterAccountRole waterAccountRole)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, Rights>>(waterAccountRole.Rights);
    }

    public static Dictionary<string, bool> AsWaterAccountFlags(this WaterAccountRole waterAccountRole)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, bool>>(waterAccountRole.Flags);
    }
}