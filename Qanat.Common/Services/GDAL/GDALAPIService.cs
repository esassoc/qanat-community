using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace Qanat.Common.Services.GDAL
{
    public class GDALAPIService
    {
        private const string mimeTypeForGdbZip = "application/x-zip-compressed";

        /// <summary>
        /// A HttpClient is registered in the Startup.cs file for this service.
        /// That is where the BaseUrl is set from the projects Configuration.
        /// </summary>
        private readonly HttpClient _httpClient;
        private readonly ILogger<GDALAPIService> _logger;

        public GDALAPIService(ILogger<GDALAPIService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task Ogr2OgrInputToGdb(GdbInputToGdbRequestDto gdbInputToGdbRequestDto)
        {
            var requestContent = gdbInputToGdbRequestDto.ToMultipartFormDataContent();
            _logger.LogInformation("Sending request to GDAL API");
            var response = await _httpClient.PostAsync("/ogr2ogr/upsert-gdb", requestContent);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed");
            }
        }

        public async Task<byte[]> Ogr2OgrInputToGdbAsZip(GdbInputsToGdbRequestDto gdbInputsToGdbRequestDto)
        {
            var requestContent = gdbInputsToGdbRequestDto.ToMultipartFormDataContent();
            _logger.LogInformation("Sending request to GDAL API");
            var response = await _httpClient.PostAsync("/ogr2ogr/upsert-gdb-as-zip", requestContent);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed");
            }
            var gdbZip = await response.Content.ReadAsByteArrayAsync();

            return gdbZip;
        }

        public async Task<byte[]> Ogr2OgrGdbToGeoJson(GdbToGeoJsonRequestDto geoJsonRequestToGdbDto)
        {
            _logger.LogInformation("Sending request to GDAL API");
            var response = await _httpClient.PostAsJsonAsync("/ogr2ogr/gdb-geojson", geoJsonRequestToGdbDto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsByteArrayAsync();
                return result;
            }

            throw new Exception($"Ogr2OgrGdbToGeoJson request failed: {response.Content}");
        }

        public async Task<int> OgrInfoGdbGetSRID(OgrInfoRequestDto ogrInfoRequestDto)
        {
            _logger.LogInformation("Sending request to GDAL API");
            var response = await _httpClient.PostAsJsonAsync("/ogrinfo/gdb-srid", ogrInfoRequestDto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<int>();
                return result;
            }

            throw new Exception("Failed to POST");
        }

        public async Task<string> OgrInfoGdbGetSRIDWKT(OgrInfoRequestDto ogrInfoRequestDto)
        {
            _logger.LogInformation("Sending request to GDAL API");
            var response = await _httpClient.PostAsJsonAsync("/ogrinfo/gdb-srid-wkt", ogrInfoRequestDto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }

            throw new Exception("Failed to POST");
        }

        public async Task<string> GdalSrsInfoGetWktForCoordinateSystem(int coordinateSystemID)
        {
            _logger.LogInformation("Sending request to GDAL API");
            var response = await _httpClient.GetAsync($"/gdalsrsinfo/epsg/{coordinateSystemID}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }

            throw new Exception("Failed to POST");
        }

        private static async Task<MultipartFormDataContent> CreateMultipartFormDataContent(FileInfo fileInfo)
        {
            using var ms = new MemoryStream();
            fileInfo.OpenRead();
            ms.Seek(0, SeekOrigin.Begin);

            var bytes = await File.ReadAllBytesAsync(fileInfo.FullName);
            var byteContent = new ByteArrayContent(bytes);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue(mimeTypeForGdbZip);

            var form = new MultipartFormDataContent();
            form.Add(byteContent, "file", fileInfo.Name);
            return form;
        }

        public async Task<List<FeatureClassInfo>> OgrInfoGdbToFeatureClassInfo(IFormFile formFile)
        {
            using var ms = new MemoryStream();
            await formFile.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var byteContent = new StreamContent(ms);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

            var form = new MultipartFormDataContent();
            form.Add(byteContent, "file", formFile.FileName);

            _logger.LogInformation("Sending request to GDAL API");

            var response = await _httpClient.PostAsync("/ogrinfo/gdb-feature-classes", form);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<FeatureClassInfo>>();
                return result;
            }
            else
            {
                throw new Exception("Failed to POST");
            }
        }

        public async Task<List<FeatureClassInfo>> OgrInfoGdbToFeatureClassInfo(OgrInfoRequestDto ogrInfoRequestDto)
        {
            _logger.LogInformation("Sending request to GDAL API");
            var response = await _httpClient.PostAsJsonAsync("/ogrinfo/gdb-feature-classes", ogrInfoRequestDto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<FeatureClassInfo>>();
                return result;
            }

            throw new Exception("Failed to POST");
        }

        public async Task<Envelope> OgrInfoGdbExtent(IFormFile formFile, string featureClassName, int? boundingBoxBufferInFeet)
        {
            using var ms = new MemoryStream();
            await formFile.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var byteContent = new StreamContent(ms);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

            var form = new MultipartFormDataContent();
            form.Add(byteContent, "file", formFile.FileName);


            _logger.LogInformation("Sending request to GDAL API");

            var response = await _httpClient.PostAsync("/ogrinfo/gdb-feature-classes", form);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Envelope>();
                return result;
            }
            else
            {
                throw new Exception("Failed to POST MyDto");
            }
        }

        public async Task<decimal> ApplyGDALWarpAndGetMeanValue(string inputTiffCanonicalName, string cutLineGeoJson)
        {
            var gdalWarpRequestDto = new GDALWarpRequestDto()
            {
                InputTiffCanonicalName = inputTiffCanonicalName,
                CutLineGeoJSON = cutLineGeoJson
            };

            var response = await _httpClient.PostAsJsonAsync("/gdalwarp/calculate-mean", gdalWarpRequestDto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<decimal>();
                return result;
            }
            else
            {
                throw new Exception("Failed to POST MyDto");
            }
        }
    }
}
