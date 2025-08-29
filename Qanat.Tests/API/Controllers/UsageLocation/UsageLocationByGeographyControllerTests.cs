using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Qanat.EFModels.Entities.WaterMeasurementCalculations;

namespace Qanat.Tests.API.Controllers.UsageLocation;

[TestClass]
public class UsageLocationByGeographyControllerTests
{
    [DataRow(7)]
    [TestMethod]
    public async Task AdminCanListByGeographyAndReportingPeriod(int geographyID)
    {
        var reportingPeriodWithUsageLocation = await AssemblySteps.QanatDbContext.ReportingPeriods.AsNoTracking()
            .Include(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocations.Any());

        var route = RouteHelper.GetRouteFor<UsageLocationByGeographyController>(x => x.ListByGeographyAndReportingPeriod(geographyID, reportingPeriodWithUsageLocation.ReportingPeriodID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultContentAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var usageLocationIndexGridDtos = await result.DeserializeContentAsync<List<UsageLocationByReportingPeriodIndexGridDto>>();
        Assert.IsNotNull(usageLocationIndexGridDtos);

        //Check that we see all usage locations for the reporting period
        Assert.AreEqual(reportingPeriodWithUsageLocation.UsageLocations.Count, usageLocationIndexGridDtos.Count);
        foreach (var usageLocation in usageLocationIndexGridDtos)
        {
            Assert.AreEqual(reportingPeriodWithUsageLocation.ReportingPeriodID, usageLocation.ReportingPeriodID);
            Assert.IsTrue(reportingPeriodWithUsageLocation.UsageLocations.Any(x => x.UsageLocationID == usageLocation.UsageLocationID));
        }
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanReplaceFromParcels(int geographyID)
    {
        //Add a new reporting period to the geography so we don't wipe WaterMeasurements
        var reportingPeriodUpsertDto = new ReportingPeriodUpsertDto()
        {
            Name = "1970",
            StartDate = new DateTime(1970, 1, 1),
            IsDefault = false,
            ReadyForAccountHolders = false
        };

        var callingUser = Users.GetByUserID(AssemblySteps.QanatDbContext, Users.QanatSystemAdminUserID);
        var newReportingPeriod = await ReportingPeriods.CreateAsync(AssemblySteps.QanatDbContext, geographyID, reportingPeriodUpsertDto, callingUser);
        Assert.IsNotNull(newReportingPeriod);

        //Add Usage Location to test removal
        var parcelToAddUsageLocationTo = await AssemblySteps.QanatDbContext.Parcels.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID);

        var defaultUsageLocationType = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.IsDefault);

        var usageLocation = new EFModels.Entities.UsageLocation()
        {
            GeographyID = geographyID,
            ParcelID = parcelToAddUsageLocationTo.ParcelID,
            ReportingPeriodID = newReportingPeriod.ReportingPeriodID,
            UsageLocationTypeID = defaultUsageLocationType.UsageLocationTypeID,
            Name = Guid.NewGuid().ToString(),
            Area = 100,
            CreateDate = DateTime.UtcNow,
            CreateUserID = Users.QanatSystemAdminUserID
        };

        await AssemblySteps.QanatDbContext.UsageLocations.AddAsync(usageLocation);
        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        //Post to the replace endpoint
        var route = RouteHelper.GetRouteFor<UsageLocationByGeographyController>(x => x.ReplaceFromParcels(geographyID, newReportingPeriod.ReportingPeriodID));
        var content = new StringContent("", Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);
        var resultContentAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var usageLocationIndexGridDtos = await result.DeserializeContentAsync<List<UsageLocationByReportingPeriodIndexGridDto>>();

        var parcels = await AssemblySteps.QanatDbContext.Parcels.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        //Check that we see all usage locations for the reporting period
        Assert.AreEqual(parcels.Count, usageLocationIndexGridDtos.Count);
        foreach (var parcel in parcels)
        {
            Assert.IsTrue(usageLocationIndexGridDtos.Any(x => x.ParcelID == parcel.ParcelID));
        }

        //Clean up usage locations and the added reporting period
        var usageLocationRemovedCount = await AssemblySteps.QanatDbContext.UsageLocations
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == newReportingPeriod.ReportingPeriodID)
            .ExecuteDeleteAsync();

        Assert.AreEqual(parcels.Count, usageLocationRemovedCount);

        var reportingPeriodRemovedCount = await AssemblySteps.QanatDbContext.ReportingPeriods
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == newReportingPeriod.ReportingPeriodID)
            .ExecuteDeleteAsync();

