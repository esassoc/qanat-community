using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public class fWaterAccountUser
{
    public int WaterAccountID { get; set; }
    public int GeographyID { get; set; }
}