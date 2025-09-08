using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Qanat.API.Services.GET.ModFlow;

public class RunResult
{
    [JsonPropertyName("RunResultId")]
    public int RunResultID { get; set; }
    public string RunResultName { get; set; }
    public List<ResultSet> ResultSets { get; set; }
}