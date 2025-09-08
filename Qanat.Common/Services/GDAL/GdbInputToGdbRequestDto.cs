using System.Net.Http.Headers;

namespace Qanat.Common.Services.GDAL;

public class GdbInputToGdbRequestDto
{
    public GdbInput GdbInput { get; set; }
    public string GdbName { get; set; }

    public MultipartFormDataContent ToMultipartFormDataContent()
    {
        var multiPartFormDataContent = new MultipartFormDataContent();

        multiPartFormDataContent.Add(new StringContent(GdbName), "GdbName");

        // If a file is found, use the file contents of that file
        var gdbInput = GdbInput;
        if (gdbInput.FileContents != null)
        {
            var byteContent = new ByteArrayContent(gdbInput.FileContents);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            multiPartFormDataContent.Add(byteContent, "GdbInput.File", $"{gdbInput.LayerName}.json");
        }
        else // otherwise canonical name will do
        {
            multiPartFormDataContent.Add(new StringContent(gdbInput.CanonicalName ?? "NULL"),
                $"GdbInput.CanonicalName");
        }

        if (!string.IsNullOrWhiteSpace(gdbInput.BlobContainer))
        {
            multiPartFormDataContent.Add(new StringContent(gdbInput.BlobContainer),
                $"GdbInput.BlobContainer");
        }

        if (!string.IsNullOrWhiteSpace(gdbInput.LayerName))
        {
            multiPartFormDataContent.Add(new StringContent(gdbInput.LayerName), "GdbInput.LayerName");
        }

        if (!string.IsNullOrWhiteSpace(gdbInput.GeometryTypeName))
        {
            multiPartFormDataContent.Add(new StringContent(gdbInput.GeometryTypeName),
                $"GdbInput.GeometryTypeName");
        }

        multiPartFormDataContent.Add(new StringContent(gdbInput.CoordinateSystemID.ToString()),
            $"GdbInput.CoordinateSystemID");

        return multiPartFormDataContent;
    }
}