using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;

namespace Qanat.Tests.API.Controllers.UsageLocation;

[TestClass]
public class UsageLocationByWaterAccountControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanList(int geographyID)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.Parcels).ThenInclude(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.Parcels.Any(p => p.UsageLocations.Any()));

        Assert.IsNotNull(waterAccount);

        var route = RouteHelper.GetRouteFor<UsageLocationByWaterAccountController>(x => x.List(geographyID, waterAccount.WaterAccountID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultContentAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var usageLocationDtos = await result.DeserializeContentAsync<List<UsageLocationDto>>();

        //Check that we see all usage locations for the water account
        Assert.AreEqual(waterAccount.Parcels.Sum(p => p.UsageLocations.Count), usageLocationDtos.Count);
        var usageLocations = waterAccount.Parcels.SelectMany(x => x.UsageLocations).ToList();
        foreach (var usageLocation in usageLocations)
        {
            var usageLocationDto = usageLocationDtos.FirstOrDefault(x => x.UsageLocationID == usageLocation.UsageLocationID);
            Assert.IsNotNull(usageLocationDto);
        }
    }
}