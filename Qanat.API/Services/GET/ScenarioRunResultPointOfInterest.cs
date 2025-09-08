using System.Collections.Generic;

namespace Qanat.API.Services.GET;

public class ScenarioRunResultPointOfInterest
{
    public string Name { get; set; }
    public double? AverageValue { get; set; }
    public List<ScenarioRunResultTimeSeriesOutput> ScenarioRunResultTimeSeriesOutputs { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}