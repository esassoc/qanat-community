using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers.UsageLocation;

[TestClass]
public class UsageLocationControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanList(int geographyID)
    {
        var parcel = await AssemblySteps.QanatDbContext.Parcels.AsNoTracking()
            .Include(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocations.Any());

        var route = RouteHelper.GetRouteFor<UsageLocationController>(x => x.List(geographyID, parcel.ParcelID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultContentAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var usageLocationDtos = await result.DeserializeContentAsync<List<UsageLocationDto>>();

        //Check that we see all usage locations for the parcel
        Assert.AreEqual(parcel.UsageLocations.Count, usageLocationDtos.Count);
        foreach (var usageLocation in parcel.UsageLocations)
        {
            var usageLocationDto = usageLocationDtos.First(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNotNull(usageLocationDto);
        }
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanGet(int geographyID)
    {
        var parcel = await AssemblySteps.QanatDbContext.Parcels.AsNoTracking()
            .Include(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocations.Any());

        var usageLocation = parcel.UsageLocations.First();

        var route = RouteHelper.GetRouteFor<UsageLocationController>(x => x.Get(geographyID, parcel.ParcelID, usageLocation.UsageLocationID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultContentAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var usageLocationDto = await result.DeserializeContentAsync<UsageLocationDto>();
        Assert.IsNotNull(usageLocationDto);

        //Check that we see the correct usage location
        Assert.AreEqual(usageLocation.UsageLocationID, usageLocationDto.UsageLocationID);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanMigrateUsageLocations(int geographyID)
    {
        var toParcel = await AssemblySteps.QanatDbContext.Parcels.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID);

        var usageLocationsToMigrate = AssemblySteps.QanatDbContext.UsageLocations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ParcelID != toParcel.ParcelID)
            .Take(5)
            .ToList();
        var usageLocationIDsToMigrate = usageLocationsToMigrate.Select(x => x.UsageLocationID).ToList();

        //Save usage location to parcel mapping so we can revert to make test re-runnable.
        var usageLocationParcelMapping = usageLocationsToMigrate
            .ToDictionary(x => x.UsageLocationID, x => x.ParcelID);

        Assert.IsTrue(usageLocationIDsToMigrate.Any(), "No usage locations to migrate found.");

        var migrationDto = new UsageLocationMigrationDto
        {
            UsageLocationIDs = usageLocationIDsToMigrate
        };

        var route = RouteHelper.GetRouteFor<UsageLocationController>(x => x.MigrateUsageLocations(geographyID, toParcel.ParcelID, null));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, migrationDto, AssemblySteps.DefaultJsonSerializerOptions);
        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");

        Console.WriteLine(resultContentAsString);

        var usageLocationDtos = await result.DeserializeContentAsync<List<UsageLocationDto>>();
        Assert.IsNotNull(usageLocationDtos);
        Assert.IsTrue(usageLocationDtos.Any(), "No usage locations returned after migration.");
        foreach (var usageLocationDto in usageLocationDtos)
        {
            Assert.AreEqual(toParcel.ParcelID, usageLocationDto.Parcel.ParcelID, $"Usage location {usageLocationDto.UsageLocationID} not migrated to correct parcel.");
        }

        // Revert the migration by restoring the original parcel mapping
        foreach (var usageLocation in usageLocationsToMigrate)
        {
            usageLocation.ParcelID = usageLocationParcelMapping[usageLocation.UsageLocationID];
        }

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        //Remove the UsageLocationParcelHistories created.
        var usageLocationParcelHistories = await AssemblySteps.QanatDbContext.UsageLocationParcelHistories
            .Where(x => usageLocationIDsToMigrate.Contains(x.UsageLocationID) && x.ToParcelID == toParcel.ParcelID)
            .ToListAsync();
        AssemblySteps.QanatDbContext.UsageLocationParcelHistories.RemoveRange(usageLocationParcelHistories);
        await AssemblySteps.QanatDbContext.SaveChangesAsync();
    }

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanMigrateUsageLocations_BadRequest_InvalidUsageLocationID(int geographyID)
    {
        var toParcel = await AssemblySteps.QanatDbContext.Parcels.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID);

        var usageLocationsToMigrate = AssemblySteps.QanatDbContext.UsageLocations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ParcelID != toParcel.ParcelID)
            .Take(5)
            .ToList();
        var usageLocationIDsToMigrate = usageLocationsToMigrate.Select(x => x.UsageLocationID).ToList();

        // Add an invalid usage location ID to the list
        usageLocationIDsToMigrate.Add(-1);

        Assert.IsTrue(usageLocationIDsToMigrate.Any(), "No usage locations to migrate found.");

        var migrationDto = new UsageLocationMigrationDto
        {
            UsageLocationIDs = usageLocationIDsToMigrate
        };

        var route = RouteHelper.GetRouteFor<UsageLocationController>(x => x.MigrateUsageLocations(geographyID, toParcel.ParcelID, null));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, migrationDto, AssemblySteps.DefaultJsonSerializerOptions);
        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);
    }
}