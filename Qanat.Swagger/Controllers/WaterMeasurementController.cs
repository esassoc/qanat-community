using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qanat.EFModels.Entities;
using Qanat.Swagger.Entities;
using Qanat.Swagger.Filters;
using Qanat.Swagger.Models;

namespace Qanat.Swagger.Controllers;

[Authorize]
[ApiController]
[Tags("Water Measurements")]
public class WaterMeasurementController(QanatDbContext dbContext, ILogger<WaterMeasurementController> logger) : ControllerBase
{
    [EndpointSummary("List by Geography, Year, and Type")]
    [EndpointDescription("List all water measurements for a specified geography, year, and water measurement type")]
    [NoAccessBlock]
    [GeographyAccess]
    [ReportingPeriodAccess]
    [HttpGet("geographies/{geographyID}/years/{year}/water-measurement-types/{waterMeasurementTypeID}/water-measurements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<ActionResult<List<WaterMeasurementConsumerDto>>> ListWaterMeasurements([FromRoute] int geographyID, [FromRoute] int year, [FromRoute] int waterMeasurementTypeID )
    {
        var waterMeasurementType = dbContext.WaterMeasurementTypes.AsNoTracking()
            .SingleOrDefault(x => x.WaterMeasurementTypeID == waterMeasurementTypeID && x.GeographyID == geographyID);
        if (waterMeasurementType == null)
        {
            return NotFound($"Water measurement type with ID {waterMeasurementTypeID} does not exist within given geography!");
        }

        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        // ReportingPeriodAccess guarantees existence
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, year);

        List<WaterMeasurementConsumerDto> waterMeasurementConsumerDtos;
        if (UserPermissions.UserIsSystemAdmin(callingUser) || UserPermissions.UserIsGeographyManager(callingUser, geographyID))
        {
            waterMeasurementConsumerDtos = dbContext.WaterMeasurements.AsNoTracking()
                .Include(x => x.UsageLocation).ThenInclude(ul => ul.Parcel)
                .Where(x =>  x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID 
                                                          && x.ReportedDate >= reportingPeriod.StartDate && x.ReportedDate <= reportingPeriod.EndDate)
                .Select(x => new WaterMeasurementConsumerDto()
                {
                    WaterMeasurementID = x.WaterMeasurementID,
                    WaterMeasurementTypeID = x.WaterMeasurementType.WaterMeasurementTypeID,
                    WaterMeasurementTypeName = x.WaterMeasurementType.WaterMeasurementTypeName,
                    UsageLocationID = x.UsageLocationID,
                    UsageLocationName = x.UsageLocation.Name,
                    UsageLocationType = x.UsageLocation.UsageLocationType.Name,
                    ParcelID = x.UsageLocation.ParcelID,
                    ParcelNumber = x.UsageLocation.Parcel.ParcelNumber,
                    ReportingDate = x.ReportedDate,
                    ReportedValueInFeet = x.ReportedValueInFeet,
                    ReportedValueInAcreFeet = x.ReportedValueInAcreFeet
                }).ToList();
        }
        else
        {
            var associatedWaterAccountIDs = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, callingUser);

            var associatedUsageLocationIDs = dbContext.UsageLocations.AsNoTracking()
                .Include(x => x.Parcel)
                .Where(x => x.GeographyID == geographyID && x.Parcel.WaterAccountID.HasValue && associatedWaterAccountIDs.Contains(x.Parcel.WaterAccountID.Value))
                .Select(x => x.UsageLocationID).ToList();

            waterMeasurementConsumerDtos = dbContext.WaterMeasurements.AsNoTracking()
                .Include(x => x.UsageLocation).ThenInclude(ul => ul.Parcel)
                .Include(x => x.UsageLocation).ThenInclude(x => x.UsageLocationType)
                .Where(x => x.GeographyID == geographyID 
                            && associatedUsageLocationIDs.Contains(x.UsageLocationID)
                            && x.WaterMeasurementTypeID == waterMeasurementTypeID 
                            && x.ReportedDate >= reportingPeriod.StartDate && x.ReportedDate <= reportingPeriod.EndDate)
                .Select(x => new WaterMeasurementConsumerDto()
                {
                    WaterMeasurementID = x.WaterMeasurementID,
                    WaterMeasurementTypeID = x.WaterMeasurementType.WaterMeasurementTypeID,
                    WaterMeasurementTypeName = x.WaterMeasurementType.WaterMeasurementTypeName,
                    UsageLocationID = x.UsageLocationID,
                    UsageLocationName = x.UsageLocation.Name,
                    UsageLocationType = x.UsageLocation.UsageLocationType.Name,
                    ParcelID = x.UsageLocation.ParcelID,
                    ParcelNumber = x.UsageLocation.Parcel.ParcelNumber,
                    ReportingDate = x.ReportedDate,
                    ReportedValueInFeet = x.ReportedValueInFeet,
                    ReportedValueInAcreFeet = x.ReportedValueInAcreFeet,
                    GeographyID = x.GeographyID
                })
                .ToList();
        }

        return Ok(waterMeasurementConsumerDtos);
    }

