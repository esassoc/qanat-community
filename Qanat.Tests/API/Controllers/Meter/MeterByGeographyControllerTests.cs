using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using Qanat.Tests.Helpers.EntityHelpers;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers.Meter;

[TestClass]
public class MeterByGeographyControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanAddMeter(int geographyID)
    {
        var meterGridDto = new MeterGridDto()
        {
            GeographyID = geographyID,
            SerialNumber = Guid.NewGuid().ToString().Substring(0, 25),
            MeterStatusID = MeterStatus.Active.MeterStatusID
        };

        var route = RouteHelper.GetRouteFor<MeterByGeographyController>(c => c.AddMeter(geographyID, meterGridDto));
        var json = JsonSerializer.Serialize(meterGridDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AddMeter_BadRequest_DuplicateSerialNumber(int geographyID)
    {
        var duplicateSerialNumber = Guid.NewGuid().ToString().Substring(0, 25);

        var meterA = await MeterHelper.AddMeterAsync(geographyID, duplicateSerialNumber);

        var meterGridDto = new MeterGridDto()
        {
            GeographyID = geographyID,
            SerialNumber = duplicateSerialNumber,
            MeterStatusID = MeterStatus.Active.MeterStatusID
        };

        var route = RouteHelper.GetRouteFor<MeterByGeographyController>(c => c.AddMeter(geographyID, meterGridDto));
        var json = JsonSerializer.Serialize(meterGridDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanGetMeters(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<MeterByGeographyController>(c => c.GetMeters(geographyID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanListUnassignedMeters(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<MeterByGeographyController>(c => c.ListUnassignedMeters(geographyID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanGetByID(int geographyID)
    {
        var meter = await AssemblySteps.QanatDbContext.Meters
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID);

        if (meter == null)
        {
            meter = await MeterHelper.AddMeterAsync(geographyID);
        }

        var route = RouteHelper.GetRouteFor<MeterByGeographyController>(c => c.GetByID(geographyID, meter.MeterID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanUpdateMeter(int geographyID)
    {
        var meter = await AssemblySteps.QanatDbContext.Meters
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID);

        if (meter == null)
        {
            meter = await MeterHelper.AddMeterAsync(geographyID);
        }

        var updatedSerialNumber = Guid.NewGuid().ToString().Substring(0, 25);
        var meterGridDto = new MeterGridDto()
        {
            GeographyID = geographyID,
            SerialNumber = updatedSerialNumber,
            MeterStatusID = MeterStatus.Broken.MeterStatusID
        };

        var route = RouteHelper.GetRouteFor<MeterByGeographyController>(c => c.UpdateMeter(geographyID, meter.MeterID, meterGridDto));
        var json = JsonSerializer.Serialize(meterGridDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var updatedMeter = JsonSerializer.Deserialize<MeterGridDto>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(updatedMeter.SerialNumber, updatedSerialNumber);
        Assert.AreEqual(updatedMeter.MeterStatusID, MeterStatus.Broken.MeterStatusID);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task UpdateMeter_BadRequest_DuplicateSerialNumber(int geographyID)
    {
        var meterA = await MeterHelper.AddMeterAsync(geographyID);
        var meterB = await MeterHelper.AddMeterAsync(geographyID);

        var meterGridDto = new MeterGridDto()
        {
            GeographyID = geographyID,
            SerialNumber = meterA.SerialNumber,
            MeterStatusID = MeterStatus.Broken.MeterStatusID,
        };

        var route = RouteHelper.GetRouteFor<MeterByGeographyController>(c => c.UpdateMeter(geographyID, meterB.MeterID, meterGridDto));
        var json = JsonSerializer.Serialize(meterGridDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }
}