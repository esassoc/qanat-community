using System.Collections.Generic;

namespace Qanat.API.Services.GET;

public class GetActionResultPointOfInterest
{
    public string Name { get; set; }
    public double? AverageValue { get; set; }
    public List<GetActionResultTimeSeriesOutput> GetActionResultTimeSeriesOutputs { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}