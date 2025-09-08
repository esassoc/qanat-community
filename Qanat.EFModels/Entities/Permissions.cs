using System.Collections.Generic;
using System.Linq;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class Permissions
    {
        public static IEnumerable<PermissionSimpleDto> List()
        {
            return Permission.All.Select(x => x.AsSimpleDto());
        }
    }
}
