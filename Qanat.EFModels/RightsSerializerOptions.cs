using System.Text.Json;
using System.Text.Json.Serialization;
using Qanat.Models.Security;

namespace Qanat.EFModels;

public static class RightsSerializerOptions
{
    // This serializer options is used to serialize and deserialize Rights objects in the UserDto and GeographyRoleSimpleDto
    // It uses a custom converter to handle the Rights type correctly.
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        WriteIndented = true,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        PropertyNameCaseInsensitive = false,
        PropertyNamingPolicy = null,
        Converters = { new RightsConverter() }
    };

}