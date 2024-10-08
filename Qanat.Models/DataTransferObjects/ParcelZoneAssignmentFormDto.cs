using System.Collections.Generic;

namespace Qanat.Models.DataTransferObjects;

public class ParcelZoneAssignmentFormDto
{
    public int ParcelID { get; set; }

    public List<ParcelZoneAssignmentDto> ParcelZoneAssignments { get; set; }

    public class ParcelZoneAssignmentDto
    {
        public int ZoneGroupID { get; set; }
        public int? ZoneID { get; set; }
    }
}