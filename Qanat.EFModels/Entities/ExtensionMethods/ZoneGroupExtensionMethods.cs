using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ZoneGroupExtensionMethods
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
            DisplayToAccountHolders = zoneGroup.DisplayToAccountHolders,
            ZoneList = zoneGroup.Zones.Select(x => x.AsZoneMinimalDto()).OrderBy(x => x.SortOrder).ToList(),
            HasAllocationPlan = zoneGroup.GeographyAllocationPlanConfigurations.Any(),
        };

        return dto;
    }

    public static ZoneGroupDto AsDto(this ZoneGroup zoneGroup)
    {
        var dto = new ZoneGroupDto()
        {
            ZoneGroupID = zoneGroup.ZoneGroupID,
            GeographyID = zoneGroup.GeographyID,
            ZoneGroupName = zoneGroup.ZoneGroupName,
            ZoneGroupSlug = zoneGroup.ZoneGroupSlug,
            ZoneGroupDescription = zoneGroup.ZoneGroupDescription,
            SortOrder = zoneGroup.SortOrder,
            DisplayToAccountHolders = zoneGroup.DisplayToAccountHolders,
            ZoneList = zoneGroup.Zones.Select(x => x.AsDetailedDto()).OrderBy(x => x.SortOrder).ToList(),
            HasAllocationPlan = zoneGroup.GeographyAllocationPlanConfigurations.Any()
        };

        return dto;
    }
}