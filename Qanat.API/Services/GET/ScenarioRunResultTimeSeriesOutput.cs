using System;

namespace Qanat.API.Services.GET;

public class ScenarioRunResultTimeSeriesOutput
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public double Value { get;set; }
}