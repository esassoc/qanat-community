using System.Collections.Generic;

namespace Qanat.Models.DataTransferObjects;

public class ParcelWaterSupplyBreakdownDto
{
    public int ParcelID { get; set; }
    public Dictionary<int, decimal> WaterSupplyByWaterType { get; set; }
}