using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    [Keyless]
    public class ParcelWaterSupplyAndUsage
    {
        public ParcelWaterSupplyAndUsage()
        {
        }
        
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelArea { get; set; }
        public int ParcelStatusID { get; set; }
        public decimal? TotalSupply { get; set; }
        public decimal? UsageToDate { get; set; }
        public int? WaterAccountID { get; set; }
        public string WaterAccountName { get; set; }
        public int? WaterAccountNumber { get; set; }
        public string WaterAccountPIN { get; set; }
        public int GeographyID { get; set; }
        public string GeographyName { get; set; }
        //public List<ZoneMinimalDto> Zones { get; set; }
    }
}