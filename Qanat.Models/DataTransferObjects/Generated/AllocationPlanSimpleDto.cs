//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AllocationPlan]

namespace Qanat.Models.DataTransferObjects
{
    public partial class AllocationPlanSimpleDto
    {
        public int AllocationPlanID { get; set; }
        public int GeographyID { get; set; }
        public int GeographyAllocationPlanConfigurationID { get; set; }
        public int ZoneID { get; set; }
        public int WaterTypeID { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}