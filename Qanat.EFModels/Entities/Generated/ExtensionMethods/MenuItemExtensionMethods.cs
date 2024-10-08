//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MenuItem]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class MenuItemExtensionMethods
    {
        public static MenuItemSimpleDto AsSimpleDto(this MenuItem menuItem)
        {
            var dto = new MenuItemSimpleDto()
            {
                MenuItemID = menuItem.MenuItemID,
                MenuItemName = menuItem.MenuItemName,
                MenuItemDisplayName = menuItem.MenuItemDisplayName
            };
            return dto;
        }
    }
}