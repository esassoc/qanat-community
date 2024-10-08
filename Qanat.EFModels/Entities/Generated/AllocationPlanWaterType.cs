using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities
{
    [Table("AllocationPlanWaterType")]
    [Index("GeographyAllocationPlanID", "WaterTypeID", Name = "AK_AllocationPlanWaterType_AllocationPlanID_WaterTypeID", IsUnique = true)]
    public partial class AllocationPlanWaterType
    {
        [Key]
        public int AllocationPlanWaterTypeID { get; set; }
        public int AllocationPlanID { get; set; }
        public int WaterTypeID { get; set; }

        [ForeignKey("GeographyAllocationPlanID")]
        [InverseProperty("AllocationPlanWaterTypes")]
        public virtual AllocationPlan AllocationPlan { get; set; }
        [ForeignKey("WaterTypeID")]
        [InverseProperty("AllocationPlanWaterTypes")]
        public virtual WaterType WaterType { get; set; }
    }
}
