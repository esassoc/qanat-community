using System;
using System.Collections.Generic;

namespace Qanat.API.Services.GET;

public class ScenarioRunResult
{
    public double AverageChangeInWaterLevel { get; set; }
    public double TotalChangeInAquiferStorage { get; set; }
    public double PercentChangeInAquiferStorage { get; set; }
    public double TotalChangeInPumping { get; set; }
    public double TotalChangeInRecharge { get; set; }
    public double? TotalChangeInGainFromStream { get; set; }

    public List<ScenarioRunResultPointOfInterest> PointsOfInterest { get; set; }
    public DateTime ModelRunEndDate { get; set; }
}