    [EndpointSummary("List by Geography and Parcel")]
    [EndpointDescription("List all water measurements for a specified geography and parcel")]
    [NoAccessBlock]
    [GeographyAccess]
    [ParcelAccess]
    [HttpGet("geographies/{geographyID}/parcels/{parcelID}/water-measurements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<ActionResult<List<WaterMeasurementConsumerDto>>> ListWaterMeasurementsByParcel([FromRoute] int geographyID, [FromRoute] int parcelID )
    {
        // ParcelAccess verifies user permissions
        var waterMeasurementConsumerDtos = dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.UsageLocation).ThenInclude(ul => ul.Parcel)
            .Include(x => x.UsageLocation).ThenInclude(x => x.UsageLocationType)
            .Where(x => x.GeographyID == geographyID && x.UsageLocation.ParcelID == parcelID)
            .Select(x => new WaterMeasurementConsumerDto()
            {
                WaterMeasurementID = x.WaterMeasurementID,
                WaterMeasurementTypeID = x.WaterMeasurementType.WaterMeasurementTypeID,
                WaterMeasurementTypeName = x.WaterMeasurementType.WaterMeasurementTypeName,
                UsageLocationID = x.UsageLocationID,
                UsageLocationName = x.UsageLocation.Name,
                UsageLocationType = x.UsageLocation.UsageLocationType.Name,
                ParcelID = x.UsageLocation.ParcelID,
                ParcelNumber = x.UsageLocation.Parcel.ParcelNumber,
                ReportingDate = x.ReportedDate,
                ReportedValueInFeet = x.ReportedValueInFeet,
                ReportedValueInAcreFeet = x.ReportedValueInAcreFeet,
                GeographyID = x.GeographyID
            }).ToList();

        return Ok(waterMeasurementConsumerDtos);
    }

    [EndpointSummary("List by Geography")]
    [EndpointDescription("List all water measurement types for a specified geography")]
    [Tags("Water Measurement Types")]
    [NoAccessBlock]
    [GeographyAccess]
    [HttpGet("geographies/{geographyID}/water-measurement-types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<WaterMeasurementTypeConsumerDto>> ListWaterMeasurementTypes([FromRoute] int geographyID)
    {
        var waterMeasurementTypeConsumerDtos = dbContext.WaterMeasurementTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .Select(x => new WaterMeasurementTypeConsumerDto()
            {
                WaterMeasurementTypeID = x.WaterMeasurementTypeID,
                WaterMeasurementTypeName = x.WaterMeasurementTypeName,
                WaterMeasurementCategoryType = x.WaterMeasurementCategoryType.WaterMeasurementCategoryTypeDisplayName,
                IsActive = x.IsActive,
                GeographyID = x.GeographyID
            }).ToList();

        return Ok(waterMeasurementTypeConsumerDtos);
    }
}