        Assert.AreEqual(1, reportingPeriodRemovedCount);
    }

    [DataRow(5, 5)]
    [TestMethod]
    public async Task AdminCanBulkUpdateUsageLocationType(int geographyID, int usageLocationCountToUpdate)
    {
        var reportingPeriod = await AssemblySteps.QanatDbContext.ReportingPeriods.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocations.Any());

        var usageLocations = await AssemblySteps.QanatDbContext.UsageLocations
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            .Take(usageLocationCountToUpdate)
            .ToListAsync();

        var usageLocationTypes = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        //Set usage locations to default usage location type to prime test.
        var defaultUsageLocationType = usageLocationTypes.FirstOrDefault(x => x.IsDefault);
        Assert.IsNotNull(defaultUsageLocationType);

        usageLocations.ForEach(x => x.UsageLocationTypeID = defaultUsageLocationType.UsageLocationTypeID);

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var usageLocationTypeToUpdateTo = usageLocationTypes.FirstOrDefault(x => !x.IsDefault);
        Assert.IsNotNull(usageLocationTypeToUpdateTo);

        var note = Guid.NewGuid().ToString();
        var updateDto = new UsageLocationBulkUpdateUsageLocationTypeDto()
        {
            UsageLocationTypeID = usageLocationTypeToUpdateTo.UsageLocationTypeID,
            UsageLocationIDs = usageLocations.Select(x => x.UsageLocationID).ToList(),
            Note = note
        };

        var route = RouteHelper.GetRouteFor<UsageLocationByGeographyController>(x => x.BulkUpdateUsageLocationType(geographyID, reportingPeriod.ReportingPeriodID, updateDto));
        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var updatedUsageLocations = await result.DeserializeContentAsync<List<UsageLocationDto>>();
        Assert.IsNotNull(updatedUsageLocations);
        Assert.AreEqual(usageLocationCountToUpdate, updatedUsageLocations.Count);

        //Check that all usage locations were updated
        foreach (var usageLocation in updatedUsageLocations)
        {
            Assert.AreEqual(usageLocationTypeToUpdateTo.UsageLocationTypeID, usageLocation.UsageLocationType.UsageLocationTypeID);
        }

        var latestHistories = AssemblySteps.QanatDbContext.UsageLocationHistories
            .Where(x => usageLocations.Select(y => y.UsageLocationID).Contains(x.UsageLocationID))
            .GroupBy(x => x.UsageLocationID)
            .Select(g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault())
            .ToList();

        Assert.IsTrue(latestHistories.Any());

        //Check that the latest history has the previous location type (which should be default) and note
        foreach (var history in latestHistories)
        {
            Assert.AreEqual(usageLocationTypeToUpdateTo.UsageLocationTypeID, history.UsageLocationTypeID);
            Assert.AreEqual(note, history.Note);
        }
    }

    [DataRow(7, "1/31/2025", 5, 21, 34, 75, 33, .0001)]
    [TestMethod]
    public async Task AdminCanBulkUpdateUsageLocationType_CheckWaterMeasurementRecalculation(int geographyID, string dateToCheckAsString, int usageLocationCountToUpdate, int landIQETWaterMeasurementTypeID, int effectivePrecipWaterMeasurementTypeID, int coverCropAdjustmentWaterMeasurementTypeID, int consumedGroundwaterWaterMeasurementTypeID, double tolerance)
    {
        var dateToCheck = DateTime.Parse(dateToCheckAsString);
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, dateToCheck.Year);

        var usageLocationToUpdateFrom = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Include(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID  && !x.WaterMeasurementTypeID.HasValue);
        Assert.IsNotNull(usageLocationToUpdateFrom);

        var usageLocationTypeToUpdateTo = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID.HasValue);
        Assert.IsNotNull(usageLocationTypeToUpdateTo);

        var usageLocations = await AssemblySteps.QanatDbContext.UsageLocations
            .Include(x => x.UsageLocationType)
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            .Take(usageLocationCountToUpdate)
            .ToListAsync();

        usageLocations.ForEach(x => x.UsageLocationTypeID = usageLocationToUpdateFrom.UsageLocationTypeID);

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var note = Guid.NewGuid().ToString();
        var updateDto = new UsageLocationBulkUpdateUsageLocationTypeDto()
        {
            UsageLocationTypeID = usageLocationTypeToUpdateTo.UsageLocationTypeID,
            UsageLocationIDs = usageLocations.Select(x => x.UsageLocationID).ToList(),
            Note = note
        };

        var route = RouteHelper.GetRouteFor<UsageLocationByGeographyController>(x => x.BulkUpdateUsageLocationType(geographyID, reportingPeriod.ReportingPeriodID, updateDto));
        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var updatedUsageLocations = await result.DeserializeContentAsync<List<UsageLocationDto>>();
        Assert.IsNotNull(updatedUsageLocations);
        Assert.AreEqual(usageLocationCountToUpdate, updatedUsageLocations.Count);
        
        var landIQETWaterMeasurements = await AssemblySteps.QanatDbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCheck.Date && x.WaterMeasurementTypeID == landIQETWaterMeasurementTypeID && usageLocations.Select(ul => ul.UsageLocationID).Contains(x.UsageLocationID))
            .ToListAsync();

        var effectivePrecipWaterMeasurements = await AssemblySteps.QanatDbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCheck.Date && x.WaterMeasurementTypeID == effectivePrecipWaterMeasurementTypeID && usageLocations.Select(ul => ul.UsageLocationID).Contains( x.UsageLocationID))
            .ToListAsync();

        var coverCroppedAdjustmentWaterMeasurements = await AssemblySteps.QanatDbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCheck.Date && x.WaterMeasurementTypeID == coverCropAdjustmentWaterMeasurementTypeID && usageLocations.Select(ul => ul.UsageLocationID).Contains(x.UsageLocationID))
            .ToListAsync();

        var consumedGroundwaterWaterMeasurements = await AssemblySteps.QanatDbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCheck.Date && x.WaterMeasurementTypeID == consumedGroundwaterWaterMeasurementTypeID && usageLocations.Select(ul => ul.UsageLocationID).Contains(x.UsageLocationID))
            .ToListAsync();

        foreach (var usageLocation in usageLocations)
        {
            var landIQET = landIQETWaterMeasurements.SingleOrDefault(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNotNull(landIQET, $"Land IQ ET Water Measurement not found for Usage Location ID: {usageLocation.UsageLocationID}");

            var effectivePrecip = effectivePrecipWaterMeasurements.SingleOrDefault(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNotNull(effectivePrecip, $"Effective Precipitation Water Measurement not found for Usage Location ID: {usageLocation.UsageLocationID}");

            var coverCroppedAdjustment = coverCroppedAdjustmentWaterMeasurements.SingleOrDefault(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNotNull(coverCroppedAdjustment, $"Cover Cropped Adjustment Water Measurement not found for Usage Location ID: {usageLocation.UsageLocationID}");

            var consumedGroundwater = consumedGroundwaterWaterMeasurements.SingleOrDefault(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNotNull(consumedGroundwater, $"Consumed Groundwater Water Measurement not found for Usage Location ID: {usageLocation.UsageLocationID}");

            var expectedConsumedGroundwater = landIQET.ReportedValueInAcreFeet - effectivePrecip.ReportedValueInAcreFeet - coverCroppedAdjustment.ReportedValueInAcreFeet;
            var valueToCheck = Math.Abs(consumedGroundwater.ReportedValueInAcreFeet - expectedConsumedGroundwater);
            Assert.IsTrue(valueToCheck <= (decimal)tolerance, $"Consumed Groundwater Water Measurement for Usage Location ID: {usageLocation.UsageLocationID} is not within tolerance. Expected: {expectedConsumedGroundwater}, Actual: {consumedGroundwater.ReportedValueInFeet}");
        }

        var updateBackDto = new UsageLocationBulkUpdateUsageLocationTypeDto()
        {
            UsageLocationTypeID = usageLocationToUpdateFrom.UsageLocationTypeID,
            UsageLocationIDs = updatedUsageLocations.Select(x => x.UsageLocationID).ToList(),
            Note = note
        };

        var updateBackJson = JsonSerializer.Serialize(updateBackDto);
        var updateBackContent = new StringContent(updateBackJson, Encoding.UTF8, "application/json");
        var updateBackResult = await AssemblySteps.AdminHttpClient.PutAsync(route, updateBackContent);

        var updatedBackCoverCroppedAdjustmentWaterMeasurements = await AssemblySteps.QanatDbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.ReportedDate == dateToCheck && x.WaterMeasurementTypeID == coverCropAdjustmentWaterMeasurementTypeID && usageLocations.Select(ul => ul.UsageLocationID).Contains(x.UsageLocationID))
            .ToListAsync();

        var updatedBackConsumedGroundwaterWaterMeasurements = await AssemblySteps.QanatDbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.ReportedDate == dateToCheck && x.WaterMeasurementTypeID == consumedGroundwaterWaterMeasurementTypeID && usageLocations.Select(ul => ul.UsageLocationID).Contains(x.UsageLocationID))
            .ToListAsync();

        foreach (var usageLocation in updatedUsageLocations)
        {
            var landIQET = landIQETWaterMeasurements.SingleOrDefault(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNotNull(landIQET, $"Land IQ ET Water Measurement not found for Usage Location ID: {usageLocation.UsageLocationID}");

            var effectivePrecip = effectivePrecipWaterMeasurements.SingleOrDefault(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNotNull(effectivePrecip, $"Effective Precipitation Water Measurement not found for Usage Location ID: {usageLocation.UsageLocationID}");

            var coverCroppedAdjustment = updatedBackCoverCroppedAdjustmentWaterMeasurements.SingleOrDefault(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNull(coverCroppedAdjustment, $"Cover Cropped Adjustment Water Measurement found for Usage Location ID: {usageLocation.UsageLocationID}");

            var consumedGroundwater = updatedBackConsumedGroundwaterWaterMeasurements.SingleOrDefault(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNotNull(consumedGroundwater, $"Consumed Groundwater Water Measurement not found for Usage Location ID: {usageLocation.UsageLocationID}");

            var expectedConsumedGroundwater = landIQET.ReportedValueInAcreFeet - effectivePrecip.ReportedValueInAcreFeet;
            var valueToCheck = Math.Abs(consumedGroundwater.ReportedValueInAcreFeet - expectedConsumedGroundwater);
            Assert.IsTrue(valueToCheck <= (decimal)tolerance, $"Consumed Groundwater Water Measurement for Usage Location ID: {usageLocation.UsageLocationID} is not within tolerance. Expected: {expectedConsumedGroundwater}, Actual: {consumedGroundwater.ReportedValueInFeet}");
        }
    }

    [DataRow(5, 5, "No valid Usage Location Type found with the ID: -1.")]
    [TestMethod]
    public async Task AdminCanBulkUpdateUsageLocationType_BadRequest_InvalidUsageLocationTypeID(int geographyID, int usageLocationCountToUpdate, string expectedStringInResult)
    {
        var reportingPeriod = await AssemblySteps.QanatDbContext.ReportingPeriods.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocations.Any());

        var usageLocations = await AssemblySteps.QanatDbContext.UsageLocations
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            .Take(usageLocationCountToUpdate)
            .ToListAsync();

        var usageLocationTypes = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        //Set usage locations to default usage location type to prime test.
        var defaultUsageLocationType = usageLocationTypes.FirstOrDefault(x => x.IsDefault);
        Assert.IsNotNull(defaultUsageLocationType);

        usageLocations.ForEach(x => x.UsageLocationTypeID = defaultUsageLocationType.UsageLocationTypeID);

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var usageLocationToUpdateTo = usageLocationTypes.FirstOrDefault(x => !x.IsDefault);
        Assert.IsNotNull(usageLocationToUpdateTo);

        var note = Guid.NewGuid().ToString();
        var updateDto = new UsageLocationBulkUpdateUsageLocationTypeDto()
        {
            UsageLocationTypeID = -1,
            UsageLocationIDs = usageLocations.Select(x => x.UsageLocationID).ToList(),
            Note = note
        };

        var route = RouteHelper.GetRouteFor<UsageLocationByGeographyController>(x => x.BulkUpdateUsageLocationType(geographyID, reportingPeriod.ReportingPeriodID, updateDto));
        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Console.WriteLine(resultContentAsString);
        Assert.IsTrue(resultContentAsString.Contains(expectedStringInResult));
    }
}