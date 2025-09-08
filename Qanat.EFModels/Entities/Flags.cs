using System.Collections.Generic;
using System.Linq;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class Flags
    {
        public static IEnumerable<FlagSimpleDto> List()
        {
            return Flag.All.Select(x => x.AsSimpleDto());
        }
    }
}