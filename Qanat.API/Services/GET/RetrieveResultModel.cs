using System.Text.Json.Serialization;

namespace Qanat.API.Services.GET;

public class RetrieveResultModel
{
    public RetrieveResultModel(int getRunID, int getCustomerID, string fileName, string fileExtension)
    {
        RunID = getRunID;
        CustomerID = getCustomerID;
        FileName = fileName;
        FileExtension = fileExtension;
    }

    public RetrieveResultModel()
    {
    }

    [JsonPropertyName("RunId")]
    public int? RunID { get; set; }

    [JsonPropertyName("CustomerId")]
    public int? CustomerID { get; set; }

    public string FileName { get; set; }

    public string FileDate { get; set; }

    public string SubType { get; set; }

    public string FileExtension { get; set; }
}