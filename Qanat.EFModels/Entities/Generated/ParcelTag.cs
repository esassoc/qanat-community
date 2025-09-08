using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities
{
    [Table("ParcelTag")]
    public partial class ParcelTag
    {
        [Key]
        public int ParcelTagID { get; set; }
        public int GeographyID { get; set; }
        public int ParcelID { get; set; }
        public int TagID { get; set; }

        [ForeignKey("GeographyID")]
        [InverseProperty("ParcelTags")]
        public virtual Geography Geography { get; set; }
        [ForeignKey("ParcelID")]
        [InverseProperty("ParcelTagParcels")]
        public virtual Parcel Parcel { get; set; }
        public virtual Parcel ParcelNavigation { get; set; }
        [ForeignKey("TagID")]
        [InverseProperty("ParcelTagTags")]
        public virtual Tag Tag { get; set; }
        public virtual Tag TagNavigation { get; set; }
    }
}
