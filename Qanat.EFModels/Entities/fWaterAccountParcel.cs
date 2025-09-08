using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public class fWaterAccountParcel
{
    public int WaterAccountID { get; set; }
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
}