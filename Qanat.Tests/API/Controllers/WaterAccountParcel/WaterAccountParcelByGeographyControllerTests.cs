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

namespace Qanat.Tests.API.Controllers.WaterAccountParcel;

[TestClass]
public class WaterAccountParcelByGeographyControllerTests
{
    [DataRow(5, 2024)] //Demo
    [TestMethod]
    public async Task AdminCanCopyFromReportingPeriod(int geographyID, int yearToCopyFrom)
    {
        var fromReportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, yearToCopyFrom);
        Assert.IsNotNull(fromReportingPeriod);

        var expectedWaterAccountParcels = await AssemblySteps.QanatDbContext.WaterAccountParcels
            .Where(x => x.ReportingPeriodID == fromReportingPeriod.ReportingPeriodID && x.Parcel.GeographyID == geographyID)
            .ToListAsync();

        //Add a new reporting period to the geography so we don't wipe WaterMeasurements
        var reportingPeriodUpsertDto = new ReportingPeriodUpsertDto()
        {
            Name = "1970",
            StartDate = new DateTime(1970, 1, 1),
            IsDefault = false,
            ReadyForAccountHolders = false
        };

        var callingUser = Users.GetByUserID(AssemblySteps.QanatDbContext, Users.QanatSystemAdminUserID);
        var toReportingPeriod = await ReportingPeriods.CreateAsync(AssemblySteps.QanatDbContext, geographyID, reportingPeriodUpsertDto, callingUser);
        Assert.IsNotNull(toReportingPeriod);
            
        //Add a new water account to the new reporting period to ensure we can handle that and write a history out.

        var waterAccount = await AssemblySteps.QanatDbContext.WaterAccounts
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID);
        Assert.IsNotNull(waterAccount);

        var parcel = await AssemblySteps.QanatDbContext.Parcels
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID);
        Assert.IsNotNull(parcel);

        var waterAccountParcel = new EFModels.Entities.WaterAccountParcel()
        {
            GeographyID = geographyID,
            ReportingPeriodID = toReportingPeriod.ReportingPeriodID,
            WaterAccountID = waterAccount.WaterAccountID,
            ParcelID = parcel.ParcelID
        };

        await AssemblySteps.QanatDbContext.WaterAccountParcels.AddAsync(waterAccountParcel);
        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByGeographyController>(x => x.CopyFromReportingPeriod(geographyID, toReportingPeriod.ReportingPeriodID, null));
        var copyDto = new CopyWaterAccountParcelsFromReportingPeriodDto()
        {
            FromReportingPeriodID = fromReportingPeriod.ReportingPeriodID,
        };

        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, copyDto, AssemblySteps.DefaultJsonSerializerOptions);
        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, resultContentAsString);
        Console.WriteLine(resultContentAsString);

        var resultAsDtos = await result.DeserializeContentAsync<List<WaterAccountParcelSimpleDto>>();
        Assert.IsNotNull(resultAsDtos);
        Assert.AreEqual(expectedWaterAccountParcels.Count, resultAsDtos.Count);

        //Check that we see all water account parcels for the reporting period
        foreach (var wap in expectedWaterAccountParcels)
        {
            var dto = resultAsDtos.FirstOrDefault(x => x.WaterAccountID == wap.WaterAccountID && x.ParcelID == wap.ParcelID);
            Assert.IsNotNull(dto);
        }

        //Clean up the new reporting period
        var removedWaterAccountParcelsCount = await AssemblySteps.QanatDbContext.WaterAccountParcels
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == toReportingPeriod.ReportingPeriodID)
            .ExecuteDeleteAsync();
        Assert.AreEqual(resultAsDtos.Count, removedWaterAccountParcelsCount);

        var removedParcelWaterAccountHistoriesCount = await AssemblySteps.QanatDbContext.ParcelWaterAccountHistories
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == toReportingPeriod.ReportingPeriodID)
            .ExecuteDeleteAsync();
        Assert.AreEqual(resultAsDtos.Count, removedParcelWaterAccountHistoriesCount);

        var removedReportingPeriodCount = await AssemblySteps.QanatDbContext.ReportingPeriods
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == toReportingPeriod.ReportingPeriodID)
            .ExecuteDeleteAsync();
        Assert.AreEqual(1, removedReportingPeriodCount);
    }

    [DataRow(5, 2024)] //Demo
    [TestMethod]
    public async Task CopyFromReportingPeriod_BadRequest_FromReportingPeriodNotValid(int geographyID, int yearToCopyFrom)
    {
        var toReportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, yearToCopyFrom);
        Assert.IsNotNull(toReportingPeriod);

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByGeographyController>(x => x.CopyFromReportingPeriod(geographyID, toReportingPeriod.ReportingPeriodID, null));
        var copyDto = new CopyWaterAccountParcelsFromReportingPeriodDto()
        {
            FromReportingPeriodID = -1,
        };

        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, copyDto, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }


    [DataRow(5, 2024)] //Demo
    [TestMethod]
    public async Task CopyFromReportingPeriod_BadRequest_CopyingFromToSameReportingPeriod(int geographyID, int yearToCopyFrom)
    {
        var toReportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(AssemblySteps.QanatDbContext, geographyID, yearToCopyFrom);
        Assert.IsNotNull(toReportingPeriod);

        var route = RouteHelper.GetRouteFor<WaterAccountParcelByGeographyController>(x => x.CopyFromReportingPeriod(geographyID, toReportingPeriod.ReportingPeriodID, null));
        var copyDto = new CopyWaterAccountParcelsFromReportingPeriodDto()
        {
            FromReportingPeriodID = toReportingPeriod.ReportingPeriodID,
        };

        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, copyDto, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }
}