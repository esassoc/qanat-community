using System.Text.Json.Serialization;

namespace Qanat.API.Services.GET;

public class GETRunDetailModel
{
    public GETRunDetailModel(int runID, int customerID)
    {
        RunID = runID;
        //This shouldn't need to happen based on the APIM rules, but for now  we can do it manually
        CustomerID = customerID;
    }
    [JsonPropertyName("CustomerId")]
    public int CustomerID { get; set; }

    [JsonPropertyName("RunId")]
    public int RunID { get; set; }
}