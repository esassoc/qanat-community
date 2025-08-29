using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.EFModels.Entities;
using Qanat.EFModels.Entities.ExtensionMethods;
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
public class WaterAccountCoverCropStatusControllerTests
{
    [DataRow(5, 2024)]
    [DataRow(7, 2024)]
    [TestMethod]
    public async Task AdminCanCreateWaterAccountCoverCropStatus(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var route = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var waterAccountCoverCropStatus = await result.Content.ReadFromJsonAsync<WaterAccountCoverCropStatusDto>();

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountCoverCropStatuses
            .Where(x => x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatus.WaterAccountCoverCropStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanCreateWaterAccountCoverCropStatus_BadRequest_AlreadyExists(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var route = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var waterAccountCoverCropStatus = await result.Content.ReadFromJsonAsync<WaterAccountCoverCropStatusDto>();

        var badRequestResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        var badRequestResultAsString = await badRequestResult.Content.ReadAsStringAsync();
        Console.WriteLine(badRequestResultAsString);

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountCoverCropStatuses
            .Where(x => x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatus.WaterAccountCoverCropStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [DataRow(7, 2024)]
    [TestMethod]
    public async Task AdminCanList(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var route = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.List(geographyID, reportingPeriod.ReportingPeriodID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.IsTrue(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());

        var waterAccountCoverCropStatuses = await result.Content.ReadFromJsonAsync<List<WaterAccountCoverCropStatusDto>>();

        Assert.IsNotNull(waterAccountCoverCropStatuses);
        Assert.IsTrue(waterAccountCoverCropStatuses.Count > 0);
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanGet(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);
        var waterAccountCoverCropStatus = AssemblySteps.QanatDbContext.WaterAccountCoverCropStatuses
            .Include(x => x.Geography).ThenInclude(x => x.GeographyConfiguration)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.Geography)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UsageLocationType)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.ReportingPeriod)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.CreateUser)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UpdateUser)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UpdateUser)
            .FirstOrDefault(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            ?.AsDto(reportingPeriod.ReportingPeriodID);

        var cleanupEntity = false;
        if (waterAccountCoverCropStatus == null)
        {
            var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
                .Include(x => x.WaterAccountParcels)
                .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

            var createRoute = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
            var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, new { }, AssemblySteps.DefaultJsonSerializerOptions);
            var resultAsString = await createResult.Content.ReadAsStringAsync();
            Assert.IsTrue(createResult.IsSuccessStatusCode, resultAsString);
            Console.WriteLine(resultAsString);
            waterAccountCoverCropStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountCoverCropStatusDto>();
            cleanupEntity = true;
        }

        var route = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Get(geographyID, reportingPeriod.ReportingPeriodID, waterAccountCoverCropStatus.WaterAccountCoverCropStatusID.Value));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.IsTrue(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());

        var waterAccountCoverCropStatusDto = await result.Content.ReadFromJsonAsync<WaterAccountCoverCropStatusDto>();
        Assert.IsNotNull(waterAccountCoverCropStatusDto);

        if (cleanupEntity)
        {
            //Clean up entity so we can rerun test safely.
            await AssemblySteps.QanatDbContext.WaterAccountCoverCropStatuses
                .Where(x => x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatusDto.WaterAccountCoverCropStatusID)
                .ExecuteDeleteAsync();
        }
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanSubmitCoverCropStatus(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var createRoute = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var createResult = await AssemblySteps.AdminHttpClient.PostAsync(createRoute, null);
        var createResultAsString = await createResult.Content.ReadAsStringAsync();
        Assert.IsTrue(createResult.IsSuccessStatusCode, createResultAsString);

        var waterAccountCoverCropStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountCoverCropStatusDto>();
        Assert.IsNotNull(waterAccountCoverCropStatus);

        var usageLocation = waterAccount.WaterAccountParcels
            .Where(x => x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            .SelectMany(x => x.Parcel.UsageLocations)
            .FirstOrDefault(x => x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);

        Assert.IsNotNull(usageLocation);

        var usageLocationTypeThatCountsAsCoverCropped = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.CountsAsCoverCropped);

        Assert.IsNotNull(usageLocationTypeThatCountsAsCoverCropped);

        usageLocation.UsageLocationTypeID = usageLocationTypeThatCountsAsCoverCropped.UsageLocationTypeID;
        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        // Submit the cover crop status.
        var submitRoute = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Submit(geographyID, reportingPeriod.ReportingPeriodID, waterAccountCoverCropStatus.WaterAccountCoverCropStatusID.Value));
        var submitResult = await AssemblySteps.AdminHttpClient.PutAsync(submitRoute, null);
        var submitResultAsString = await submitResult.Content.ReadAsStringAsync();
        Assert.IsTrue(submitResult.IsSuccessStatusCode, submitResultAsString);
        Console.WriteLine(submitResultAsString);

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountCoverCropStatuses
            .Where(x => x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatus.WaterAccountCoverCropStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanSubmitCoverCropStatus_BadRequest_NoUsageLocationThatCountsAsCoverCrop(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var createRoute = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var createResult = await AssemblySteps.AdminHttpClient.PostAsync(createRoute, null);
        var createResultAsString = await createResult.Content.ReadAsStringAsync();
        Assert.IsTrue(createResult.IsSuccessStatusCode, createResultAsString);

        var waterAccountCoverCropStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountCoverCropStatusDto>();
        Assert.IsNotNull(waterAccountCoverCropStatus);

        var usageLocations = waterAccount.WaterAccountParcels
            .Where(x => x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            .SelectMany(x => x.Parcel.UsageLocations)
            .Where(x => x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            .ToList();

        Assert.IsTrue(usageLocations.Any());

        var usageLocationTypeThatDoesNotCountAsCoverCropped = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && !x.CountsAsCoverCropped);

        Assert.IsNotNull(usageLocationTypeThatDoesNotCountAsCoverCropped);

        foreach (var usageLocation in usageLocations)
        {
            usageLocation.UsageLocationTypeID = usageLocationTypeThatDoesNotCountAsCoverCropped.UsageLocationTypeID;
        }

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        // Submit the cover crop status.
        var submitRoute = RouteHelper.GetRouteFor<WaterAccountCoverCropStatusController>(x => x.Submit(geographyID, reportingPeriod.ReportingPeriodID, waterAccountCoverCropStatus.WaterAccountCoverCropStatusID.Value));
        var submitResult = await AssemblySteps.AdminHttpClient.PutAsync(submitRoute, null);
        Assert.AreEqual(HttpStatusCode.BadRequest, submitResult.StatusCode);
        var submitResultAsString = await submitResult.Content.ReadAsStringAsync();
        Console.WriteLine(submitResultAsString);

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountCoverCropStatuses
            .Where(x => x.WaterAccountCoverCropStatusID == waterAccountCoverCropStatus.WaterAccountCoverCropStatusID)
            .ExecuteDeleteAsync();
    }
}
