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
public class WaterAccountFallowStatusControllerTests
{
    [DataRow(5, 2024)]
    [DataRow(7, 2024)]
    [TestMethod]
    public async Task AdminCanCreateWaterAccountFallowStatus(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var route = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var waterAccountFallowStatus = await result.Content.ReadFromJsonAsync<WaterAccountFallowStatusDto>();

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountFallowStatuses
            .Where(x => x.WaterAccountFallowStatusID == waterAccountFallowStatus.WaterAccountFallowStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanCreateWaterAccountFallowStatus_BadRequest_AlreadyExists(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var route = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var waterAccountFallowStatus = await result.Content.ReadFromJsonAsync<WaterAccountFallowStatusDto>();

        var badRequestResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        var badRequestResultAsString = await badRequestResult.Content.ReadAsStringAsync();
        Console.WriteLine(badRequestResultAsString);

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountFallowStatuses
            .Where(x => x.WaterAccountFallowStatusID == waterAccountFallowStatus.WaterAccountFallowStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [DataRow(7, 2024)]
    [TestMethod]
    public async Task AdminCanList(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var route = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.List(geographyID, reportingPeriod.ReportingPeriodID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.IsTrue(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());

        var waterAccountFallowStatuses = await result.Content.ReadFromJsonAsync<List<WaterAccountFallowStatusDto>>();

        Assert.IsNotNull(waterAccountFallowStatuses);
        Assert.IsTrue(waterAccountFallowStatuses.Count > 0);
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanGet(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);
        var waterAccountFallowStatus = AssemblySteps.QanatDbContext.WaterAccountFallowStatuses
            .Include(x => x.Geography).ThenInclude(x => x.GeographyConfiguration)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.Geography)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UsageLocationType)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.ReportingPeriod)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.CreateUser)
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.UpdateUser)
            .FirstOrDefault(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            ?.AsDto(reportingPeriod.ReportingPeriodID);

        var cleanupEntity = false;
        if (waterAccountFallowStatus == null)
        {
            var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
                .Include(x => x.WaterAccountParcels)
                .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

            var createRoute = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
            var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, new { }, AssemblySteps.DefaultJsonSerializerOptions);
            var resultAsString = await createResult.Content.ReadAsStringAsync();
            Assert.IsTrue(createResult.IsSuccessStatusCode, resultAsString);
            Console.WriteLine(resultAsString);
            waterAccountFallowStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountFallowStatusDto>();
            cleanupEntity = true;
        }

        var route = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Get(geographyID, reportingPeriod.ReportingPeriodID, waterAccountFallowStatus.WaterAccountFallowStatusID.Value));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        Assert.IsTrue(result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());

        var waterAccountFallowStatusDto = await result.Content.ReadFromJsonAsync<WaterAccountFallowStatusDto>();
        Assert.IsNotNull(waterAccountFallowStatusDto);

        if (cleanupEntity)
        {
            //Clean up entity so we can rerun test safely.
            await AssemblySteps.QanatDbContext.WaterAccountFallowStatuses
                .Where(x => x.WaterAccountFallowStatusID == waterAccountFallowStatusDto.WaterAccountFallowStatusID)
                .ExecuteDeleteAsync();
        }
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanSubmitFallowStatus(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var createRoute = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var createResultAsString = await createResult.Content.ReadAsStringAsync();
        Assert.IsTrue(createResult.IsSuccessStatusCode, createResultAsString);

        var waterAccountFallowStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountFallowStatusDto>();
        Assert.IsNotNull(waterAccountFallowStatus);

        var usageLocation = waterAccount.WaterAccountParcels
            .Where(x => x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            .SelectMany(x => x.Parcel.UsageLocations)
            .FirstOrDefault(x => x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);

        Assert.IsNotNull(usageLocation);

        var usageLocationTypeThatCountsAsFallowed = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.CountsAsFallowed);

        Assert.IsNotNull(usageLocationTypeThatCountsAsFallowed);

        usageLocation.UsageLocationTypeID = usageLocationTypeThatCountsAsFallowed.UsageLocationTypeID;

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        // Submit the fallow status.
        var submitRoute = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Submit(geographyID, reportingPeriod.ReportingPeriodID, waterAccountFallowStatus.WaterAccountFallowStatusID.Value));
        var submitResult = await AssemblySteps.AdminHttpClient.PutAsync(submitRoute, null);
        var submitResultAsString = await submitResult.Content.ReadAsStringAsync();
        Assert.IsTrue(submitResult.IsSuccessStatusCode, submitResultAsString);
        Console.WriteLine(submitResultAsString);

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountFallowStatuses
            .Where(x => x.WaterAccountFallowStatusID == waterAccountFallowStatus.WaterAccountFallowStatusID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, 2024)]
    [TestMethod]
    public async Task AdminCanSubmitFallowStatus_BadRequest_NoUsageLocationThatCountAsFallowed(int geographyID, int year)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, year);
        Assert.IsNotNull(reportingPeriod);

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Select(wap => wap.ReportingPeriodID).Contains(reportingPeriod.ReportingPeriodID));

        var createRoute = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Create(geographyID, reportingPeriod.ReportingPeriodID, waterAccount.WaterAccountID));
        var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, new { }, AssemblySteps.DefaultJsonSerializerOptions);
        var createResultAsString = await createResult.Content.ReadAsStringAsync();
        Assert.IsTrue(createResult.IsSuccessStatusCode, createResultAsString);

        var waterAccountFallowStatus = await createResult.Content.ReadFromJsonAsync<WaterAccountFallowStatusDto>();
        Assert.IsNotNull(waterAccountFallowStatus);

        var usageLocations = waterAccount.WaterAccountParcels
            .Where(x => x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            .SelectMany(x => x.Parcel.UsageLocations)
            .Where(x => x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
            .ToList();

        Assert.IsTrue(usageLocations.Any());

        var usageLocationTypeThatDoesNotCountAsFallowed = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && !x.CountsAsFallowed);

        Assert.IsNotNull(usageLocationTypeThatDoesNotCountAsFallowed);

        foreach (var usageLocation in usageLocations)
        {
            usageLocation.UsageLocationTypeID = usageLocationTypeThatDoesNotCountAsFallowed.UsageLocationTypeID;
        }

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        // Submit the fallow status.
        var submitRoute = RouteHelper.GetRouteFor<WaterAccountFallowStatusController>(x => x.Submit(geographyID, reportingPeriod.ReportingPeriodID, waterAccountFallowStatus.WaterAccountFallowStatusID.Value));
        var submitResult = await AssemblySteps.AdminHttpClient.PutAsync(submitRoute, null);
        Assert.AreEqual(HttpStatusCode.BadRequest, submitResult.StatusCode);
        var submitResultAsString = await submitResult.Content.ReadAsStringAsync();
        Console.WriteLine(submitResultAsString);

        //Clean up entity so we can rerun test safely.
        await AssemblySteps.QanatDbContext.WaterAccountFallowStatuses
            .Where(x => x.WaterAccountFallowStatusID == waterAccountFallowStatus.WaterAccountFallowStatusID)
            .ExecuteDeleteAsync();
    }
}