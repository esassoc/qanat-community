//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ZoneGroup]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ZoneGroupExtensionMethods
    {
        public static ZoneGroupSimpleDto AsSimpleDto(this ZoneGroup zoneGroup)
        {
            var dto = new ZoneGroupSimpleDto()
            {
                ZoneGroupID = zoneGroup.ZoneGroupID,
                GeographyID = zoneGroup.GeographyID,
                ZoneGroupName = zoneGroup.ZoneGroupName,
                ZoneGroupSlug = zoneGroup.ZoneGroupSlug,
                ZoneGroupDescription = zoneGroup.ZoneGroupDescription,
                SortOrder = zoneGroup.SortOrder
            };
            return dto;
        }
    }
}