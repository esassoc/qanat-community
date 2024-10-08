using System.Net.Http.Headers;

namespace Qanat.Common.Services.GDAL;

public class GdbInputsToGdbRequestDto
{
    public IList<GdbInput> GdbInputs { get; set; }
    public string GdbName { get; set; }

    public MultipartFormDataContent ToMultipartFormDataContent()
    {
        var multiPartFormDataContent = new MultipartFormDataContent();

        multiPartFormDataContent.Add(new StringContent(GdbName), "GdbName");

        if (GdbInputs.Any())
        {
            for (var i = 0; i < GdbInputs.Count(); i++)
            {
                // If a file is found, use the file contents of that file
                var gdbInput = GdbInputs[i];
                if (gdbInput.FileContents != null)
                {
                    var byteContent = new ByteArrayContent(gdbInput.FileContents);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    multiPartFormDataContent.Add(byteContent, $"GdbInputs[{i}].File", $"{gdbInput.LayerName}.json");
                }
                else // otherwise canonical name will do
                {
                    multiPartFormDataContent.Add(new StringContent(gdbInput.CanonicalName ?? "NULL"), $"GdbInputs[{i}].CanonicalName");
                }

                if (!string.IsNullOrWhiteSpace(gdbInput.BlobContainer))
                {
                    multiPartFormDataContent.Add(new StringContent(gdbInput.BlobContainer), $"GdbInputs[{i}].BlobContainer");
                }
                if (!string.IsNullOrWhiteSpace(gdbInput.LayerName))
                {
                    multiPartFormDataContent.Add(new StringContent(gdbInput.LayerName), $"GdbInputs[{i}].LayerName");
                }
                if (!string.IsNullOrWhiteSpace(gdbInput.GeometryTypeName))
                {
                    multiPartFormDataContent.Add(new StringContent(gdbInput.GeometryTypeName), $"GdbInputs[{i}].GeometryTypeName");
                }

                multiPartFormDataContent.Add(new StringContent(gdbInput.CoordinateSystemID.ToString()), $"GdbInputs[{i}].CoordinateSystemID");
            }
        }

        return multiPartFormDataContent;
    }
}