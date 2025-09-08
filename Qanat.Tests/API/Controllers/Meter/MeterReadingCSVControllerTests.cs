using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using Qanat.Tests.Helpers.EntityHelpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Qanat.EFModels.Entities;

namespace Qanat.Tests.API.Controllers.Meter;

[TestClass]
public class MeterReadingCSVControllerTests
{
    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanDownloadMeterReadingCSVTemplate(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.DownloadCSVTemplate(geographyID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, await result.Content.ReadAsStringAsync());
        Assert.AreEqual("text/csv", result.Content.Headers.ContentType?.MediaType);

        var contentAsString = await result.Content.ReadAsStringAsync();

        Console.WriteLine(contentAsString);
        var header = string.Join(",", MeterReadingUpsertDtoCSVMap.Headers);
        Assert.AreEqual(header.Trim(), contentAsString.Trim());
    }

    [DataRow(5, "{0},2025-03-01,14:00,MK,0,100000,gallons,Comment", 1)]
    [DataRow(5, "{0},2025-01-02,22:45,MK,0,20.416,acre-feet,Comment\n{0},2025-02-20,17:37,MK,20.416,38.907,acre-feet,Comment\n{0},2025-03-19,19:08,MK,38.907,39.87,acre-feet,Comment", 3)]
    [DataRow(5, "{0},2025-03-01,14:00,MK,0,100000,gallons,Comment\n,,,,,, ,", 1)] //Excel is notorious for putting in blank rows, make sur this still works
    [DataRow(5, "{0},2025-03-01,14:00,MK,0,100000,AF,Comment", 1)] // test alternative unit type display name
    [DataRow(5, "{0},2025-03-01,14:00,MK,0,100000,Acre-Feet,Comment", 1)] //test case sensitivity for unit type
    [DataRow(5, "{0},2025-03-01,4:00,MK,0,100000,gallons,Comment", 1)] //test H:MM time format
    [TestMethod]
    public async Task AdminCanUploadCSV(int geographyID, string csv, int expectedCount)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!));

        var headers = string.Join(",", MeterReadingUpsertDtoCSVMap.Headers);
        var replacedSerialNumber = string.Format(csv, meter.SerialNumber);
        var csvContent = $"{headers}\n{replacedSerialNumber}";
        using var form = CreateCSVFormData(csvContent, "valid.csv");

        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, form);
        var contentAsString = await result.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, contentAsString);
        Console.WriteLine(contentAsString);

        var meterReadings = await AssemblySteps.QanatDbContext.MeterReadings
            .Where(m => m.GeographyID == geographyID && m.WellID == well.WellID && m.MeterID == meter.MeterID)
            .Include(meterReading => meterReading.Meter)
            .ToListAsync();

        Assert.AreEqual(expectedCount, meterReadings.Count);
        Assert.AreEqual(geographyID, meterReadings[0].GeographyID);
        Assert.AreEqual(well.WellID, meterReadings[0].WellID);
        Assert.AreEqual(meter.MeterID, meterReadings[0].MeterID);
        Assert.AreEqual(meter.SerialNumber, meterReadings[0].Meter.SerialNumber);
        Assert.AreEqual("MK", meterReadings[0].ReaderInitials);
        Assert.AreEqual("Comment", meterReadings[0].Comment);
    }

    [DataRow(5)]
    [TestMethod]
    public async Task UploadCSV_BadRequest_EmptyFile(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!));

        using var form = CreateCSVFormData(string.Empty, "empty.csv");

        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, form);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

        var contentAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(contentAsString);
        Assert.IsTrue(contentAsString.Contains("No file uploaded."));
    }

    [DataRow(5)]
    [TestMethod]
    public async Task UploadCSV_BadRequest_InvalidContentType(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!));

        var csvContent = "Not,a,real,CSV";
        using var form = CreateCSVFormData(csvContent, "invalid.csv", "application/json");

        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, form);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

        var contentAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(contentAsString);
        Assert.IsTrue(contentAsString.Contains("Invalid file type."));
    }

    #region Unhappy Path for ParseMeterReadingsFromCSVAsync 

    [DataRow(5)]
    [TestMethod]
    public async Task UploadCSV_BadRequest_DuplicatedHeaders(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!));

        var duplicatedHeader = string.Join(",", MeterReadingUpsertDtoCSVMap.Headers.Take(1)) + "," +
                               string.Join(",", MeterReadingUpsertDtoCSVMap.Headers);

        var csvContent = $"{duplicatedHeader}\n123ABC,2024-03-01,14:00,MK,100,120,gallons,Comment";

        using var form = CreateCSVFormData(csvContent, "duplicated.csv");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, form);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        var contentAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(contentAsString);
        Assert.IsTrue(contentAsString.Contains("The following headers are duplicated:"));
    }

    [DataRow(5)]
    [TestMethod]
    public async Task UploadCSV_BadRequest_MissingFieldInRow(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!));

        var headers = string.Join(",", MeterReadingUpsertDtoCSVMap.Headers);
        var csvContent = $"{headers}\n123ABC,2024-03-01,14:00,MK,100"; // too few columns

        using var form = CreateCSVFormData(csvContent, "missingfield.csv");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, form);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        var contentAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(contentAsString);
        Assert.IsTrue(contentAsString.Contains("column name is not missing or misspelled")); // same message
    }

    [DataRow(5)]
    [TestMethod]
    public async Task UploadCSV_BadRequest_MissingHeaders(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!));

        // Missing headers or invalid column names
        var invalidCsv = "Bogus,Header,Names\n1,2,3\n";
        using var form = CreateCSVFormData(invalidCsv, "typeerror.csv");

        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, form);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

        var contentAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(contentAsString);
        Assert.IsTrue(contentAsString.Contains("Please check that the column name is not missing or misspelled"));
    }

    [DataRow(5)]
    [TestMethod]
    public async Task UploadCSV_BadRequest_TypeConversionError(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!));

        var headers = string.Join(",", MeterReadingUpsertDtoCSVMap.Headers);
        var csvContent = $"{headers}\n123ABC,2024-03-01,14:00,MK,not_a_number,120,gallons,Comment";

        using var form = CreateCSVFormData(csvContent, "typeerror.csv");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, form);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        var contentAsString = await result.Content.ReadAsStringAsync();
        Console.WriteLine(contentAsString);
        Assert.IsTrue(contentAsString.Contains("data types in the CSV match the expected types"));
    }

    #endregion

    #region Unhappy Path for ValidateCSVRecordsAsync

    [DataRow(5, ",2024-03-01,14:00,MK,100,120,gallons,Comment", "Serial Number is required")]
    [DataRow(5, "DOESNOTEXIST,2024-03-01,14:00,MK,100,120,gallons,Comment", "Meter with Serial Number")]
    [DataRow(5, "123ABC,,14:00,MK,100,120,gallons,Comment", "Date is required")]
    [DataRow(5, "123ABC,2024-03-01,,MK,100,120,gallons,Comment", "Time is required")]
    [DataRow(5, "123ABC,2024-03-01,notatime,MK,100,120,gallons,Comment", "Time is not in the correct format")]
    [DataRow(5, "123ABC,2024-03-01,14:00,MK,,120,gallons,Comment", "Previous Reading is required")]
    [DataRow(5, "123ABC,2024-03-01,14:00,MK,100,,gallons,Comment", "Current Reading is required")]
    [DataRow(5, "123ABC,2024-03-01,14:00,MK,100,120,,Comment", "Unit Type is required")]
    [DataRow(5, "123ABC,2024-03-01,14:00,MK,100,120,FakeUnit,Comment", "Unit Type FakeUnit is not valid")]
    [DataRow(5, "123ABC,2024-03-01,14:00,MK,100,120,FakeUnit,Comment\n123ABC,2024-03-01,14:00,MK,100,120,FakeUnit,Comment", "There are multiple readings uploaded for ")]
    [DataRow(5, "123ABC,,14:00,MK,100,120,FakeUnit,Comment\n123ABC,,14:00,MK,100,120,FakeUnit,Comment", "There are multiple readings uploaded for ")] // Missing dates triggered an invalid server exception for this specific validation before
    [TestMethod]
    public async Task UploadCSV_BadRequest_ValidateCSVRecordsAsyncTests(int geographyID, string csv, string expectedErrorMessage)
    {
        var headers = string.Join(",", MeterReadingUpsertDtoCSVMap.Headers);
        var csvContent = $"{headers}\n{csv}";

        using var form = CreateCSVFormData(csvContent, "invalid_row.csv");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!)), form);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        var content = await result.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        Assert.IsTrue(content.Contains(expectedErrorMessage));
    }

    [DataRow(5)]
    [TestMethod]
    public async Task UploadCSV_BadRequest_NoActiveWellMeter(int geographyID)
    {
        await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);

        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!));
        var headers = string.Join(",", MeterReadingUpsertDtoCSVMap.Headers);
        var csvContent = $"{headers}\n{meter.SerialNumber},2024-03-01,14:00,MK,100,120,gallons,Comment";
        using var form = CreateCSVFormData(csvContent, "valid.csv");

        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, form);
        var contentAsString = await result.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, contentAsString);
        Console.WriteLine(contentAsString);

        Assert.IsTrue(contentAsString.Contains("is not currently assigned to any well."));
    }

    [DataRow(5)]
    [TestMethod]
    public async Task UploadCSV_BadRequest_DuplicateReadingDateInDatabase(int geographyID)
    {
        var well = await WellHelper.AddWellAsync(geographyID);
        var meter = await MeterHelper.AddMeterAsync(geographyID);
        await MeterHelper.AssociateMeterToWellAsync(well.WellID, meter.MeterID, DateTime.UnixEpoch);

        await AssemblySteps.QanatDbContext.MeterReadings.AddAsync(new MeterReading()
        {
            GeographyID = well.GeographyID,
            WellID = well.WellID,
            MeterID = meter.MeterID,
            MeterReadingUnitTypeID = MeterReadingUnitType.Gallons.MeterReadingUnitTypeID,
            ReadingDate = new DateTime(2024, 3, 1, 14, 0, 0),
            PreviousReading = 100,
            CurrentReading = 100,
            Volume = 0,
            VolumeInAcreFeet = 0,
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var route = RouteHelper.GetRouteFor<MeterReadingCSVController>(c => c.UploadCSV(geographyID, null!));
        var headers = string.Join(",", MeterReadingUpsertDtoCSVMap.Headers);
        var csvContent = $"{headers}\n{meter.SerialNumber},2024-03-01,14:00,MK,100,120,gallons,Comment";
        using var form = CreateCSVFormData(csvContent, "valid.csv");

        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, form);
        var contentAsString = await result.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, contentAsString);
        Console.WriteLine(contentAsString);

        Assert.IsTrue(contentAsString.Contains("There is already a meter reading for meter"));
    }

    #endregion

    private MultipartFormDataContent CreateCSVFormData(string csvContent, string fileName = "upload.csv", string contentType = "text/csv")
    {
        var form = new MultipartFormDataContent();
        var content = new ByteArrayContent(Encoding.UTF8.GetBytes(csvContent));
        content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        form.Add(content, "CSVFile", fileName);
        return form;
    }
}