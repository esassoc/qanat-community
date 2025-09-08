using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Qanat.API.Services
{
    public static class HttpUtilities
    {
        public static async Task<byte[]> GetData(this HttpRequest httpRequest)
        {
            using var ms = new MemoryStream(2048);
            await httpRequest.Body.CopyToAsync(ms);
            var bytes = ms.ToArray();

            return bytes;
        }

        public static async Task<byte[]> GetIFormFileData(IFormFile file)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();

            return bytes;
        }
    }
}