using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;

namespace Qanat.Tests.API.Controllers;

[TestClass]
public class GeographyControllerTests
{
    [TestMethod]
    public async Task AdminCanGetGeographyList()
    {
        var route = RouteHelper.GetRouteFor<GeographyController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultContentAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);
    }

    [TestMethod]
    [DataRow(1)] //MIUGSA
    [DataRow(2)] //Pajaro
    [DataRow(3)] //RRB
    [DataRow(4)] //Yolo
    [DataRow(5)] //Demo
    [DataRow(6)] //MSGSA
    [DataRow(7)] //ETSGSA
    [DataRow(-1)] //Bogus ID
    public async Task AdminCanGetGeographyByID(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<GeographyController>(c => c.GetGeographyByID(geographyID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultContentAsString = await result.Content.ReadAsStringAsync();

        var geographyDto = await result.DeserializeIfSuccessAsync<GeographyDto>();

        if (geographyID == -1)
        {
            Assert.IsFalse(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            Assert.IsNull(geographyDto);
            return;
        }

        Assert.AreEqual(geographyID, geographyDto.GeographyID);
        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);
    }
}
