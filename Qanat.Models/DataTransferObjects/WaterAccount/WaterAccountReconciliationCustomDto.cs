using System.Collections.Generic;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountReconciliationCustomDto
{
    public ParcelMinimalDto Parcel { get; set; }
    public WaterAccountMinimalDto LastKnownOwner { get; set; }
    public List<WaterAccountMinimalDto> AccountsClaimingOwnership { get; set; }
}