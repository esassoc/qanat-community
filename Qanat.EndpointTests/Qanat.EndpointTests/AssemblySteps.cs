using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Qanat.EndpointTests
{
    [TestClass]
    public static class AssemblySteps
    {

        public static IConfigurationRoot Configuration => new ConfigurationBuilder().AddJsonFile(@"C:\git\Qanat\Qanat.EndpointTests\Qanat.EndpointTests\environment.json").Build();
        public static Dictionary<string, string> TokenDictionary = new Dictionary<string, string>();

        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext testContext)
        {
            var authorityURL = Configuration["IdentityManagement:Url"];
            var scope = Configuration["IdentityManagement:Scope"];
            var grantType = Configuration["IdentityManagement:GrantType"];

            var adminClientFlowClientIdentifier = Configuration["IdentityManagement:AdminClientFlowClientID"];
            var adminClientFlowClientSecret = Configuration["IdentityManagement:AdminClientFlowClientSecret"];

            var inactiveClientFlowClientIdentifier = Configuration["IdentityManagement:InactiveClientFlowClientID"];
            var inactiveClientFlowClientSecret = Configuration["IdentityManagement:InactiveClientFlowClientSecret"];

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(authorityURL);

                TokenDictionary.Add("Admin", GetB2CAccessToken(httpClient, adminClientFlowClientIdentifier, adminClientFlowClientSecret, scope, grantType).Result);
                TokenDictionary.Add("Inactive", GetB2CAccessToken(httpClient, inactiveClientFlowClientIdentifier, inactiveClientFlowClientSecret, scope, grantType).Result);
            }
        }

        private static async Task<string> GetB2CAccessToken(HttpClient httpClient, string clientID, string clientSecret, string scope, string grantType)
        {
            var adminContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientID),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", scope),
                new KeyValuePair<string, string>("grant_type", grantType),
            });

            var adminHttpResponse = await httpClient.PostAsync("", adminContent);
            var adminResponseContent = await adminHttpResponse.Content.ReadAsStringAsync();
            var adminResponseObject = JsonConvert.DeserializeObject<B2CTokenResponse>(adminResponseContent);
            return adminResponseObject.access_token;
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {

        }
    }

    public class B2CTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int not_before { get; set; }
        public int expires_in { get; set; }
        public int expires_on { get; set; }
        public string resource { get; set; }
    }
}
