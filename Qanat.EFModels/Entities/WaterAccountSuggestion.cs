using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public class WaterAccountSuggestion
{
    public string WaterAccountName { get; set; }
    public List<ParcelIDAndNumber> Parcels { get; set; }
    public string WellIDList { get; set; }
    public string ContactName { get; set; }
    public string ContactAddress { get; set; }
    public double ParcelArea { get; set; }
    public string Zones { get; set; }
}