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
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers.WaterAccount;

[TestClass]
public class WaterAccountFallowStatusReviewControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanList(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<WaterAccountFallowStatusReviewController>(x => x.List(geographyID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanApprove(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels)
            .ThenInclude(x => x.Parcel)
            .ThenInclude(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var createRoute = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var createResultAsString = await createResult.Content.ReadAsStringAsync();
        Assert.IsTrue(createResult.IsSuccessStatusCode, createResultAsString);

        var waterAccountFallowStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountFallowStatusDto>();
        Assert.IsNotNull(waterAccountFallowStatus);

        var fallowStatusIDs = new List<int> { waterAccountFallowStatus.WaterAccountFallowStatusID!.Value };
        var route = RouteHelper.GetRouteFor<WaterAccountFallowStatusReviewController>(x => x.Approve(geographyID, fallowStatusIDs));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, fallowStatusIDs, AssemblySteps.DefaultJsonSerializerOptions);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var updatedWaterAccountFallowStatuses = await result.Content.ReadFromJsonAsync<List<WaterAccountFallowStatusDto>>();
        Assert.AreEqual(1, updatedWaterAccountFallowStatuses.Count);

        var updatedWaterAccountFallowStatus = updatedWaterAccountFallowStatuses.FirstOrDefault(x => x.WaterAccountFallowStatusID == waterAccountFallowStatus.WaterAccountFallowStatusID);
        Assert.IsNotNull(updatedWaterAccountFallowStatus);
        Assert.AreEqual(SelfReportStatus.Approved.SelfReportStatusID, updatedWaterAccountFallowStatus.SelfReportStatus.SelfReportStatusID);
        Assert.IsNotNull(updatedWaterAccountFallowStatus.ApprovedByUser);
        Assert.IsNotNull(updatedWaterAccountFallowStatus.ApprovedDate);

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountFallowStatuses
            .Where(x => x.WaterAccountFallowStatusID == waterAccountFallowStatus.WaterAccountFallowStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanApprove_BadRequest_InvalidWaterAccountFallowStatusID(int geographyID, int year)
    {
        var fallowStatusIDs = new List<int> { -1 };
        var route = RouteHelper.GetRouteFor<WaterAccountFallowStatusReviewController>(x => x.Approve(geographyID, fallowStatusIDs));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, fallowStatusIDs, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

        var resultAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(resultAsString);
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanReturn(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels)
            .ThenInclude(x => x.Parcel)
            .ThenInclude(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var createRoute = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var createResultAsString = await createResult.Content.ReadAsStringAsync();
        Assert.IsTrue(createResult.IsSuccessStatusCode, createResultAsString);

        var waterAccountFallowStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountFallowStatusDto>();
        Assert.IsNotNull(waterAccountFallowStatus);

        var fallowStatusIDs = new List<int> { waterAccountFallowStatus.WaterAccountFallowStatusID!.Value };
        var route = RouteHelper.GetRouteFor<WaterAccountFallowStatusReviewController>(x => x.Return(geographyID, fallowStatusIDs));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, fallowStatusIDs, AssemblySteps.DefaultJsonSerializerOptions);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var updatedWaterAccountFallowStatuses = await result.Content.ReadFromJsonAsync<List<WaterAccountFallowStatusDto>>();
        Assert.AreEqual(1, updatedWaterAccountFallowStatuses.Count);

        var updatedWaterAccountFallowStatus = updatedWaterAccountFallowStatuses.FirstOrDefault(x => x.WaterAccountFallowStatusID == waterAccountFallowStatus.WaterAccountFallowStatusID);
        Assert.IsNotNull(updatedWaterAccountFallowStatus);
        Assert.AreEqual(SelfReportStatus.Returned.SelfReportStatusID, updatedWaterAccountFallowStatus.SelfReportStatus.SelfReportStatusID);
        Assert.IsNotNull(updatedWaterAccountFallowStatus.ReturnedByUser);
        Assert.IsNotNull(updatedWaterAccountFallowStatus.ReturnedDate);

        // Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountFallowStatuses
            .Where(x => x.WaterAccountFallowStatusID == waterAccountFallowStatus.WaterAccountFallowStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanReturn_BadRequest_InvalidWaterAccountFallowStatusID(int geographyID, int year)
    {
        var fallowStatusIDs = new List<int> { -1 };
        var route = RouteHelper.GetRouteFor<WaterAccountFallowStatusReviewController>(x => x.Return(geographyID, fallowStatusIDs));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, fallowStatusIDs, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(resultAsString);
    }
}
