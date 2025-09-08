using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Common.Util;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using Qanat.Tests.Helpers.EntityHelpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers.Meter;

[TestClass]
public class MeterReadingByMeterControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanCreateMeterReading(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var meterReadingUpsertDto = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.UtcNow,
            ReadingTime = "12:00",
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var json = JsonSerializer.Serialize(meterReadingUpsertDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, well.WellID, meter.MeterID, meterReadingUpsertDto));
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var meterReadingDto = JsonSerializer.Deserialize<MeterReadingDto>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(150, meterReadingDto.Volume);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task CreateMeterReading_BadRequest_MeterNotAssociatedToSelectedWell(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);

        var meterReadingUpsertDto = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.UtcNow,
            ReadingTime = "12:00",
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var json = JsonSerializer.Serialize(meterReadingUpsertDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, well.WellID, meter.MeterID, meterReadingUpsertDto));
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task CreateMeterReading_BadRequest_ReadingDateBeforeWellMeterStartDate(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UtcNow);

        var meterReadingUpsertDto = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.UnixEpoch,
            ReadingTime = "12:00",
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var json = JsonSerializer.Serialize(meterReadingUpsertDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, well.WellID, meter.MeterID, meterReadingUpsertDto));
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task CreateMeterReading_BadRequest_DuplicateDate(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UtcNow);
        var meterReadingA = await MeterReadingHelper.AddMeterReadingAsync(geographyID, well.WellID, meter.MeterID, DateTime.UtcNow, "12:00", MeterReadingUnitType.Gallons.MeterReadingUnitTypeID, 100, 250);

        var meterReadingUpsertDto = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.UtcNow,
            ReadingTime = "12:00",
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var json = JsonSerializer.Serialize(meterReadingUpsertDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, well.WellID, meter.MeterID, meterReadingUpsertDto));
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanGetMeterReadingByID(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);
        var meterReading = await MeterReadingHelper.AddMeterReadingAsync(geographyID, well.WellID, meter.MeterID, DateTime.UtcNow, "12:00", MeterReadingUnitType.Gallons.MeterReadingUnitTypeID, 100, 250);

        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.GetMeterReadingByID(geographyID, well.WellID, meter.MeterID, meterReading.MeterReadingID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanGetLastReadingFromDate(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var firstDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));
        var meterReadingA = await MeterReadingHelper.AddMeterReadingAsync(geographyID, well.WellID, meter.MeterID, firstDate, "12:00", MeterReadingUnitType.Gallons.MeterReadingUnitTypeID, 100, 250);

        var secondDate = DateTime.UtcNow.AddMonths(1);
        var meterReadingB = await MeterReadingHelper.AddMeterReadingAsync(geographyID, well.WellID, meter.MeterID, secondDate, "12:00", MeterReadingUnitType.Gallons.MeterReadingUnitTypeID, 250, 500);

        var dateA = DateTime.UtcNow;
        var routeA = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.GetLastReadingFromDate(geographyID, well.WellID, meter.MeterID, dateA.ToString("yyyy-MM-dd")));
        var resultA = await AssemblySteps.AdminHttpClient.GetAsync(routeA);
        var resultAAsString = await resultA.Content.ReadAsStringAsync();

        Assert.IsTrue(resultA.IsSuccessStatusCode, resultAAsString);
        Console.WriteLine(resultAAsString);

        var meterReadingADto = JsonSerializer.Deserialize<MeterReadingDto>(resultAAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(meterReadingA.MeterReadingID, meterReadingADto.MeterReadingID);

        var dateB = DateTime.UtcNow.AddMonths(1).AddDays(1);
        var routeB = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.GetLastReadingFromDate(geographyID, well.WellID, meter.MeterID, dateB.ToString("yyyy-MM-dd")));
        var resultB = await AssemblySteps.AdminHttpClient.GetAsync(routeB);
        var resultBAsString = await resultB.Content.ReadAsStringAsync();

        Assert.IsTrue(resultB.IsSuccessStatusCode, resultBAsString);
        Console.WriteLine(resultBAsString);

        var meterReadingBDto = JsonSerializer.Deserialize<MeterReadingDto>(resultBAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(meterReadingB.MeterReadingID, meterReadingBDto.MeterReadingID);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task LastReadingFromDate_BadRequest_InvalidDate(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.GetLastReadingFromDate(geographyID, well.WellID, meter.MeterID, "invalid-date"));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task LastReadingFromDate_NotThere_GetNoContent(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.GetLastReadingFromDate(geographyID, well.WellID, meter.MeterID, DateTime.UtcNow.ToString("yyyy-MM-dd")));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanUpdateMeterReading(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);
        var meterReading = await MeterReadingHelper.AddMeterReadingAsync(geographyID, well.WellID, meter.MeterID, DateTime.UtcNow, "12:00", MeterReadingUnitType.Gallons.MeterReadingUnitTypeID, 100, 250);
        var updatedCurrentReading = 300;

        var meterReadingUpsertDto = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.UtcNow,
            ReadingTime = "12:00",
            PreviousReading = 100,
            CurrentReading = updatedCurrentReading,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var json = JsonSerializer.Serialize(meterReadingUpsertDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.UpdateWellMeterReading(geographyID, well.WellID, meter.MeterID, meterReading.MeterReadingID, meterReadingUpsertDto));
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);
        var resultAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var updatedMeterReading = JsonSerializer.Deserialize<MeterReadingDto>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(updatedCurrentReading, updatedMeterReading.CurrentReading);
        Assert.AreEqual(200, updatedMeterReading.Volume);
    }


    [DataRow(5)]
    [TestMethod]
    public async Task UpdateMeterReading_BadRequest_ReadingDateBeforeWellMeterStartDate(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UtcNow);
        var meterReading = await MeterReadingHelper.AddMeterReadingAsync(geographyID, well.WellID, meter.MeterID, DateTime.UtcNow, "12:00", MeterReadingUnitType.Gallons.MeterReadingUnitTypeID, 100, 250);
        var meterReadingUpsertDto = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.UnixEpoch,
            ReadingTime = "12:00",
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var json = JsonSerializer.Serialize(meterReadingUpsertDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.UpdateWellMeterReading(geographyID, well.WellID, meter.MeterID, meterReading.MeterReadingID, meterReadingUpsertDto));
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    #region MonthlyInterpolation and MeteredExtraction Tests

    [DataRow(5, "2025-1-1", "00:00", "2025-1-31", "00:00")] // Full month.
    [DataRow(5, "2025-1-15", "12:00", "2025-1-31", "00:00")] // Start date mid-month, start time not at midnight.
    [DataRow(5, "2025-1-1", "00:00", "2025-1-20", "12:00")] // End date mid-month, end time not at midnight.
    [TestMethod]
    public async Task AdminCanCreateMultipleReadingsAndGetMonthlyInterpolations_TwoReadingsInSameMonth(int geographyID, string startDate, string startTime, string endDate, string endTime)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var meterReadingUpsertDtoA = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse(startDate),
            ReadingTime = startTime,
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonA = JsonSerializer.Serialize(meterReadingUpsertDtoA, AssemblySteps.DefaultJsonSerializerOptions);
        var contentA = new StringContent(jsonA, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, well.WellID, meter.MeterID, meterReadingUpsertDtoA));
        var resultA = await AssemblySteps.AdminHttpClient.PostAsync(route, contentA);
        var resultAsStringA = await resultA.Content.ReadAsStringAsync();

        Assert.IsTrue(resultA.IsSuccessStatusCode, resultAsStringA);
        Console.WriteLine(resultAsStringA);

        var meterReadingDtoA = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringA, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(150, meterReadingDtoA.Volume);

        var meterReadingUpsertDtoB = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse(endDate),
            ReadingTime = endTime,
            PreviousReading = 250,
            CurrentReading = 500,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonB = JsonSerializer.Serialize(meterReadingUpsertDtoB, AssemblySteps.DefaultJsonSerializerOptions);
        var contentB = new StringContent(jsonB, Encoding.UTF8, "application/json");
        var resultB = await AssemblySteps.AdminHttpClient.PostAsync(route, contentB);
        var resultAsStringB = await resultB.Content.ReadAsStringAsync();

        Assert.IsTrue(resultB.IsSuccessStatusCode, resultAsStringB);
        Console.WriteLine(resultAsStringB);

        var meterReadingDtoB = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringB, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(250, meterReadingDtoB.Volume);

        var meterReadingMonthlyInterpolations = await AssemblySteps.QanatDbContext.MeterReadingMonthlyInterpolations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WellID == well.WellID && x.MeterID == meter.MeterID)
            .ToListAsync();

        Assert.AreEqual(1, meterReadingMonthlyInterpolations.Count);

        var monthlyInterpolation = meterReadingMonthlyInterpolations[0];
        var monthlyInterpolationSimpleDto = monthlyInterpolation.AsSimpleDto();
        var json = JsonSerializer.Serialize(monthlyInterpolationSimpleDto, AssemblySteps.DefaultJsonSerializerOptions);

        Assert.AreEqual("2025-01-01", monthlyInterpolation.Date.ToString("yyyy-MM-dd"), json);

        var expectedVolume = meterReadingDtoA.Volume + meterReadingDtoB.Volume;
        Assert.AreEqual(expectedVolume, monthlyInterpolation.InterpolatedVolume, json);

        Console.WriteLine(json);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanCreateMultipleReadingsAndGetMonthlyInterpolations_TwoReadingsInSameMonth_GallonsFirstReadingAcreFeetSecondReading(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var meterReadingUpsertDtoA = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2025-01-01"),
            ReadingTime = "00:00",
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonA = JsonSerializer.Serialize(meterReadingUpsertDtoA, AssemblySteps.DefaultJsonSerializerOptions);
        var contentA = new StringContent(jsonA, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, well.WellID, meter.MeterID, meterReadingUpsertDtoA));
        var resultA = await AssemblySteps.AdminHttpClient.PostAsync(route, contentA);
        var resultAsStringA = await resultA.Content.ReadAsStringAsync();

        Assert.IsTrue(resultA.IsSuccessStatusCode, resultAsStringA);
        Console.WriteLine(resultAsStringA);

        var meterReadingDtoA = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringA, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(150, meterReadingDtoA.Volume);

        var previousReadingInAcreFeet = UnitConversionHelper.ConvertGallonsToAcreFeet(meterReadingDtoA.CurrentReading);
        var currentReadingInAcreFeet = UnitConversionHelper.ConvertGallonsToAcreFeet(500);

        var meterReadingUpsertDtoB = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.AcreFeet.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2025-01-31"),
            ReadingTime = "00:00",
            PreviousReading = previousReadingInAcreFeet,
            CurrentReading = currentReadingInAcreFeet,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonB = JsonSerializer.Serialize(meterReadingUpsertDtoB, AssemblySteps.DefaultJsonSerializerOptions);
        var contentB = new StringContent(jsonB, Encoding.UTF8, "application/json");
        var resultB = await AssemblySteps.AdminHttpClient.PostAsync(route, contentB);
        var resultBAsString = await resultB.Content.ReadAsStringAsync();

        Assert.IsTrue(resultB.IsSuccessStatusCode, resultBAsString);
        Console.WriteLine(resultBAsString);

        var resultAsStringB = await resultB.Content.ReadAsStringAsync();
        var meterReadingDtoB = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringB, AssemblySteps.DefaultJsonSerializerOptions);

        var meterReadingMonthlyInterpolations = await AssemblySteps.QanatDbContext.MeterReadingMonthlyInterpolations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WellID == well.WellID && x.MeterID == meter.MeterID)
            .ToListAsync();

        Assert.AreEqual(1, meterReadingMonthlyInterpolations.Count);

        var monthlyInterpolation = meterReadingMonthlyInterpolations[0];
        var monthlyInterpolationSimpleDto = monthlyInterpolation.AsSimpleDto();
        var json = JsonSerializer.Serialize(monthlyInterpolationSimpleDto, AssemblySteps.DefaultJsonSerializerOptions);
        Console.WriteLine(json);

        var expectedVolume = meterReadingDtoA.VolumeInAcreFeet + meterReadingDtoB.VolumeInAcreFeet;
        Assert.AreEqual(expectedVolume, monthlyInterpolation.InterpolatedVolumeInAcreFeet, json);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanCreateMultipleReadingsAndGetMonthlyInterpolations_TwoReadingsInSameMonth_AcreFeetFirstReadingGallonSecondReading(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var meterReadingUpsertDtoA = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.AcreFeet.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2025-01-01"),
            ReadingTime = "00:00",
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonA = JsonSerializer.Serialize(meterReadingUpsertDtoA, AssemblySteps.DefaultJsonSerializerOptions);
        var contentA = new StringContent(jsonA, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, well.WellID, meter.MeterID, meterReadingUpsertDtoA));
        var resultA = await AssemblySteps.AdminHttpClient.PostAsync(route, contentA);
        var resultAsStringA = await resultA.Content.ReadAsStringAsync();

        Assert.IsTrue(resultA.IsSuccessStatusCode, resultAsStringA);
        Console.WriteLine(resultAsStringA);

        var meterReadingDtoA = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringA, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(150, meterReadingDtoA.Volume);

        var previousReadingInGallons = UnitConversionHelper.ConvertAcreFeetToGallons(meterReadingDtoA.CurrentReading);
        var currentReadingInGallons = UnitConversionHelper.ConvertGallonsToAcreFeet(500);

        var meterReadingUpsertDtoB = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2025-01-31"),
            ReadingTime = "00:00",
            PreviousReading = previousReadingInGallons,
            CurrentReading = currentReadingInGallons,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonB = JsonSerializer.Serialize(meterReadingUpsertDtoB, AssemblySteps.DefaultJsonSerializerOptions);
        var contentB = new StringContent(jsonB, Encoding.UTF8, "application/json");
        var resultB = await AssemblySteps.AdminHttpClient.PostAsync(route, contentB);
        var resultBAsString = await resultB.Content.ReadAsStringAsync();

        Assert.IsTrue(resultB.IsSuccessStatusCode, resultBAsString);
        Console.WriteLine(resultBAsString);

        var resultAsStringB = await resultB.Content.ReadAsStringAsync();
        var meterReadingDtoB = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringB, AssemblySteps.DefaultJsonSerializerOptions);

        var meterReadingMonthlyInterpolations = await AssemblySteps.QanatDbContext.MeterReadingMonthlyInterpolations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WellID == well.WellID && x.MeterID == meter.MeterID)
            .ToListAsync();

        Assert.AreEqual(1, meterReadingMonthlyInterpolations.Count);

        var monthlyInterpolation = meterReadingMonthlyInterpolations[0];
        var monthlyInterpolationSimpleDto = monthlyInterpolation.AsSimpleDto();
        var json = JsonSerializer.Serialize(monthlyInterpolationSimpleDto, AssemblySteps.DefaultJsonSerializerOptions);
        Console.WriteLine(json);

        var expectedVolume = meterReadingDtoA.VolumeInAcreFeet + meterReadingDtoB.VolumeInAcreFeet;
        Assert.AreEqual(expectedVolume, monthlyInterpolation.InterpolatedVolumeInAcreFeet, json);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanCreateMultipleReadingsAndGetMonthlyInterpolations_TwoReadingsAcrossTwoMonths(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var meterReadingUpsertDtoA = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2025-1-1"),
            ReadingTime = "12:00",
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonA = JsonSerializer.Serialize(meterReadingUpsertDtoA, AssemblySteps.DefaultJsonSerializerOptions);
        var contentA = new StringContent(jsonA, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, well.WellID, meter.MeterID, meterReadingUpsertDtoA));
        var resultA = await AssemblySteps.AdminHttpClient.PostAsync(route, contentA);
        var resultAsStringA = await resultA.Content.ReadAsStringAsync();

        Assert.IsTrue(resultA.IsSuccessStatusCode, resultAsStringA);

        var meterReadingDtoA = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringA, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(150, meterReadingDtoA.Volume);

        var meterReadingUpsertDtoB = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2025-2-15"),
            ReadingTime = "12:00",
            PreviousReading = 250,
            CurrentReading = 500,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonB = JsonSerializer.Serialize(meterReadingUpsertDtoB, AssemblySteps.DefaultJsonSerializerOptions);
        var contentB = new StringContent(jsonB, Encoding.UTF8, "application/json");
        var resultB = await AssemblySteps.AdminHttpClient.PostAsync(route, contentB);
        var resultAsStringB = await resultB.Content.ReadAsStringAsync();

        Assert.IsTrue(resultB.IsSuccessStatusCode, resultAsStringB);

        var meterReadingDtoB = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringB, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(250, meterReadingDtoB.Volume);

        var meterReadingMonthlyInterpolations = await AssemblySteps.QanatDbContext.MeterReadingMonthlyInterpolations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WellID == well.WellID && x.MeterID == meter.MeterID)
            .ToListAsync();

        Assert.AreEqual(2, meterReadingMonthlyInterpolations.Count);

        var firstMonthlyInterpolation = meterReadingMonthlyInterpolations[0];
        var firstMonthlyInterpolationAsSimpleDto = firstMonthlyInterpolation.AsSimpleDto();
        var firstMonthlyInterpolationJSON = JsonSerializer.Serialize(firstMonthlyInterpolationAsSimpleDto, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual("2025-01-01", firstMonthlyInterpolation.Date.ToString("yyyy-MM-dd"));
        Console.WriteLine(firstMonthlyInterpolationJSON);

        var secondMonthlyInterpolation = meterReadingMonthlyInterpolations[1];
        var secondMonthlyInterpolationAsSimpleDto = secondMonthlyInterpolation.AsSimpleDto();
        var secondMonthlyInterpolationJSON = JsonSerializer.Serialize(secondMonthlyInterpolationAsSimpleDto, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual("2025-02-01", secondMonthlyInterpolation.Date.ToString("yyyy-MM-dd"));
        Console.WriteLine(secondMonthlyInterpolationJSON);

        var expectedTotalVolume = meterReadingDtoA.Volume + meterReadingDtoB.Volume;
        Assert.AreEqual(expectedTotalVolume, firstMonthlyInterpolation.InterpolatedVolume + secondMonthlyInterpolation.InterpolatedVolume);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanCreateMultipleReadingsAndGetMonthlyInterpolations_ThreeReadingsAcrossMultipleMonths(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeter = await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var meterReadingUpsertDtoA = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2025-1-2"),
            ReadingTime = "10:45",
            PreviousReading = 100,
            CurrentReading = 250,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonA = JsonSerializer.Serialize(meterReadingUpsertDtoA, AssemblySteps.DefaultJsonSerializerOptions);
        var contentA = new StringContent(jsonA, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, well.WellID, meter.MeterID, meterReadingUpsertDtoA));
        var resultA = await AssemblySteps.AdminHttpClient.PostAsync(route, contentA);
        var resultAsStringA = await resultA.Content.ReadAsStringAsync();

        Assert.IsTrue(resultA.IsSuccessStatusCode, resultAsStringA);

        var meterReadingDtoA = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringA, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(150, meterReadingDtoA.Volume);

        var meterReadingUpsertDtoB = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2025-2-15"),
            ReadingTime = "12:00",
            PreviousReading = 250,
            CurrentReading = 500,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonB = JsonSerializer.Serialize(meterReadingUpsertDtoB, AssemblySteps.DefaultJsonSerializerOptions);
        var contentB = new StringContent(jsonB, Encoding.UTF8, "application/json");
        var resultB = await AssemblySteps.AdminHttpClient.PostAsync(route, contentB);
        var resultAsStringB = await resultB.Content.ReadAsStringAsync();

        Assert.IsTrue(resultB.IsSuccessStatusCode, resultAsStringB);

        var meterReadingDtoB = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringB, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(250, meterReadingDtoB.Volume);

        var meterReadingUpsertDtoC = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2025-4-15"),
            ReadingTime = "12:00",
            PreviousReading = 500,
            CurrentReading = 750,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        var jsonC = JsonSerializer.Serialize(meterReadingUpsertDtoC, AssemblySteps.DefaultJsonSerializerOptions);
        var contentC = new StringContent(jsonC, Encoding.UTF8, "application/json");
        var resultC = await AssemblySteps.AdminHttpClient.PostAsync(route, contentC);
        var resultAsStringC = await resultC.Content.ReadAsStringAsync();

        Assert.IsTrue(resultC.IsSuccessStatusCode, resultAsStringC);

        var meterReadingDtoC = JsonSerializer.Deserialize<MeterReadingDto>(resultAsStringC, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(250, meterReadingDtoC.Volume);

        var meterReadingMonthlyInterpolations = await AssemblySteps.QanatDbContext.MeterReadingMonthlyInterpolations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WellID == well.WellID && x.MeterID == meter.MeterID)
            .ToListAsync();

        Assert.AreEqual(4, meterReadingMonthlyInterpolations.Count);

        var firstMonthlyInterpolation = meterReadingMonthlyInterpolations[0];
        var firstMonthlyInterpolationAsSimpleDto = firstMonthlyInterpolation.AsSimpleDto();
        var firstMonthlyInterpolationJSON = JsonSerializer.Serialize(firstMonthlyInterpolationAsSimpleDto, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual("2025-01-01", firstMonthlyInterpolation.Date.ToString("yyyy-MM-dd"));
        Console.WriteLine(firstMonthlyInterpolationJSON);

        var secondMonthlyInterpolation = meterReadingMonthlyInterpolations[1];
        var secondMonthlyInterpolationAsSimpleDto = secondMonthlyInterpolation.AsSimpleDto();
        var secondMonthlyInterpolationJSON = JsonSerializer.Serialize(secondMonthlyInterpolationAsSimpleDto, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual("2025-02-01", secondMonthlyInterpolation.Date.ToString("yyyy-MM-dd"));
        Console.WriteLine(secondMonthlyInterpolationJSON);

        var thirdMonthlyInterpolation = meterReadingMonthlyInterpolations[2];
        var thirdMonthlyInterpolationAsSimpleDto = thirdMonthlyInterpolation.AsSimpleDto();
        var thirdMonthlyInterpolationJSON = JsonSerializer.Serialize(thirdMonthlyInterpolationAsSimpleDto, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual("2025-03-01", thirdMonthlyInterpolation.Date.ToString("yyyy-MM-dd"));
        Console.WriteLine(thirdMonthlyInterpolationJSON);

        var fourthMonthlyInterpolation = meterReadingMonthlyInterpolations[3];
        var fourthMonthlyInterpolationAsSimpleDto = fourthMonthlyInterpolation.AsSimpleDto();
        var fourthMonthlyInterpolationJSON = JsonSerializer.Serialize(fourthMonthlyInterpolationAsSimpleDto, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual("2025-04-01", fourthMonthlyInterpolation.Date.ToString("yyyy-MM-dd"));
        Console.WriteLine(fourthMonthlyInterpolationJSON);

        var expectedTotalVolume = meterReadingDtoA.Volume + meterReadingDtoB.Volume + meterReadingDtoC.Volume;
        var actualTotal = firstMonthlyInterpolation.InterpolatedVolume + secondMonthlyInterpolation.InterpolatedVolume + thirdMonthlyInterpolation.InterpolatedVolume + fourthMonthlyInterpolation.InterpolatedVolume;
        Assert.AreEqual(expectedTotalVolume, actualTotal, .0001m);
    }

    [DataRow(5, 54)] //Demo, Metered Extraction
    [TestMethod]
    public async Task AdminCanCreateMeterReading_MeteredExtractionWaterMeasurementsProcessed_MultipleWellsIrrigatingParcel(int geographyID, int waterMeasurementTypeID)
    {
        var reportingPeriod = await AssemblySteps.QanatDbContext.ReportingPeriods.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.IsDefault)
            .FirstOrDefaultAsync();

        var parcelA = await ParcelHelper.AddParcelAsync(geographyID, 1500);
        var usageLocationA = await UsageLocationHelper.AddUsageLocationAsync(geographyID, parcelA.ParcelID, reportingPeriod.ReportingPeriodID, 1000);
        var usageLocationB = await UsageLocationHelper.AddUsageLocationAsync(geographyID, parcelA.ParcelID, reportingPeriod.ReportingPeriodID, 500);

        var wellA = await WellHelper.AddWellAsync(geographyID);
        var meterA = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeterA = await MeterHelper.AssociateMeterToWellAsync(wellA.WellID, meterA.MeterID, DateTime.UnixEpoch);
        var wellIrrigatedParcelA = await WellHelper.AddWellIrrigatedParcel(wellA.WellID, parcelA.ParcelID);

        var wellB = await WellHelper.AddWellAsync(geographyID);
        var meterB = await MeterHelper.AddMeterAsync(geographyID);
        var wellMeterB = await MeterHelper.AssociateMeterToWellAsync(wellB.WellID, meterB.MeterID, DateTime.UnixEpoch);
        var wellIrrigatedParcelB = await WellHelper.AddWellIrrigatedParcel(wellB.WellID, parcelA.ParcelID);

        var routeA = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, wellA.WellID, meterA.MeterID, null));
        var meterAReadingA = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.AcreFeet.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2024-1-1"),
            ReadingTime = "12:00",
            PreviousReading = 0,
            CurrentReading = 1000,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        await AssemblySteps.AdminHttpClient.PostAsJsonAsync(routeA, meterAReadingA, AssemblySteps.DefaultJsonSerializerOptions);

        var meterAReadingB = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.AcreFeet.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2024-2-1"),
            ReadingTime = "12:00",
            PreviousReading = 1000,
            CurrentReading = 2000,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        await AssemblySteps.AdminHttpClient.PostAsJsonAsync(routeA, meterAReadingB, AssemblySteps.DefaultJsonSerializerOptions);

        var extractedWaterMeasurementsA = await AssemblySteps.QanatDbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.UsageLocationID == usageLocationA.UsageLocationID)
            .ToListAsync();
        Assert.AreEqual(1, extractedWaterMeasurementsA.Count);
        var extractedWaterMeasurementA = extractedWaterMeasurementsA[0];


        var extractedWaterMeasurementsB = await AssemblySteps.QanatDbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.UsageLocationID == usageLocationB.UsageLocationID)
            .ToListAsync();
        Assert.AreEqual(1, extractedWaterMeasurementsB.Count);
        var extractedWaterMeasurementB = extractedWaterMeasurementsB[0];

        var totalAcreFeet = extractedWaterMeasurementA.ReportedValueInAcreFeet + extractedWaterMeasurementB.ReportedValueInAcreFeet;
        Assert.AreEqual(2000m, totalAcreFeet);
        Assert.AreEqual(1333.33m, extractedWaterMeasurementA.ReportedValueInAcreFeet, 0.01m);
        Assert.AreEqual(666.67m, extractedWaterMeasurementB.ReportedValueInAcreFeet, 0.01m);

        var routeB = RouteHelper.GetRouteFor<MeterReadingByMeterController>(c => c.CreateMeterReading(geographyID, wellB.WellID, meterB.MeterID, null));
        var meterBReadingA = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.AcreFeet.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2024-1-1"),
            ReadingTime = "12:00",
            PreviousReading = 0,
            CurrentReading = 1000,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        await AssemblySteps.AdminHttpClient.PostAsJsonAsync(routeB, meterBReadingA, AssemblySteps.DefaultJsonSerializerOptions);

        var meterBReadingB = new MeterReadingUpsertDto()
        {
            MeterReadingUnitTypeID = MeterReadingUnitType.AcreFeet.MeterReadingUnitTypeID,
            ReadingDate = DateTime.Parse("2024-2-1"),
            ReadingTime = "12:00",
            PreviousReading = 1000,
            CurrentReading = 2000,
            ReaderInitials = "AB",
            Comment = "Test Comment"
        };

        await AssemblySteps.AdminHttpClient.PostAsJsonAsync(routeB, meterBReadingB, AssemblySteps.DefaultJsonSerializerOptions);

        var extractedWaterMeasurementsC = await AssemblySteps.QanatDbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.UsageLocationID == usageLocationA.UsageLocationID)
            .ToListAsync();
        Assert.AreEqual(1, extractedWaterMeasurementsC.Count);
        var extractedWaterMeasurementC = extractedWaterMeasurementsC[0];

        var extractedWaterMeasurementsD = await AssemblySteps.QanatDbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.UsageLocationID == usageLocationB.UsageLocationID)
            .ToListAsync();
        Assert.AreEqual(1, extractedWaterMeasurementsD.Count);
        var extractedWaterMeasurementD = extractedWaterMeasurementsD[0];

        var totalAcreFeetB = extractedWaterMeasurementC.ReportedValueInAcreFeet + extractedWaterMeasurementD.ReportedValueInAcreFeet;
        Assert.AreEqual(4000m, totalAcreFeetB);
        Assert.AreEqual(2666.67m, extractedWaterMeasurementC.ReportedValueInAcreFeet, 0.01m);
        Assert.AreEqual(1333.33m, extractedWaterMeasurementD.ReportedValueInAcreFeet, 0.01m);
    }

    #endregion
}