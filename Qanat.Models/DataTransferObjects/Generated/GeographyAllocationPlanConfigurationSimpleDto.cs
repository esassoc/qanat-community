//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GeographyAllocationPlanConfiguration]

namespace Qanat.Models.DataTransferObjects
{
    public partial class GeographyAllocationPlanConfigurationSimpleDto
    {
        public int GeographyAllocationPlanConfigurationID { get; set; }
        public int GeographyID { get; set; }
        public int ZoneGroupID { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisibleToLandowners { get; set; }
        public bool? IsVisibleToPublic { get; set; }
        public string AllocationPlansDescription { get; set; }
    }
}