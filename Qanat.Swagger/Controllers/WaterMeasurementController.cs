using System.Collections.Generic;
using System.Linq;
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
    [EndpointSummary("List by Geography")]
    [EndpointDescription("List all water measurements a specified geography, reporting period, and water measurement type")]
    [NoAccessBlock]
    [GeographyAccess]
    [ReportingPeriodAccess]
    [HttpGet("geographies/{geographyID}/reporting-periods/{reportingPeriodID}/water-measurement-types/{waterMeasurementTypeID}/water-measurements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<WaterMeasurementConsumerDto>> ListWaterMeasurements([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromRoute] int waterMeasurementTypeID )
    {
        var waterMeasurementType = dbContext.WaterMeasurementTypes.AsNoTracking()
            .SingleOrDefault(x => x.WaterMeasurementTypeID == waterMeasurementTypeID && x.GeographyID == geographyID);
        if (waterMeasurementType == null)
        {
            return NotFound($"Water measurement type with ID {waterMeasurementTypeID} does not exist within given geography!");
        }

        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        // Reporting period existence is now checked by ReportingPeriodAccessAttribute
        var reportingPeriod = dbContext.ReportingPeriods.AsNoTracking().SingleOrDefault(x => x.ReportingPeriodID == reportingPeriodID && x.GeographyID == geographyID);
        // No need to check for null, attribute handles NotFound

        List<WaterMeasurementConsumerDto> waterMeasurementConsumerDtos;
        if (UserPermissions.UserIsSystemAdmin(callingUser))
        {
            waterMeasurementConsumerDtos = dbContext.WaterMeasurements.AsNoTracking()
                .Where(x =>  x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID 
                                                          && x.ReportedDate >= reportingPeriod.StartDate && x.ReportedDate <= reportingPeriod.EndDate)
                .Select(x => new WaterMeasurementConsumerDto()
                {
                    WaterMeasurementID = x.WaterMeasurementID,
                    WaterMeasurementTypeID = x.WaterMeasurementType.WaterMeasurementTypeID,
                    UsageLocationID = x.UsageLocationID,
                    ReportingDate = x.ReportedDate,
                    ReportedValueInFeet = x.ReportedValueInFeet,
                    ReportedValueInAcreFeet = x.ReportedValueInAcreFeet
                }).ToList();
        }
        else
        {
            var managedGeographyIDs = UserPermissions.ListManagedGeographyIDsByUser(callingUser);
            var associatedWaterAccountIDs = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, callingUser);

            var associatedUsageLocationIDs = dbContext.UsageLocations.AsNoTracking()
                .Include(x => x.Parcel)
                .Where(x => x.GeographyID == geographyID && x.Parcel.WaterAccountID.HasValue && associatedWaterAccountIDs.Contains(x.Parcel.WaterAccountID.Value))
                .Select(x => x.UsageLocationID).ToList();

            waterMeasurementConsumerDtos = dbContext.WaterMeasurements.AsNoTracking()
                .Where(x => (managedGeographyIDs.Contains(x.GeographyID) ||
                             associatedUsageLocationIDs.Contains(x.UsageLocationID))
                            && x.WaterMeasurementTypeID == waterMeasurementTypeID &&
                            x.ReportedDate >= reportingPeriod.StartDate && x.ReportedDate <= reportingPeriod.EndDate)
                .Select(x => new WaterMeasurementConsumerDto()
                {
                    WaterMeasurementID = x.WaterMeasurementID,
                    WaterMeasurementTypeID = x.WaterMeasurementType.WaterMeasurementTypeID,
                    UsageLocationID = x.UsageLocationID,
                    ReportingDate = x.ReportedDate,
                    ReportedValueInFeet = x.ReportedValueInFeet,
                    ReportedValueInAcreFeet = x.ReportedValueInAcreFeet
                })
                .ToList();
        }

        return Ok(waterMeasurementConsumerDtos);
    }

    [EndpointSummary("List by Geography")]
    [EndpointDescription("List all water measurements a specified geography")]
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