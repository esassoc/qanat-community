using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Qanat.EndpointTests.Services.APIHelper
{
    public class APIHelper
    {
        public static async Task<RequestResult<T>> Post<T>(string bearerToken, string route, string modelAsString)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {bearerToken}");
                    }

                    var baseAddress = AssemblySteps.Configuration["BaseAddress"];
                    httpClient.BaseAddress = new Uri($"https://{baseAddress}/");
                    var content = new StringContent(modelAsString);
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    var httpResponse = await httpClient.PostAsync(route, content);

                    RequestResult<T> result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        result = new RequestResult<T>
                        {
                            HttpResponseMessage = httpResponse,
                            HttpResponseObject = DeserializeResponse<T>(httpResponse)
                        };
                    }
                    else
                    {
                        result = new RequestResult<T>
                        {
                            HttpResponseMessage = httpResponse,
                            Errors = await httpResponse.Content.ReadAsStringAsync()
                        };
                    }

                    return result;
                }
            }
        }


        public static async Task<RequestResult<T>> Get<T>(string bearerToken, string route)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {bearerToken}");
                    }

                    var baseAddress = AssemblySteps.Configuration["BaseAddress"];
                    httpClient.BaseAddress = new Uri($"https://{baseAddress}/");
                    var httpResponse = await httpClient.GetAsync(route);

                    RequestResult<T> result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        result = new RequestResult<T>
                        {
                            HttpResponseMessage = httpResponse,
                            HttpResponseObject = DeserializeResponse<T>(httpResponse)
                        };
                    }
                    else
                    {
                        result = new RequestResult<T>
                        {
                            HttpResponseMessage = httpResponse,
                            Errors = await httpResponse.Content.ReadAsStringAsync()
                        };
                    }

                    return result;
                }
            }
        }

        public static async Task<RequestResult<T>> Put<T>(string bearerToken, string route, string modelAsString)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {bearerToken}");
                    }

                    var baseAddress = AssemblySteps.Configuration["BaseAddress"];
                    httpClient.BaseAddress = new Uri($"https://{baseAddress}/");
                    var content = new StringContent(modelAsString);
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    var httpResponse = await httpClient.PutAsync(route, content);

                    RequestResult<T> result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        result = new RequestResult<T>
                        {
                            HttpResponseMessage = httpResponse,
                            HttpResponseObject = DeserializeResponse<T>(httpResponse)
                        };
                    }
                    else
                    {
                        result = new RequestResult<T>
                        {
                            HttpResponseMessage = httpResponse,
                            Errors = await httpResponse.Content.ReadAsStringAsync()
                        };
                    }

                    return result;
                }
            }
        }

        public static async Task<RequestResult<T>> Delete<T>(string bearerToken, string route)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {bearerToken}");
                    }

                    var baseAddress = AssemblySteps.Configuration["BaseAddress"];
                    httpClient.BaseAddress = new Uri($"https://{baseAddress}/");
                    var httpResponse = await httpClient.DeleteAsync(route);

                    RequestResult<T> result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        result = new RequestResult<T>
                        {
                            HttpResponseMessage = httpResponse,
                            HttpResponseObject = DeserializeResponse<T>(httpResponse)
                        };
                    }
                    else
                    {
                        result = new RequestResult<T>
                        {
                            HttpResponseMessage = httpResponse,
                            Errors = await httpResponse.Content.ReadAsStringAsync()
                        };
                    }

                    return result;
                }
            }
        }

        private static T DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            using (var streamReader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    return JsonSerializer.Create(new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        NullValueHandling = NullValueHandling.Ignore,
                    }).Deserialize<T>(jsonTextReader);
                }
            }
        }
        public class RequestResult<T>
        {
            public HttpResponseMessage HttpResponseMessage { get; set; }
            public T HttpResponseObject { get; set; }
            public string Errors { get; set; }
        }
    }

}
