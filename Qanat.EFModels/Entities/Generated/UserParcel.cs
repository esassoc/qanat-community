using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities
{
    [Table("UserParcel")]
    [Index("UserID", "ParcelID", Name = "AK_UserParcel_UserID_ParcelID", IsUnique = true)]
    public partial class UserParcel
    {
        [Key]
        public int UserParcelID { get; set; }
        public int UserID { get; set; }
        public int ParcelID { get; set; }
        public bool? IsClaimed { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("UserParcels")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey("UserID")]
        [InverseProperty("UserParcels")]
        public virtual User User { get; set; }
    }
}
