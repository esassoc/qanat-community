namespace Qanat.Models.DataTransferObjects
{
    public class ParcelWaterSupplyDto
    {
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelArea { get; set; }
        public string ParcelStatusDisplayName { get; set; }
        public Dictionary<int, decimal> WaterSupplyByWaterType { get; set; }
        public string ZoneIDs { get; set; }
    }
}