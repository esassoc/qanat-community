using System;

namespace Qanat.API.Services.GET.IWFM;

public class BudgetGroundwaterPeriod
{
    public DateTime Time { get; set; }
    public double Percolation { get; set; }
    public double BeginningStorage { get; set; }
    public double EndingStorage { get; set; }
    public double DeepPercolation { get; set; }
    public double GainFromStream { get; set; }
    public double GainFromLake { get; set; }
    public double BoundaryInflow { get; set; }
    public double Pumping { get; set; }
    public double OutflowToRootZone { get; set; }
    public double Recharge { get; set; }
    public double Subsidence { get; set; }
    public double SubsurfaceIrrigation { get; set; }
    public double TileDrainOutflow { get; set; }
    public double NetSubsurfaceInflow { get; set; }
    public double Discrepancy { get; set; }
    public double CumulativeSubsidence { get; set; }
}