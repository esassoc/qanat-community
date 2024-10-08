using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities
{
    [Table("PendingWaterAccountUser")]
    [Index("WaterAccountID", "Email", Name = "AK_PendingWaterAccountUser_Unique_WaterAccountID_Email", IsUnique = true)]
    public partial class PendingWaterAccountUser
    {
        [Key]
        public int PendingWaterAccountUserID { get; set; }
        public int WaterAccountID { get; set; }
        public int WaterAccountRoleID { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string Email { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastEmailSent { get; set; }

        [ForeignKey("WaterAccountID")]
        [InverseProperty("PendingWaterAccountUsers")]
        public virtual WaterAccount WaterAccount { get; set; }
    }
}
