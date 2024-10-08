using System.Collections.Generic;

namespace Qanat.API.Services.GET.ModFlow;

public class DataSeries
{
    public string Name { get; set; }
    public bool IsDefaultDisplayed { get; set; }
    public List<DataPoint> DataPoints { get; set; }
}