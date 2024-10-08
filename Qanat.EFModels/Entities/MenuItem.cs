
using System.Collections.Generic;
using System.Linq;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class MenuItems
    {
        public static IEnumerable<MenuItemSimpleDto> List()
        {
            return MenuItem.All.Select(x => x.AsSimpleDto());
        }
    }
}
