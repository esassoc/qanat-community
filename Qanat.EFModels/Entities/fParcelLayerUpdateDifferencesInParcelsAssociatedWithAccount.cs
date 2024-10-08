using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities
{
    [Keyless]
    public class fParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
    {
        public string AccountName { get; set; }
        public string OwnerAddress { get; set; }
        public bool? AccountAlreadyExists { get; set; }
        public string ExistingParcels { get; set; }
        public string UpdatedParcels { get; set; }
        public string ExistingAddress { get; set; }
    }
}
