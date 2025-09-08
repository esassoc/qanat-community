using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Controllers;
using Qanat.Models.DataTransferObjects;
using Qanat.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.API.Controllers.UsageLocationType;

[TestClass]
public class UsageLocationTypeControllerTests
{
    #region Create

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanCreateUsageLocationType(int geographyID)
    {
        var newName = $"Test-{Guid.NewGuid()}";

        var existingSortOrders = await AssemblySteps.QanatDbContext.UsageLocationTypes
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.SortOrder)
            .ToListAsync();

        var nextSortOrder = existingSortOrders.Any() ? existingSortOrders.Max() + 1 : 1;

        var newUsageLocationType = new UsageLocationTypeUpsertDto
        {
            Name = newName,
            Definition = "This is a test usage location type.",
            CanBeRemoteSensed = true,
            IsIncludedInUsageCalculation = true,
            IsDefault = false,
            ColorHex = "#00FF00",
            SortOrder = nextSortOrder
        };

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Create(geographyID, newUsageLocationType));
        var json = JsonSerializer.Serialize(newUsageLocationType);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);
        var resultContentAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var usageLocationTypes = await result.DeserializeIfSuccessAsync<List<UsageLocationTypeDto>>();
        Assert.IsNotNull(usageLocationTypes);

        var created = usageLocationTypes.FirstOrDefault(x => x.Name == newName);
        Assert.IsNotNull(created, "Expected usage location type to be created but it was not found.");

        // Cleanup
        await AssemblySteps.QanatDbContext.UsageLocationTypes
            .Where(x => x.UsageLocationTypeID == created.UsageLocationTypeID)
            .ExecuteDeleteAsync();
    }

    [DataRow(5, "The name '{0}' is already in use.")]
    [TestMethod]
    public async Task AdminCannotCreateUsageLocationType_WithDuplicateName(int geographyID, string expectedErrorMessage)
    {
        var existing = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .FirstAsync(x => x.GeographyID == geographyID);

        var newUsageLocationType = new UsageLocationTypeUpsertDto
        {
            Name = existing.Name, // Duplicate
            Definition = "Some definition",
            CanBeRemoteSensed = true,
            IsIncludedInUsageCalculation = true,
            IsDefault = false,
            ColorHex = "#ABCDEF",
            SortOrder = 999
        };

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Create(geographyID, newUsageLocationType));
        var json = JsonSerializer.Serialize(newUsageLocationType);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        var expectedErrorMessageFormatted = string.Format(expectedErrorMessage, existing.Name);
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessageFormatted), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    [DataRow(5, "The sort order '{0}' is already in use.")]
    [TestMethod]
    public async Task AdminCannotCreateUsageLocationType_WithDuplicateSortOrder(int geographyID, string expectedErrorMessage)
    {
        var existing = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .FirstAsync(x => x.GeographyID == geographyID);

        var newUsageLocationType = new UsageLocationTypeUpsertDto
        {
            Name = Guid.NewGuid().ToString(),
            Definition = "Some definition",
            CanBeRemoteSensed = false,
            IsIncludedInUsageCalculation = true,
            IsDefault = false,
            ColorHex = "#123456",
            SortOrder = existing.SortOrder // Duplicate
        };

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Create(geographyID, newUsageLocationType));
        var json = JsonSerializer.Serialize(newUsageLocationType);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        var expectedErrorMessageFormatted = string.Format(expectedErrorMessage, existing.SortOrder);
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessageFormatted), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    [DataRow(5, "Only one usage location type can be marked as default.")]
    [TestMethod]
    public async Task AdminCannotCreateUsageLocationType_WithDuplicateDefault(int geographyID, string expectedErrorMessage)
    {
        var existingDefault = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.IsDefault);

        Assert.IsNotNull(existingDefault, "No default usage location type found.");

        var newUsageLocationType = new UsageLocationTypeUpsertDto
        {
            Name = Guid.NewGuid().ToString(),
            Definition = "Should fail due to duplicate default",
            CanBeRemoteSensed = false,
            IsIncludedInUsageCalculation = false,
            IsDefault = true,
            ColorHex = "#DEADBF",
            SortOrder = 999
        };

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Create(geographyID, newUsageLocationType));
        var json = JsonSerializer.Serialize(newUsageLocationType);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessage), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    #endregion

    #region Read

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanListUsageLocationTypes(int geographyID)
    {
        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.List(geographyID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);
        var resultContentAsString = await result.Content.ReadAsStringAsync();

        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var usageLocationTypeDtos = await result.DeserializeIfSuccessAsync<List<UsageLocationTypeDto>>();
        Assert.IsNotNull(usageLocationTypeDtos);
    }

    #endregion

    #region Update

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanUpdateUsageLocationTypes(int geographyID)
    {
        var existingUsageLocationTypes = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var usageLocationTypeUpsertDtos = existingUsageLocationTypes.Select(x => new UsageLocationTypeUpsertDto()
        {
            UsageLocationTypeID = x.UsageLocationTypeID,
            Name = x.Name,
            Definition = Guid.NewGuid().ToString(),
            CanBeRemoteSensed = x.CanBeRemoteSensed,
            IsIncludedInUsageCalculation = x.IsIncludedInUsageCalculation,
            IsDefault = x.IsDefault,
            ColorHex = x.ColorHex,
            SortOrder = x.SortOrder,
        }).ToList();

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Update(geographyID, usageLocationTypeUpsertDtos));

        var json = JsonSerializer.Serialize(usageLocationTypeUpsertDtos);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, $"\nRoute: {route}\n{resultContentAsString}");
        Console.WriteLine(resultContentAsString);

        var updatedUsageLocationTypeDtos = await result.DeserializeIfSuccessAsync<List<UsageLocationTypeDto>>();
        Assert.IsNotNull(updatedUsageLocationTypeDtos);

        // Check that the existing usage location types were updated
        foreach (var existingType in existingUsageLocationTypes)
        {
            var updatedTo = usageLocationTypeUpsertDtos.FirstOrDefault(x => x.UsageLocationTypeID == existingType.UsageLocationTypeID);
            Assert.IsNotNull(updatedTo);
            var updatedType = updatedUsageLocationTypeDtos.FirstOrDefault(x => x.UsageLocationTypeID == existingType.UsageLocationTypeID);

            Assert.IsNotNull(updatedType, $"Existing usage location type with ID {existingType.UsageLocationTypeID} was not updated successfully.");
            Assert.AreEqual(updatedTo.Name, updatedType.Name, "Name did not match after update.");
            Assert.AreEqual(updatedTo.Definition, updatedType.Definition, "Definition did not match after update.");
            Assert.AreEqual(updatedTo.CanBeRemoteSensed, updatedType.CanBeRemoteSensed, "CanBeRemoteSensed did not match after update.");
            Assert.AreEqual(updatedTo.IsIncludedInUsageCalculation, updatedType.IsIncludedInUsageCalculation, "IsIncludedInUsageCalculation did not match after update.");
            Assert.AreEqual(updatedTo.IsDefault, updatedType.IsDefault, "IsDefault did not match after update.");
            Assert.AreEqual(updatedTo.ColorHex, updatedType.ColorHex, "ColorHex did not match after update.");
            Assert.AreEqual(updatedTo.SortOrder, updatedType.SortOrder, "SortOrder did not match after update.");
        }
    }

    [DataRow(5, "The following Usage Location Types are missing from the update:")]
    [TestMethod]
    public async Task AdminCanUpdateUsageLocationTypes_BadRequest_MissingUsageLocationType(int geographyID, string expectedErrorMessage)
    {
        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Update(geographyID, new List<UsageLocationTypeUpsertDto>()));
        var json = JsonSerializer.Serialize(new List<UsageLocationTypeUpsertDto>());
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessage), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    [DataRow(5, "Provided duplicate names:")]
    [TestMethod]
    public async Task AdminCanUpdateUsageLocationTypes_BadRequest_DuplicateNames(int geographyID, string expectedErrorMessage)
    {
        var existingUsageLocationTypes = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var usageLocationTypeUpsertDtos = existingUsageLocationTypes.Select(x => new UsageLocationTypeUpsertDto()
        {
            UsageLocationTypeID = x.UsageLocationTypeID,
            Name = x.Name,
            Definition = Guid.NewGuid().ToString(),
            CanBeRemoteSensed = x.CanBeRemoteSensed,
            IsIncludedInUsageCalculation = x.IsIncludedInUsageCalculation,
            IsDefault = x.IsDefault,
            ColorHex = x.ColorHex,
            SortOrder = x.SortOrder,
        }).ToList();

        usageLocationTypeUpsertDtos.Add(new UsageLocationTypeUpsertDto()
        {
            Name = usageLocationTypeUpsertDtos.First().Name, // Duplicate name
            Definition = "New definition",
            CanBeRemoteSensed = true,
            IsIncludedInUsageCalculation = true,
            IsDefault = false,
            ColorHex = "#FF5733",
            SortOrder = usageLocationTypeUpsertDtos.Count + 1,
        });

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Update(geographyID, usageLocationTypeUpsertDtos));
        var json = JsonSerializer.Serialize(usageLocationTypeUpsertDtos);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessage), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    [DataRow(5, "Provided duplicate sort orders:")]
    [TestMethod]
    public async Task AdminCanUpdateUsageLocationTypes_BadRequest_DuplicateSortOrders(int geographyID, string expectedErrorMessage)
    {
        var existingUsageLocationTypes = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var usageLocationTypeUpsertDtos = existingUsageLocationTypes.Select(x => new UsageLocationTypeUpsertDto()
        {
            UsageLocationTypeID = x.UsageLocationTypeID,
            Name = x.Name,
            Definition = Guid.NewGuid().ToString(),
            CanBeRemoteSensed = x.CanBeRemoteSensed,
            IsIncludedInUsageCalculation = x.IsIncludedInUsageCalculation,
            IsDefault = x.IsDefault,
            ColorHex = x.ColorHex,
            SortOrder = x.SortOrder,
        }).ToList();

        usageLocationTypeUpsertDtos.Add(new UsageLocationTypeUpsertDto()
        {
            Name = Guid.NewGuid().ToString(),
            Definition = "New definition",
            CanBeRemoteSensed = true,
            IsIncludedInUsageCalculation = true,
            IsDefault = false,
            ColorHex = "#FF5733",
            SortOrder = usageLocationTypeUpsertDtos.First().SortOrder, // Duplicate sort order
        });

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Update(geographyID, usageLocationTypeUpsertDtos));
        var json = JsonSerializer.Serialize(usageLocationTypeUpsertDtos);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessage), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    [DataRow(5, "Sort orders must be sequential starting from 1.")]
    [TestMethod]
    public async Task AdminCanUpdateUsageLocationTypes_BadRequest_NonSequentialSortOrders(int geographyID, string expectedErrorMessage)
    {
        var existingUsageLocationTypes = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var usageLocationTypeUpsertDtos = existingUsageLocationTypes.Select(x => new UsageLocationTypeUpsertDto()
        {
            UsageLocationTypeID = x.UsageLocationTypeID,
            Name = x.Name,
            Definition = Guid.NewGuid().ToString(),
            CanBeRemoteSensed = x.CanBeRemoteSensed,
            IsIncludedInUsageCalculation = x.IsIncludedInUsageCalculation,
            IsDefault = x.IsDefault,
            ColorHex = x.ColorHex,
            SortOrder = x.SortOrder,
        }).ToList();

        usageLocationTypeUpsertDtos.Add(new UsageLocationTypeUpsertDto()
        {
            Name = Guid.NewGuid().ToString(),
            Definition = "New definition",
            CanBeRemoteSensed = true,
            IsIncludedInUsageCalculation = true,
            IsDefault = false,
            ColorHex = "#FF5733",
            SortOrder = 10, // Non-sequential sort order
        });

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Update(geographyID, usageLocationTypeUpsertDtos));
        var json = JsonSerializer.Serialize(usageLocationTypeUpsertDtos);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessage), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    [DataRow(5, "Only one usage location type can be marked as default.")]
    [TestMethod]
    public async Task AdminCanUpdateUsageLocationTypes_BadRequest_MultipleDefaultUsageLocationTypes(int geographyID, string expectedErrorMessage)
    {
        var existingUsageLocationTypes = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var usageLocationTypeUpsertDtos = existingUsageLocationTypes.Select(x => new UsageLocationTypeUpsertDto()
        {
            UsageLocationTypeID = x.UsageLocationTypeID,
            Name = x.Name,
            Definition = Guid.NewGuid().ToString(),
            CanBeRemoteSensed = x.CanBeRemoteSensed,
            IsIncludedInUsageCalculation = x.IsIncludedInUsageCalculation,
            IsDefault = x.IsDefault,
            ColorHex = x.ColorHex,
            SortOrder = x.SortOrder,
        }).ToList();

        usageLocationTypeUpsertDtos.Add(new UsageLocationTypeUpsertDto()
        {
            Name = Guid.NewGuid().ToString(),
            Definition = "New definition",
            CanBeRemoteSensed = true,
            IsIncludedInUsageCalculation = true,
            IsDefault = true, // Assumes there is an existing type with default set already.
            ColorHex = "#FF5733",
            SortOrder = usageLocationTypeUpsertDtos.Count + 1,
        });

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Update(geographyID, usageLocationTypeUpsertDtos));
        var json = JsonSerializer.Serialize(usageLocationTypeUpsertDtos);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessage), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    [DataRow(5, "The following Usage Location Types could not be found to update:")]
    [TestMethod]
    public async Task AdminCanUpdateUsageLocationTypes_BadRequest_UnknownUsageLocationTypes(int geographyID, string expectedErrorMessage)
    {
        var usageLocationTypeUpsertDtos = new List<UsageLocationTypeUpsertDto>
        {
            new()
            {
                UsageLocationTypeID = -1, // Assuming this ID does not exist
                Name = "Unknown Type",
                Definition = "Definition for unknown type",
                CanBeRemoteSensed = true,
                IsIncludedInUsageCalculation = true,
                IsDefault = false,
                ColorHex = "#FF5733",
                SortOrder = 1
            }
        };

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Update(geographyID, usageLocationTypeUpsertDtos));
        var json = JsonSerializer.Serialize(usageLocationTypeUpsertDtos);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PutAsync(route, content);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessage), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    #endregion

    #region Delete

    [DataRow(5)]
    [TestMethod]
    public async Task AdminCanDeleteUsageLocationType(int geographyID)
    {
        // First, create a new usage location type to delete
        var existingUsageLocationTypes = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var newUsageLocationType = new UsageLocationTypeUpsertDto()
        {
            Name = Guid.NewGuid().ToString(),
            Definition = "New definition",
            CanBeRemoteSensed = true,
            IsIncludedInUsageCalculation = true,
            IsDefault = false,
            ColorHex = "#FF5733",
            SortOrder = existingUsageLocationTypes.Count + 1,
        };

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Create(geographyID, newUsageLocationType));
        var json = JsonSerializer.Serialize(newUsageLocationType);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, content);

        var resultAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(result.IsSuccessStatusCode, $"Failed to create usage location type: {resultAsString}");

        // Now delete the created usage location type
        var createdUsageLocationTypeDtos = await result.DeserializeIfSuccessAsync<List<UsageLocationTypeDto>>();
        var usageLocationTypeToDelete = createdUsageLocationTypeDtos.FirstOrDefault(x => x.Name == newUsageLocationType.Name);

        Assert.IsNotNull(usageLocationTypeToDelete, "Created usage location type was not found.");

        var deleteRoute = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Delete(geographyID, usageLocationTypeToDelete.UsageLocationTypeID));
        var deleteResult = await AssemblySteps.AdminHttpClient.DeleteAsync(deleteRoute);
        var deleteResultAsString = await deleteResult.Content.ReadAsStringAsync();
        Assert.IsTrue(deleteResult.IsSuccessStatusCode, $"Failed to delete usage location type: {deleteResultAsString}");
    }

    [DataRow(5, "Cannot delete a Usage Location Type that is marked as default.")]
    [TestMethod]
    public async Task AdminCanDeleteUsageLocationType_BadRequest_DefaultUsageLocationType(int geographyID, string expectedErrorMessage)
    {
        // Attempt to delete a default usage location type
        var defaultUsageLocationType = await AssemblySteps.QanatDbContext.UsageLocationTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.IsDefault);

        Assert.IsNotNull(defaultUsageLocationType, "No default usage location type found for the geography.");

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Delete(geographyID, defaultUsageLocationType.UsageLocationTypeID));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessage), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    [DataRow(5, "Cannot delete a Usage Location Type that is associated with existing Usage Locations.")]
    [TestMethod]
    public async Task AdminCanDeleteUsageLocationType_BadRequest_UsageLocationsExist(int geographyID, string expectedErrorMessage)
    {
        // Attempt to delete a usage location type that has associated usage locations
        var usageLocationTypeWithLocations = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .Include(x => x.UsageLocations)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageLocations.Any());

        Assert.IsNotNull(usageLocationTypeWithLocations, "No usage location type with associated locations found for the geography.");

        var route = RouteHelper.GetRouteFor<UsageLocationTypeController>(x => x.Delete(geographyID, usageLocationTypeWithLocations.UsageLocationTypeID));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, route);

        var resultContentAsString = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(resultContentAsString.Contains(expectedErrorMessage), $"Expected error message to contain '{expectedErrorMessage}'.");
        Console.WriteLine(resultContentAsString);
    }

    #endregion
}
