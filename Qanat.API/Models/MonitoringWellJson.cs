using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using NetTopologySuite.Features;

namespace Qanat.API.Models;

public class MonitoringWellJson
{
    public string type { get; set; }
    public List<MonitoringWells> features { get; set; }
}

public class MonitoringWells
{
    public string type { get; set; }
    public MonitoringWellsGeometry geometry { get; set; }
    public MonitoringWellProperties properties { get; set; }
}

public class MonitoringWellsGeometry
{
    public string type { get; set; }
    public List<float> coordinates { get; set; }
}
public class MonitoringWellProperties {
    public decimal? WSE { get; set; }
    public string WELL_NAME { get; set; }
    public double MSMT_DATE { get; set; }
    public DateTime MeasurementDate
    {
        get
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(MSMT_DATE.ToString())).UtcDateTime;//Convert.ToDateTime(Convert.ToDouble(feature.properties["MSMT_DATE"]));
        }
    }

    public string SITE_CODE { get; set; }
    public int OBJECTID { get; set; }
}
