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
public class WaterAccountCoverCropStatusReviewControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanList(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusReviewController>(x => x.List(geographyID));
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

        var createRoute = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var createResultAsString = await createResult.Content.ReadAsStringAsync();
        Assert.IsTrue(createResult.IsSuccessStatusCode, createResultAsString);

        var waterAccountCoverCropStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountCoverCropStatusDto>();
        Assert.IsNotNull(waterAccountCoverCropStatus);

        var coverCropStatusIDs = new List<int> { waterAccountCoverCropStatus.WaterAccountCoverCropStatusID!.Value };
        var route = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusReviewController>(x => x.Approve(geographyID, coverCropStatusIDs));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, coverCropStatusIDs, AssemblySteps.DefaultJsonSerializerOptions);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var updatedWaterAccountCoverCropStatuses = await result.Content.ReadFromJsonAsync<List<WaterAccountCoverCropStatusDto>>();
        Assert.AreEqual(1, updatedWaterAccountCoverCropStatuses.Count);

        var updatedWaterAccountCoverCropStatus = updatedWaterAccountCoverCropStatuses.FirstOrDefault(x => x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatus.WaterAccountCoverCropStatusID);
        Assert.IsNotNull(updatedWaterAccountCoverCropStatus);
        Assert.AreEqual(SelfReportStatus.Approved.SelfReportStatusID, updatedWaterAccountCoverCropStatus.SelfReportStatus.SelfReportStatusID);
        Assert.IsNotNull(updatedWaterAccountCoverCropStatus.ApprovedByUser);
        Assert.IsNotNull(updatedWaterAccountCoverCropStatus.ApprovedDate);

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountCoverCropStatuses
            .Where(x => x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatus.WaterAccountCoverCropStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanApprove_BadRequest_InvalidWaterAccountCoverCropStatusID(int geographyID, int year)
    {
        var coverCropStatusIDs = new List<int> { -1 };
        var route = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusReviewController>(x => x.Approve(geographyID, coverCropStatusIDs));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, coverCropStatusIDs, AssemblySteps.DefaultJsonSerializerOptions);
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

        var createRoute = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var createResultAsString = await createResult.Content.ReadAsStringAsync();
        Assert.IsTrue(createResult.IsSuccessStatusCode, createResultAsString);

        var waterAccountCoverCropStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountCoverCropStatusDto>();
        Assert.IsNotNull(waterAccountCoverCropStatus);

        var coverCropStatusIDs = new List<int> { waterAccountCoverCropStatus.WaterAccountCoverCropStatusID!.Value };
        var route = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusReviewController>(x => x.Return(geographyID, coverCropStatusIDs));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, coverCropStatusIDs, AssemblySteps.DefaultJsonSerializerOptions);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var updatedWaterAccountCoverCropStatuses = await result.Content.ReadFromJsonAsync<List<WaterAccountCoverCropStatusDto>>();
        Assert.AreEqual(1, updatedWaterAccountCoverCropStatuses.Count);

        var updatedWaterAccountCoverCropStatus = updatedWaterAccountCoverCropStatuses.FirstOrDefault(x => x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatus.WaterAccountCoverCropStatusID);
        Assert.IsNotNull(updatedWaterAccountCoverCropStatus);
        Assert.AreEqual(SelfReportStatus.Returned.SelfReportStatusID, updatedWaterAccountCoverCropStatus.SelfReportStatus.SelfReportStatusID);
        Assert.IsNotNull(updatedWaterAccountCoverCropStatus.ReturnedByUser);
        Assert.IsNotNull(updatedWaterAccountCoverCropStatus.ReturnedDate);

        // Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountCoverCropStatuses
            .Where(x => x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatus.WaterAccountCoverCropStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanReturn_BadRequest_InvalidWaterAccountCoverCropStatusID(int geographyID, int year)
    {
        var coverCropStatusIDs = new List<int> { -1 };
        var route = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusReviewController>(x => x.Return(geographyID, coverCropStatusIDs));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, coverCropStatusIDs, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(resultAsString);
    }
}
