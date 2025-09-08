using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;

namespace Qanat.Tests.API.Controllers.Parcel;

[TestClass]
public class ParcelByGeographyControllerTests
{
    [DataRow(1)] //MIUGSA
    [DataRow(2)] //Pajaro
    [DataRow(3)] //RRB
    [DataRow(4)] //Yolo
    [DataRow(5)] //Demo
    [DataRow(6)] //MSGSA
    [DataRow(7)] //ETSGSA
    [TestMethod]
    public async Task ListByGeographyID(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<ParcelByGeographyController>(c => c.ListByGeographyID(geographyID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.IsTrue(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());

        var resultAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(resultAsString);

        var resultAsDtos = JsonSerializer.Deserialize<List<ParcelIndexGridDto>>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.IsNotNull(resultAsDtos); // Ensure it deserialized correctly

        var parcelIDs = resultAsDtos.Select(x => x.ParcelID).ToList();
        Assert.AreEqual(parcelIDs.Count, parcelIDs.Distinct().Count());
    }

    [DataRow(1)] //MIUGSA
    [DataRow(2)] //Pajaro
    [DataRow(3)] //RRB
    [DataRow(4)] //Yolo
    [DataRow(5)] //Demo
    [DataRow(6)] //MSGSA
    [DataRow(7)] //ETSGSA
    [TestMethod]
    public async Task AdminCanListByGeographyIDForCurrentUser(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<ParcelByGeographyController>(c => c.ListByGeographyIDForCurrentUser(geographyID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.IsTrue(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());

        var resultAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(resultAsString);

        var resultAsDtos = JsonSerializer.Deserialize<List<ParcelIndexGridDto>>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.IsNotNull(resultAsDtos); // Ensure it deserialized correctly

        var parcelIDs = resultAsDtos.Select(x => x.ParcelID).ToList();
        Assert.AreEqual(parcelIDs.Count, parcelIDs.Distinct().Count());
    }
}
