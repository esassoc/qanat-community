using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public class fParcelStagingChanges
{
    public string ParcelNumber { get; set; }
    public string OldGeometryText { get; set; }
    public string NewGeometryText { get; set; }
    public string OldOwnerName { get; set; }
    public string NewOwnerName { get; set; }
    public string OldOwnerAddress { get; set; }
    public string NewOwnerAddress { get; set; }
    public int ParcelStatusID { get; set; }
    public bool HasOwnerNameChange { get; set; }
    public bool HasOwnerAddressChange { get; set; }
    public bool HasGeometryChange { get; set; }
    public bool HasAcresChange { get; set; }
    public bool IsNew { get; set; }
}