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
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;

namespace Qanat.Tests.API.Controllers.WaterAccountParcel;

[TestClass]
public class WaterAccountParcelByWaterAccountControllerTests
{
    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanGetWaterAccountParcels(int geographyID)
    {
        var waterAccountWithWaterAccountParcels = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .Include(wa => wa.WaterAccountParcels)
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID && wa.WaterAccountParcels.Any());

        Assert.IsNotNull(waterAccountWithWaterAccountParcels);

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.GetWaterAccountParcels(waterAccountWithWaterAccountParcels.WaterAccountID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);

        Console.WriteLine(resultAsString);

        var resultAsDtos = JsonSerializer.Deserialize<List<ParcelIndexGridDto>>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.IsNotNull(resultAsDtos);
    }

    [DataRow(5, 2021)] //Demo
    [TestMethod]
    public async Task AdminCanGetCurrentParcelsFromAccountID(int geographyID, int year)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID);

        Assert.IsNotNull(waterAccount);

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.GetCurrentParcelsFromAccountID(waterAccount.WaterAccountID, year));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);

        Console.WriteLine(resultAsString);

        var resultAsDtos = JsonSerializer.Deserialize<List<ParcelMinimalDto>>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.IsNotNull(resultAsDtos);
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanUpdateWaterAccountParcels(int geographyID)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID && wa.WaterAccountParcels.Any());

        var reportingPeriods = await AssemblySteps.QanatDbContext.ReportingPeriods.AsNoTracking()
            .Where(rp => rp.GeographyID == geographyID)
            .OrderByDescending(x => x.EndDate)
            .ToListAsync(); 
            
        var latestTwoReportingPeriods = reportingPeriods.Take(2).ToList();
        var selectedReportingPeriod = latestTwoReportingPeriods.MinBy(x => x.EndDate);

        var parcelIDsToRestore = waterAccount.WaterAccountParcels.Where(x => x.ReportingPeriodID == selectedReportingPeriod.ReportingPeriodID).Select(x => x.ParcelID).ToList();

        var parcelNotAssociatedToWaterAccountForReportingPeriod = await AssemblySteps.QanatDbContext.Parcels
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterAccountParcels.Any(wap => wap.WaterAccountID != waterAccount.WaterAccountID && wap.ReportingPeriodID != selectedReportingPeriod.ReportingPeriodID));

        var updateDto = new WaterAccountParcelsUpdateDto()
        {
            ReportingPeriodID = selectedReportingPeriod.ReportingPeriodID,
            ParcelIDs = [parcelNotAssociatedToWaterAccountForReportingPeriod.ParcelID]
        };

        var json = JsonSerializer.Serialize(updateDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.UpdateWaterAccountParcels(waterAccount.WaterAccountID, null)), content);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);

        Console.WriteLine(resultAsString);

        var resultAsDtos = JsonSerializer.Deserialize<List<ParcelMinimalDto>>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);

        Assert.IsNotNull(resultAsDtos);
        Assert.IsTrue(resultAsDtos.Count == 1);
        Assert.AreEqual(parcelNotAssociatedToWaterAccountForReportingPeriod.ParcelID, resultAsDtos[0].ParcelID);


        //Check that the latest reporting period was updated as well.
        var latestReportingPeriod = latestTwoReportingPeriods.MaxBy(x => x.EndDate);
        Assert.IsNotNull(latestReportingPeriod);

        var latestReportingPeriodWaterAccountParcels = await AssemblySteps.QanatDbContext.WaterAccountParcels.AsNoTracking()
            .Where(x => x.WaterAccountID == waterAccount.WaterAccountID && x.ReportingPeriodID == latestReportingPeriod.ReportingPeriodID)
            .ToListAsync();

        Assert.IsNotNull(latestReportingPeriodWaterAccountParcels);
        Assert.AreEqual(1, latestReportingPeriodWaterAccountParcels.Count);
        Assert.AreEqual(parcelNotAssociatedToWaterAccountForReportingPeriod.ParcelID, latestReportingPeriodWaterAccountParcels[0].ParcelID);

        //Restore the parcels so we don't break other tests. Doing it through the API to test cover more cases as a bonus.
        var restoreDto = new WaterAccountParcelsUpdateDto()
        {
            ReportingPeriodID = selectedReportingPeriod.ReportingPeriodID,
            ParcelIDs = parcelIDsToRestore
        };

        var restoreJson = JsonSerializer.Serialize(restoreDto, AssemblySteps.DefaultJsonSerializerOptions);
        var restoreContent = new StringContent(restoreJson, Encoding.UTF8, "application/json");
        var restoreResult = await AssemblySteps.AdminHttpClient.PutAsync(RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.UpdateWaterAccountParcels(waterAccount.WaterAccountID, null)), restoreContent);
        var restoreResultAsString = await restoreResult.Content.ReadAsStringAsync();
        Assert.IsTrue(restoreResult.IsSuccessStatusCode, restoreResultAsString);

        var restoreResultAsDtos = JsonSerializer.Deserialize<List<ParcelMinimalDto>>(restoreResultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.IsNotNull(restoreResultAsDtos);
        Assert.IsTrue(restoreResultAsDtos.Count == parcelIDsToRestore.Count);

        foreach (var restoreResultAsDto in restoreResultAsDtos)
        {
            var parcelID = parcelIDsToRestore.FirstOrDefault(x => x == restoreResultAsDto.ParcelID);
            Assert.IsNotNull(parcelID);
        }
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanUpdateWaterAccountParcels_BadRequest_InvalidReportingPeriodID(int geographyID)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID && wa.WaterAccountParcels.Any());

        var updateDto = new WaterAccountParcelsUpdateDto()
        {
            ReportingPeriodID = -1,
            ParcelIDs = [waterAccount.WaterAccountParcels.First().ParcelID]
        };

        var json = JsonSerializer.Serialize(updateDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.UpdateWaterAccountParcels(waterAccount.WaterAccountID, null)), content);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsFalse(result.IsSuccessStatusCode, resultAsString);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanUpdateWaterAccountParcels_BadRequest_InvalidParcelID(int geographyID)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID && wa.WaterAccountParcels.Any());

        var reportingPeriodID = waterAccount.WaterAccountParcels.First().ReportingPeriodID;

        var updateDto = new WaterAccountParcelsUpdateDto()
        {
            ReportingPeriodID = reportingPeriodID,
            ParcelIDs = [-1]
        };

        var json = JsonSerializer.Serialize(updateDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.UpdateWaterAccountParcels(waterAccount.WaterAccountID, null)), content);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsFalse(result.IsSuccessStatusCode, resultAsString);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanUpdateWaterAccountParcels_BadRequest_ParcelNotInGeography(int geographyID)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID && wa.WaterAccountParcels.Any());

        var reportingPeriodID = waterAccount.WaterAccountParcels.First().ReportingPeriodID;

        var parcelNotInGeography = await AssemblySteps.QanatDbContext.Parcels
            .Include(x => x.WaterAccountParcels)
            .FirstOrDefaultAsync(x => x.GeographyID != geographyID);

        var updateDto = new WaterAccountParcelsUpdateDto()
        {
            ReportingPeriodID = reportingPeriodID,
            ParcelIDs = [parcelNotInGeography.ParcelID]
        };

        var json = JsonSerializer.Serialize(updateDto, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.UpdateWaterAccountParcels(waterAccount.WaterAccountID, null)), content);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsFalse(result.IsSuccessStatusCode, resultAsString);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanAddOrphanedParcelToWaterAccount(int geographyID)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID);

        var orphanedWaterAccountParcel = await AssemblySteps.QanatDbContext.Parcels
            .FirstOrDefaultAsync(p => p.GeographyID == geographyID && !p.WaterAccountID.HasValue);

        Assert.IsNotNull(waterAccount);
        Assert.IsNotNull(orphanedWaterAccountParcel, "If this is fails we need to modify the test to orphan a parcel :(");

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.AddOrphanedParcelToWaterAccount(waterAccount.WaterAccountID, orphanedWaterAccountParcel.ParcelID));
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, null);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);

        Console.WriteLine(resultAsString);

        //Re-orphan the parcel :(
        orphanedWaterAccountParcel.WaterAccountID = null;
        orphanedWaterAccountParcel.ParcelStatusID = ParcelStatus.Unassigned.ParcelStatusID;

        AssemblySteps.QanatDbContext.Parcels.Update(orphanedWaterAccountParcel);

        var waterAccountParcelsToRemove = await AssemblySteps.QanatDbContext.WaterAccountParcels
            .Where(wap => wap.WaterAccountID == waterAccount.WaterAccountID && wap.ParcelID == orphanedWaterAccountParcel.ParcelID)
            .ToListAsync();

        AssemblySteps.QanatDbContext.WaterAccountParcels.RemoveRange(waterAccountParcelsToRemove);
        await AssemblySteps.QanatDbContext.SaveChangesAsync();
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanAddOrphanedParcelToWaterAccount_BadRequest_ParcelNotInGeography(int geographyID)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID);

        var parcelNotInGeography = await AssemblySteps.QanatDbContext.Parcels
            .FirstOrDefaultAsync(p => p.GeographyID != geographyID && !p.WaterAccountID.HasValue);

        Assert.IsNotNull(waterAccount);
        Assert.IsNotNull(parcelNotInGeography, "If this is fails we need to modify the test to orphan a parcel :(");

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.AddOrphanedParcelToWaterAccount(waterAccount.WaterAccountID, parcelNotInGeography.ParcelID));
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, null);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsFalse(result.IsSuccessStatusCode, resultAsString);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanAddOrphanedParcelToWaterAccount_BadRequest_ParcelAlreadyAssociated(int geographyID)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID);

        var parcelAlreadyAssociated = await AssemblySteps.QanatDbContext.Parcels
            .Include(x => x.WaterAccount)
            .FirstOrDefaultAsync(p => p.GeographyID == geographyID && p.WaterAccountID == waterAccount.WaterAccountID);

        var reset = false;
        if (parcelAlreadyAssociated == null)
        {
            parcelAlreadyAssociated = await AssemblySteps.QanatDbContext.Parcels
                .FirstOrDefaultAsync(p => p.GeographyID == geographyID && !p.WaterAccountID.HasValue);

            parcelAlreadyAssociated.WaterAccountID = waterAccount.WaterAccountID;
            parcelAlreadyAssociated.ParcelStatusID = ParcelStatus.Assigned.ParcelStatusID;

            var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, DateTime.UtcNow.Year);

            var waterAccountParcel = new EFModels.Entities.WaterAccountParcel
            {
                GeographyID = geographyID,
                WaterAccountID = waterAccount.WaterAccountID,
                ParcelID = parcelAlreadyAssociated.ParcelID,
                ReportingPeriodID = reportingPeriod.ReportingPeriodID
            };

            AssemblySteps.QanatDbContext.WaterAccountParcels.Add(waterAccountParcel);

            await AssemblySteps.QanatDbContext.SaveChangesAsync();
            reset = true;
        }

        Assert.IsNotNull(waterAccount);
        Assert.IsNotNull(parcelAlreadyAssociated);

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.AddOrphanedParcelToWaterAccount(waterAccount.WaterAccountID, parcelAlreadyAssociated.ParcelID));
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, null);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsFalse(result.IsSuccessStatusCode, resultAsString);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

        if (reset)
        {
            parcelAlreadyAssociated.WaterAccountID = null;
            parcelAlreadyAssociated.ParcelStatusID = ParcelStatus.Unassigned.ParcelStatusID;

            var waterAccountParcelToRemove = await AssemblySteps.QanatDbContext.WaterAccountParcels
                .FirstOrDefaultAsync(wap => wap.WaterAccountID == waterAccount.WaterAccountID && wap.ParcelID == parcelAlreadyAssociated.ParcelID);

            AssemblySteps.QanatDbContext.WaterAccountParcels.Remove(waterAccountParcelToRemove);
            await AssemblySteps.QanatDbContext.SaveChangesAsync();
        }
    }

    [DataRow(5)] //Demo
    [TestMethod]
    public async Task AdminCanGetParcelHistoriesByWaterAccount(int geographyID)
    {
        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.Parcel).ThenInclude(x => x.ParcelHistories)
            .FirstOrDefaultAsync(wa => wa.GeographyID == geographyID && wa.WaterAccountParcels.Any());

        Assert.IsNotNull(waterAccount);

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByWaterAccountController>(c => c.GetWaterAccountParcelHistory(waterAccount.WaterAccountID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultAsString);
        Console.WriteLine(resultAsString);

        var resultAsDtos = JsonSerializer.Deserialize<List<ParcelHistoryDto>>(resultAsString, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.IsNotNull(resultAsDtos);
        Assert.IsTrue(resultAsDtos.Count > 0);
    }
}