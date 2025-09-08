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
public class MeterByWellControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanAddWellMeter(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);

        var addWellMeterRequestDto = new AddWellMeterRequestDto
        {
            WellID = well.WellID,
            MeterID = meter.MeterID,
            StartDate = DateTime.UtcNow
        };

        var route = RouteHelper.GetRouteFor<MeterByWellController>(c => c.AddWellMeter(geographyID, addWellMeterRequestDto));
        var json = JsonSerializer.Serialize(addWellMeterRequestDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AddWellMeter_BadRequest(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);

        await AssemblySteps.QanatDbContext.WellMeters.AddAsync(new WellMeter
        {
            WellID = well.WellID,
            MeterID = meter.MeterID,
            StartDate = DateTime.UtcNow
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var addWellMeterRequestDto = new AddWellMeterRequestDto
        {
            WellID = well.WellID,
            MeterID = meter.MeterID,
            StartDate = DateTime.UtcNow
        };

        var route = RouteHelper.GetRouteFor<MeterByWellController>(c => c.AddWellMeter(geographyID, addWellMeterRequestDto));
        var json = JsonSerializer.Serialize(addWellMeterRequestDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanGetWellMeterByWellID(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);

        var date = DateTime.UtcNow;
        await AssemblySteps.QanatDbContext.WellMeters.AddAsync(new WellMeter
        {
            WellID = well.WellID,
            MeterID = meter.MeterID,
            StartDate = date
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var route = RouteHelper.GetRouteFor<MeterByWellController>(c => c.GetWellMeterByWellID(well.WellID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var wellMeterDto = JsonSerializer.Deserialize<WellMeterDto>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(well.WellID, wellMeterDto.WellID);
        Assert.AreEqual(meter.MeterID, wellMeterDto.MeterID);
        Assert.AreEqual(date.ToString("s"), wellMeterDto.StartDate.ToString("s"));
    }

    [DataRow(5)]
    [TestMethod]
    public async Task GetWellMeterByWellID_NotFound(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var route = RouteHelper.GetRouteFor<MeterByWellController>(c => c.GetWellMeterByWellID(well.WellID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanRemoveWellMeter(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);

        var date = DateTime.UtcNow;
        await AssemblySteps.QanatDbContext.WellMeters.AddAsync(new WellMeter
        {
            WellID = well.WellID,
            MeterID = meter.MeterID,
            StartDate = date
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var endDate = date.AddMonths(1);
        var removeWellMeterRequestDto = new RemoveWellMeterRequestDto
        {
            WellID = well.WellID,
            MeterID = meter.MeterID,
            EndDate = endDate
        };

        var route = RouteHelper.GetRouteFor<MeterByWellController>(c => c.RemoveWellMeter(well.WellID, meter.MeterID, removeWellMeterRequestDto));
        var json = JsonSerializer.Serialize(removeWellMeterRequestDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task RemoveWellMeter_BadRequest_NoWellMeter(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);

        var removeWellMeterRequestDto = new RemoveWellMeterRequestDto
        {
            WellID = well.WellID,
            MeterID = meter.MeterID,
            EndDate = DateTime.UtcNow
        };

        var route = RouteHelper.GetRouteFor<MeterByWellController>(c => c.RemoveWellMeter(well.WellID, meter.MeterID, removeWellMeterRequestDto));
        var json = JsonSerializer.Serialize(removeWellMeterRequestDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task RemoveWellMeter_BadRequest_EndDateBeforeStartDate(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var date = DateTime.UtcNow;

        await AssemblySteps.QanatDbContext.WellMeters.AddAsync(new WellMeter
        {
            WellID = well.WellID,
            MeterID = meter.MeterID,
            StartDate = date
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var endDate = date.AddMonths(-1);
        var removeWellMeterRequestDto = new RemoveWellMeterRequestDto
        {
            WellID = well.WellID,
            MeterID = meter.MeterID,
            EndDate = endDate
        };

        var route = RouteHelper.GetRouteFor<MeterByWellController>(c => c.RemoveWellMeter(well.WellID, meter.MeterID, removeWellMeterRequestDto));
        var json = JsonSerializer.Serialize(removeWellMeterRequestDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }
}