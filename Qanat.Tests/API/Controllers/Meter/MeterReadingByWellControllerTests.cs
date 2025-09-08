using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using Qanat.Tests.Helpers.EntityHelpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers.Meter;

[TestClass]
public class MeterReadingByWellControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanListMeterReadingsByWell(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);
        var meterReading = await MeterReadingHelper.AddMeterReadingAsync(geographyID, well.WellID, meter.MeterID, DateTime.UnixEpoch, "12:00", MeterReadingUnitType.Gallons.MeterReadingUnitTypeID, 100, 250);

        var route = RouteHelper.GetRouteFor<MeterReadingByWellController>(c => c.ListMeterReadingsByWell(geographyID, well.WellID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        
        var meterReadingDtos = JsonSerializer.Deserialize<List<MeterReadingDto>>(resultAsString);
        Assert.AreEqual(1, meterReadingDtos.Count);
        Assert.AreEqual(meterReading.MeterReadingID, meterReadingDtos[0].MeterReadingID);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanListMonthlyInterpolationsByWell(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);
        var meterReadingA = await MeterReadingHelper.AddMeterReadingAsync(geographyID, well.WellID, meter.MeterID, DateTime.Parse("2025-1-1"), "12:00", MeterReadingUnitType.Gallons.MeterReadingUnitTypeID, 100, 250);
        var meterReadingB = await MeterReadingHelper.AddMeterReadingAsync(geographyID, well.WellID, meter.MeterID, DateTime.Parse("2025-1-31"), "12:00", MeterReadingUnitType.Gallons.MeterReadingUnitTypeID, 250, 500);

        var monthlyInterpolations = await MeterReadingMonthlyInterpolations.RebuildMonthlyInterpolationsAsync(AssemblySteps.QanatDbContext, meter.MeterID);
        Assert.AreEqual(1, monthlyInterpolations.Count);

        var route = RouteHelper.GetRouteFor<MeterReadingByWellController>(c => c.ListMonthlyInterpolationsByWell(geographyID, well.WellID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);

        var monthlyInterpolationDtos = JsonSerializer.Deserialize<List<MeterReadingMonthlyInterpolationSimpleDto>>(resultAsString);
        Assert.AreEqual(1, monthlyInterpolationDtos.Count);

        var monthlyInterpolation = monthlyInterpolations[0];
        var monthlyInterpolationDto = monthlyInterpolationDtos[0];
        Assert.AreEqual(meter.MeterID, monthlyInterpolationDto.MeterID);
        Assert.AreEqual(meterReadingA.Volume + meterReadingB.Volume, monthlyInterpolationDto.InterpolatedVolume);
        Assert.AreEqual(monthlyInterpolation.Date.ToString("s"), monthlyInterpolationDto.Date.ToString("s"));
        Assert.AreEqual(monthlyInterpolation.InterpolatedVolume, monthlyInterpolationDto.InterpolatedVolume);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task ListMonthlyInterpolationsByWell_NoCurrentMeter(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var route = RouteHelper.GetRouteFor<MeterReadingByWellController>(c => c.ListMonthlyInterpolationsByWell(geographyID, well.WellID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }
}