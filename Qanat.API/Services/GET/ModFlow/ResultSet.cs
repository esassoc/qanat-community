using System.Collections.Generic;

namespace Qanat.API.Services.GET.ModFlow;

public class ResultSet
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public List<DataSeries> DataSeries { get; set; }
}