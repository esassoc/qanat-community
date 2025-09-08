using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SitkaCaptureService
{
    public class SitkaCaptureService
    {
        private static HttpClient _client { get; set; }

        public SitkaCaptureService(string baseUri)
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(baseUri)
            };
        }

        public async Task<byte[]> PrintPDF(CapturePostData postData)
        {
            var response = await _client.PostAsJsonAsync("/pdf", postData);
            var pdf = response.Content.ReadAsByteArrayAsync();
            return pdf.Result;
        }

    }
}