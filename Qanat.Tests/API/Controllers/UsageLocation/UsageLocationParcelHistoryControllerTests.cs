using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers.UsageLocation;

[TestClass]
public class UsageLocationParcelHistoryControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanList(int geographyID)
    {
        var toParcel = await AssemblySteps.QanatDbContext.Parcels.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID);

        var usageLocationsToMigrate = AssemblySteps.QanatDbContext.UsageLocations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ParcelID != toParcel.ParcelID)
            .Take(1)
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

        var migrateRoute = RouteHelper.GetRouteFor<UsageLocationController>(x => x.MigrateUsageLocations(geographyID, toParcel.ParcelID, null));
        var migrateResult = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(migrateRoute, migrationDto, AssemblySteps.DefaultJsonSerializerOptions);
        var migrateResultContentAsString = await migrateResult.Content.ReadAsStringAsync();
        Assert.IsTrue(migrateResult.IsSuccessStatusCode, $"\nRoute: {migrateRoute}\n{migrateResultContentAsString}");

        var fromParcelID = usageLocationsToMigrate.First().ParcelID;
        var fromParcelRoute = RouteHelper.GetRouteFor<UsageLocationParcelHistoryController>(x => x.List(geographyID, fromParcelID));
        var fromParcelResult = await AssemblySteps.AdminHttpClient.GetAsync(fromParcelRoute);
        var fromParcelResultContentAsString = await fromParcelResult.Content.ReadAsStringAsync();
        Assert.IsTrue(fromParcelResult.IsSuccessStatusCode, $"\nRoute: {fromParcelRoute}\n{fromParcelResultContentAsString}");
        Console.WriteLine(fromParcelResultContentAsString);

        var fromParcelHistories = await fromParcelResult.DeserializeContentAsync<List<UsageLocationParcelHistoryDto>>();
        Assert.IsTrue(fromParcelHistories.Any(), "No usage location parcel histories found for the from parcel.");
        Assert.IsTrue(fromParcelHistories.Any(x => usageLocationIDsToMigrate.Contains(x.UsageLocationID)), "Not all usage locations are present in the from parcel histories.");

        var toParcelRoute = RouteHelper.GetRouteFor<UsageLocationParcelHistoryController>(x => x.List(geographyID, toParcel.ParcelID));
        var toParcelResult = await AssemblySteps.AdminHttpClient.GetAsync(toParcelRoute);
        var toParcelResultContentAsString = await toParcelResult.Content.ReadAsStringAsync();
        Assert.IsTrue(toParcelResult.IsSuccessStatusCode, $"\nRoute: {toParcelRoute}\n{toParcelResultContentAsString}");
        Console.WriteLine(toParcelResultContentAsString);

        var toParcelHistories = await toParcelResult.DeserializeContentAsync<List<UsageLocationParcelHistoryDto>>();
        Assert.IsTrue(toParcelHistories.Any(x => x.ToParcelID == toParcel.ParcelID), "No usage location parcel histories found for the to parcel.");
        Assert.IsTrue(toParcelHistories.Any(x => usageLocationIDsToMigrate.Contains(x.UsageLocationID)), "Not all usage locations are present in the to parcel histories.");

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
}