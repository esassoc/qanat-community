using System.Collections.Generic;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountSuggestionDto
{
    public string WaterAccountName { get; set; }
    public string ParcelNumbers { get; set; }
    public string ParcelIDList { get; set; }
    public string WellIDList { get; set; }
    public List<WellLinkDisplayDto> WellIDs { get; set; }
    public string ContactName { get; set; }
    public string ContactAddress { get; set; }
    public double ParcelArea { get; set; }
    public string Zones { get; set; }
    public List<ParcelLinkDisplayDto> Parcels { get; set; }
}