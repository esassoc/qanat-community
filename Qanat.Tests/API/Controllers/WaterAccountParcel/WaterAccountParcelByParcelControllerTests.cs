using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;

namespace Qanat.Tests.API.Controllers.WaterAccountParcel;

[TestClass]
public class WaterAccountParcelByParcelControllerTests
{
    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanGetWaterAccountParcelsForParcel(int geographyID)
    {
        var parcel = await AssemblySteps.QanatDbContext.Parcels.AsNoTracking()
            .FirstOrDefaultAsync(p => p.GeographyID == geographyID);

        Assert.IsNotNull(parcel);

        var waterAccountParcelsForParcel = await AssemblySteps.QanatDbContext.WaterAccountParcels
            .Where(wap => wap.ParcelID == parcel.ParcelID)
            .ToListAsync();

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByParcelController>(c => c.GetWaterAccountParcelsForParcel(parcel.ParcelID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);

        Console.WriteLine(resultAsString);

        var resultAsDtos = JsonSerializer.Deserialize<List<WaterAccountMinimalAndReportingPeriodSimpleDto>>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.IsNotNull(resultAsDtos); // Ensure it deserialized correctly

        if (waterAccountParcelsForParcel.Any())
        {
            foreach (var waterAccountParcel in waterAccountParcelsForParcel)
            {
                var r = resultAsDtos.FirstOrDefault(x => x.WaterAccount.WaterAccountID == waterAccountParcel.WaterAccountID && x.ReportingPeriod.ReportingPeriodID == waterAccountParcel.ReportingPeriodID);
                Assert.IsNotNull(r);
            }
        }
        else
        {
            Assert.AreEqual(0, resultAsDtos.Count);
        }
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanUpdateWaterAccountParcelsForParcel(int geographyID)
    {
        var parcel = await AssemblySteps.QanatDbContext.Parcels
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(p => p.GeographyID == geographyID && p.WaterAccountParcels.Any());

        Assert.IsNotNull(parcel);

        var waterAccountParcelsForParcel = await AssemblySteps.QanatDbContext.WaterAccountParcels.Where(wap => wap.ParcelID == parcel.ParcelID).ToListAsync();
        Assert.IsTrue(waterAccountParcelsForParcel.Count > 0);

        var updateDto = new UpdateWaterAccountParcelsByParcelDto
        {
            ReportingPeriodWaterAccounts = waterAccountParcelsForParcel.Select(x => new WaterAccountReportingPeriodDto()
            {
                ReportingPeriodID = x.ReportingPeriodID,
                WaterAccountID = null
            }).ToList()
        };

        var firstWaterAccountID = waterAccountParcelsForParcel[0].WaterAccountID;
        var waterAccountToUpdateTo = await AssemblySteps.QanatDbContext.WaterAccounts.FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountID != firstWaterAccountID);

        updateDto.ReportingPeriodWaterAccounts[0].WaterAccountID = waterAccountToUpdateTo.WaterAccountID;

        var json = JsonSerializer.Serialize(updateDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<WaterAccountParcelByParcelController>(c => c.UpdateWaterAccountParcelsForParcel(parcel.ParcelID, null));
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);

        Console.WriteLine(resultAsString);

        // Reset back to what was there.
        var resetDto = new UpdateWaterAccountParcelsByParcelDto
        {
            ReportingPeriodWaterAccounts = waterAccountParcelsForParcel.Select(x => new WaterAccountReportingPeriodDto()
            {
                ReportingPeriodID = x.ReportingPeriodID,
                WaterAccountID = x.WaterAccountID
            }).ToList()
        };

        var resetJson = JsonSerializer.Serialize(resetDto, AssemblySteps.DefaultJsonSerializerOptions);
        var resetContent = new StringContent(resetJson, Encoding.UTF8, "application/json");

        var resetResult = await AssemblySteps.AdminHttpClient.PutAsync(route, resetContent);
        var resetResultAsString = await resetResult.Content.ReadAsStringAsync();
        Assert.IsTrue(resetResult.IsSuccessStatusCode, resetResultAsString);

        var resetValues = JsonSerializer.Deserialize<List<WaterAccountMinimalAndReportingPeriodSimpleDto>>(resetResultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.IsNotNull(resetValues); // Ensure it deserialized correctly

        foreach (var waterAccountParcel in waterAccountParcelsForParcel)
        {
            var resetValue = resetValues.FirstOrDefault(x => x.WaterAccount.WaterAccountID == waterAccountParcel.WaterAccountID && x.ReportingPeriod.ReportingPeriodID == waterAccountParcel.ReportingPeriodID);
            Assert.IsNotNull(resetValue);
        }
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task UpdateWaterAccountParcelsForParcel_BadRequest(int geographyID)
    {
        var parcel = await AssemblySteps.QanatDbContext.Parcels.FirstOrDefaultAsync(p => p.GeographyID == geographyID);
        Assert.IsNotNull(parcel);

        var waterAccountParcelsForParcel = await AssemblySteps.QanatDbContext.WaterAccountParcels.Where(wap => wap.ParcelID == parcel.ParcelID).ToListAsync();
        var updateDto = new UpdateWaterAccountParcelsByParcelDto
        {
            ReportingPeriodWaterAccounts = waterAccountParcelsForParcel.Select(x => new WaterAccountReportingPeriodDto()
            {
                ReportingPeriodID = x.ReportingPeriodID,
                WaterAccountID = x.WaterAccountID
            }).ToList()
        };

        updateDto.ReportingPeriodWaterAccounts[0].ReportingPeriodID = -1;
        updateDto.ReportingPeriodWaterAccounts[0].WaterAccountID = -1;

        updateDto.ReportingPeriodWaterAccounts[1].ReportingPeriodID = updateDto.ReportingPeriodWaterAccounts[2].ReportingPeriodID;

        var json = JsonSerializer.Serialize(updateDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var route = RouteHelper.GetRouteFor<WaterAccountParcelByParcelController>(c => c.UpdateWaterAccountParcelsForParcel(parcel.ParcelID, null));
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsFalse(result.IsSuccessStatusCode, resultAsString);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

        Console.WriteLine(resultAsString);
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanGetWaterAccountParcelHistory(int geographyID)
    {
        var parcel = await AssemblySteps.QanatDbContext.Parcels.AsNoTracking()
            .FirstOrDefaultAsync(p => p.GeographyID == geographyID);

        Assert.IsNotNull(parcel);

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByParcelController>(c => c.GetWaterAccountParcelHistory(parcel.ParcelID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);

        Console.WriteLine(resultAsString);
    }
}