using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Common.Util;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using Qanat.Tests.Helpers.EntityHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers.Meter;

[TestClass]
public class MeterReadingByGeographyControllerTests
{
    [DataRow(5, 100)]
    [TestMethod]
    public async Task AdminCanListByGeographyID(int geographyID, int readingCount)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var meterReadingsToAdd = new List<MeterReading>();
        for (var i = 0; i < readingCount; i++)
        {
            var readingDate = DateTime.UtcNow.AddDays(-i);

            var meterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID;
            var previousReading = 1000 + i;
            var currentReading = previousReading + 10;
            var volume = currentReading - previousReading;
            var volumeInAcreFeet = UnitConversionHelper.ConvertGallonsToAcreFeet(volume);
            var meterReading = new MeterReading()
            {
                GeographyID = geographyID,
                WellID = well.WellID,
                MeterID = meter.MeterID,
                MeterReadingUnitTypeID = meterReadingUnitTypeID,

                ReadingDate = readingDate,
                PreviousReading = previousReading,
                CurrentReading = currentReading,

                Volume = volume,
                VolumeInAcreFeet = volumeInAcreFeet,
                ReaderInitials = "MK",
                Comment = "Test comment"
            };

            meterReadingsToAdd.Add(meterReading);
        }

        await AssemblySteps.QanatDbContext.MeterReadings.AddRangeAsync(meterReadingsToAdd);
        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var route = RouteHelper.GetRouteFor<MeterReadingByGeographyController>(x => x.ListByGeographyID(geographyID));
        var response = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await response.Content.ReadAsStringAsync();

        Assert.IsTrue(response.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var meterReadingGridDtos = JsonSerializer.Deserialize<List<MeterReadingGridDto>>(resultAsString);
        Assert.IsNotNull(meterReadingGridDtos);

        var meterReadings = await AssemblySteps.QanatDbContext.MeterReadings.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        Assert.AreEqual(meterReadings.Count, meterReadingGridDtos.Count);
    }
}