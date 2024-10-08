using System;

namespace Qanat.API.Services.GET;

public class GetActionResultTimeSeriesOutput
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public double Value { get;set; }
}