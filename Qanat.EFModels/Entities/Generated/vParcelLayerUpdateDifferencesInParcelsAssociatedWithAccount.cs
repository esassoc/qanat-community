using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities
{
    [Keyless]
    public partial class vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
    {
        [StringLength(255)]
        [Unicode(false)]
        public string AccountName { get; set; }
        public bool? AccountAlreadyExists { get; set; }
        [StringLength(8000)]
        [Unicode(false)]
        public string ExistingParcels { get; set; }
        [StringLength(8000)]
        [Unicode(false)]
        public string UpdatedParcels { get; set; }
        [Required]
        [StringLength(803)]
        [Unicode(false)]
        public string ExistingAddress { get; set; }
        [StringLength(8000)]
        [Unicode(false)]
        public string UpdatedAddresses { get; set; }
        public bool? AccountAddressDifferent { get; set; }
    }
}
