using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class ZoneGroupExtensionMethods
{
    public static ZoneGroupMinimalDto AsZoneGroupMinimalDto(this ZoneGroup zoneGroup)
    {
        var dto = new ZoneGroupMinimalDto()
        {
            ZoneGroupID = zoneGroup.ZoneGroupID,
            GeographyID = zoneGroup.GeographyID,
            ZoneGroupName = zoneGroup.ZoneGroupName,
            ZoneGroupSlug = zoneGroup.ZoneGroupSlug,
            ZoneGroupDescription = zoneGroup.ZoneGroupDescription,
            SortOrder = zoneGroup.SortOrder,
            ZoneList = zoneGroup.Zones.Select(x => x.AsZoneMinimalDto()).OrderBy(x => x.SortOrder).ToList(),
            HasAllocationPlan = zoneGroup.GeographyAllocationPlanConfigurations.Any()
        };
        return dto;
    }
}