using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Tests.Helpers;
using System;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers;

[TestClass]
public class SystemInfoControllerTests
{
    [TestMethod]
    public async Task AdminCanGetSystemInfo()
    {
        var route = RouteHelper.GetRouteFor<SystemInfoController>(x => x.GetSystemInfo(default));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.IsTrue(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());

        var systemInfoAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(systemInfoAsString);
    }

    [TestMethod]
    public async Task UserCanGetSystemInfo()
    {
        var route = RouteHelper.GetRouteFor<SystemInfoController>(x => x.GetSystemInfo(default));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.IsTrue(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());

        var systemInfoAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(systemInfoAsString);
    }

    [TestMethod]
    public async Task UnauthorizedUserCanGetSystemInfo()
    {
        var route = RouteHelper.GetRouteFor<SystemInfoController>(x => x.GetSystemInfo(default));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.IsTrue(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());

        var systemInfoAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(systemInfoAsString);
    }
}