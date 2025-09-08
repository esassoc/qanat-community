using System.Collections.Generic;

namespace Qanat.Swagger.Models;

public class ParcelConsumerDto
{
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public double ParcelArea { get; set; }
    public string OwnerName { get; set; }
    public string OwnerAddress { get; set; }
    public int? WaterAccountID { get; set; }
    public List<ZoneConsumerDto> Zones { get; set; }
    public int GeographyID { get; set; }
}