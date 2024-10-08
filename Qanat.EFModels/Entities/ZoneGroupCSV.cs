using CsvHelper.Configuration;

namespace Qanat.EFModels.Entities;

public class ZoneGroupCSV
{
    public string APN { get; set; }
    public string? Zone { get; set; }
}

public sealed class ZoneGroupCSVMap : ClassMap<ZoneGroupCSV>
{
    public ZoneGroupCSVMap(string apnColumnName, string zoneColumnName)
    {
        Map(m => m.APN).Name(apnColumnName);
        Map(m => m.Zone).Name(zoneColumnName);
    }
}