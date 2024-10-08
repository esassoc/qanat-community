using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities
{
    [Keyless]
    public partial class vGeoServerAllWell
    {
        public int PrimaryKey { get; set; }
        public int GeographyID { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string WellName { get; set; }
        public int? WellDepth { get; set; }
        [Column(TypeName = "geometry")]
        public Geometry LocationPoint { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string StateWCRNumber { get; set; }
        public int WellStatusID { get; set; }
    }
}
