using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities
{
    [Table("Tag")]
    [Index("TagName", Name = "AK_Tag_TagName", IsUnique = true)]
    [Index("TagID", "GeographyID", Name = "AK_Tag_Unique_TagID_GeographyID", IsUnique = true)]
    public partial class Tag
    {
        public Tag()
        {
            ParcelTagTagNavigations = new HashSet<ParcelTag>();
            ParcelTagTags = new HashSet<ParcelTag>();
        }

        [Key]
        public int TagID { get; set; }
        public int GeographyID { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string TagName { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string TagDescription { get; set; }

        [ForeignKey("GeographyID")]
        [InverseProperty("Tags")]
        public virtual Geography Geography { get; set; }
        public virtual ICollection<ParcelTag> ParcelTagTagNavigations { get; set; }
        [InverseProperty("Tag")]
        public virtual ICollection<ParcelTag> ParcelTagTags { get; set; }
    }
}
