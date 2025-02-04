using System.Text.Json.Serialization;
using Qanat.EFModels.Entities;

namespace Qanat.API.Services.GET;

public class GETNewRunModel
{
    public GETNewRunModel(int numForRunName, ScenarioRun scenarioRun, int getCustomerID, int getUserID, int getScenarioID)
    {
        Name = scenarioRun.ActionName;
        Description = "This action was automatically created by the Groundwater Accounting Platform";
        CustomerID = getCustomerID;
        UserID = getUserID;
        ModelID = scenarioRun.Model.GETModelID;
        ScenarioID = getScenarioID;
        CreateMaps = false;
        IsDifferential = true;
        //Input Volume is N/A for Custom Scenarios
        InputVolumeType = 0;
        OutputVolumeType = 1;
    }

    public int OutputVolumeType { get; set; }

    public int InputVolumeType { get; set; }

    public bool IsDifferential { get; set; }

    public bool CreateMaps { get; set; }

    [JsonPropertyName("ScenarioId")]
    public int ScenarioID { get; set; }

    [JsonPropertyName("ModelId")]
    public int ModelID { get; set; }

    [JsonPropertyName("UserId")]
    public int UserID { get; set; }

    [JsonPropertyName("CustomerId")]
    public int CustomerID { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
}