using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers.WaterMeasurement;

[TestClass]
public class WaterMeasurementControllerTests
{
    [DataRow(1, 2023)] //MIUGSA, 2023 has a lot less data than 2024 for MIUGSA
    [DataRow(2, 2024)] //Pajaro
    [DataRow(3, 2024)] //RRB
    [DataRow(4, 2025)] //Yolo
    [DataRow(5, 2024)] //Demo
    [DataRow(6, 2024)] //MSGSA
    [DataRow(7, 2024)] //ETSGSA
    [TestMethod]
    public async Task AdminCanDownloadExcelWorkbookForGeographyAndYear(int geographyID, int year)
    {
        var route = RouteHelper.GetRouteFor<WaterMeasurementController>(c => c.DownloadExcelWorkbookForGeographyAndYear(geographyID, year));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var contentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, contentAsString);
    }

    [DataRow(5, 38, 19, new[]{1,2,3,4,5,6,7,8,9,10,11,12})]
    [DataRow(5, 38, 19, new[]{1})]
    [TestMethod]
    public async Task AdminCanListByGeographyIDReportingPeriodIDWaterMeasurementTypeIDAndMonths(int geographyID, int reportingPeriodID, int waterMeasurementTypeID, int[] months)
    {
        var route = RouteHelper.GetRouteFor<WaterMeasurementController>(c => c.ListByGeographyIDReportingPeriodIDWaterMeasurementTypeIDAndMonths(geographyID, reportingPeriodID, waterMeasurementTypeID, months.ToList()));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var contentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, contentAsString);
        Console.WriteLine(contentAsString);

        var waterMeasurementQualityAssuranceDtos = await result.Content.ReadFromJsonAsync<List<WaterMeasurementQualityAssuranceDto>>(AssemblySteps.DefaultJsonSerializerOptions);
        Assert.IsNotNull(waterMeasurementQualityAssuranceDtos, "Response deserialized to null.");
        Assert.IsTrue(waterMeasurementQualityAssuranceDtos.Count >= 1, "Expected at least one result.");
        Assert.IsTrue(waterMeasurementQualityAssuranceDtos.Where(x => x.PercentileBucket.HasValue).All(x => x.PercentileBucket is >= 0 and <= 5), "All percentile buckets with values should be between 0 and 5.");
        var distinctCount = waterMeasurementQualityAssuranceDtos.Select(x => x.UsageLocationID).Distinct().Count();
        Assert.AreEqual(distinctCount, waterMeasurementQualityAssuranceDtos.Count, "UsageLocationIDs are not unique. Grouping failed?");
    }

    [DataRow(6, 52, 2024, 1, .75)] //MSGSA, Eligible Inches of Rain, Year, Month, ValueInAcreFeetPerAcre
    [TestMethod]
    public async Task AdminCanBulkSetWaterMeasurements(int geographyID, int waterMeasurementTypeID, int year, int month, double valueInAcreFeetPerAcre)
    {
        var bulkSetDto = new WaterMeasurementBulkSetDto()
        {
            WaterMeasurementTypeID = waterMeasurementTypeID,
            Year = year,
            Month = month,
            ValueInAcreFeetPerAcre = (decimal) valueInAcreFeetPerAcre
        };

        var route = RouteHelper.GetRouteFor<WaterMeasurementController>(c => c.BulkSetWaterMeasurements(geographyID, bulkSetDto));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, bulkSetDto, AssemblySteps.DefaultJsonSerializerOptions);
        var contentAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, contentAsString);
    }

    [DataRow(6)]
    [TestMethod]
    public async Task BulkSetWaterMeasurements_BadRequest_MissingRequiredFields(int geographyID)
    {
        var bulkSetDto = new WaterMeasurementBulkSetDto();
        var route = RouteHelper.GetRouteFor<WaterMeasurementController>(c => c.BulkSetWaterMeasurements(geographyID, bulkSetDto));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, bulkSetDto, AssemblySteps.DefaultJsonSerializerOptions);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [DataRow(6, -1, 2024, 1, .75, "Could not find a Water Measurement Type with the ID -1.")] //MSGSA
    [DataRow(6, 20, 2024, 1, .75, "Water Measurement Type Consumed Groundwater is not user editable.")] //MSGSA
    [DataRow(6, 52, 1900, 1, .75, "Year 1900 is not a valid year for the selected geography.")] //MSGSA
    [DataRow(6, 52, 2024, -1, .75, "Month -1 is not a valid month.")] //MSGSA
    [TestMethod]
    public async Task BulkSetWaterMeasurements_BadRequest_FailedValidation(int geographyID, int waterMeasurementTypeID, int year, int month, double valueInAcreFeetPerAcre, string errorMessageExpected)
    {
        var bulkSetDto = new WaterMeasurementBulkSetDto()
        {
            WaterMeasurementTypeID = waterMeasurementTypeID,
            Year = year,
            Month = month,
            ValueInAcreFeetPerAcre = (decimal)valueInAcreFeetPerAcre
        };

        var route = RouteHelper.GetRouteFor<WaterMeasurementController>(c => c.BulkSetWaterMeasurements(geographyID, bulkSetDto));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, bulkSetDto, AssemblySteps.DefaultJsonSerializerOptions);
        var contentAsString = await result.Content.ReadAsStringAsync();

        Console.WriteLine(contentAsString);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.IsTrue(contentAsString.Contains(errorMessageExpected));
    }
}
