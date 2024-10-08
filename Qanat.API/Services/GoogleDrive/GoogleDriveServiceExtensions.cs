using System.Threading.Tasks; 
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace Qanat.API.Services.GoogleDrive;

public static class GoogleDriveServiceExtensions
{
    public static async Task<FileList> ListFiles(this DriveService driveService, int? pageSize = null)
    {
        var listRequest = driveService.Files.List();
        listRequest.Fields = "files(id, name, mimeType, size)";
        listRequest.Q = "trashed=false";
        listRequest.PageSize = pageSize;

        var files = await listRequest.ExecuteAsync();
        return files;
    }
}
