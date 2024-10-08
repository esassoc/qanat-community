using System.Collections.Generic;
using System.Linq;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class Roles
    {
        public static IEnumerable<RoleDto> List()
        {
            return Role.All.Select(x => x.AsRoleDto());
        }
    }
}
