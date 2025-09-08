using System;
using System.Collections.Generic;

namespace Qanat.Swagger.Models;

public class WellConsumerDto
{
    public int WellID { get; set; }
    public string WellName { get; set; }
    public string WellStatus { get; set; }
    public string StateWCRNumber { get; set; }
    public string CountyPermitNumber { get; set; }
    public double? WellDepth { get; set; }
    public DateOnly? DateDrilled { get; set; }
    public int? ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public List<int> IrrigatedParcelIDs { get; set; }
    public int GeographyID { get; set; }
}