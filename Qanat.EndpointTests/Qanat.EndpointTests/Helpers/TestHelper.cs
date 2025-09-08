using Qanat.EndpointTests.Services.APIHelper;
using Qanat.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qanat.EndpointTests.Helpers
{
    public static class TestHelper
    {
        public static async Task<List<UserDto>> GetUsers(string bearerToken)
        {
            var route = "users";

            var result = await APIHelper.Get<List<UserDto>>(bearerToken, route);

            return result.HttpResponseMessage.IsSuccessStatusCode ? result.HttpResponseObject : null;
        }

        public static async Task<List<WatershedDto>> GetWatersheds(string bearerToken)
        {
            var route = "watersheds";

            var result = await APIHelper.Get<List<WatershedDto>>(bearerToken, route);

            return result.HttpResponseMessage.IsSuccessStatusCode ? result.HttpResponseObject : null;
        }

    }
}
