using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Features;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Swagger.Entities;
using Qanat.Swagger.Filters;
using Qanat.Swagger.Models;

namespace Qanat.Swagger.Controllers;

[Authorize]
[ApiController]
[Tags("Usage Locations")]
public class UsageLocationController(QanatDbContext dbContext, ILogger<UsageLocationController> logger) : ControllerBase
{

    [EndpointSummary("List by Geography and Reporting Period")]
    [EndpointDescription("List all usage locations for a specified geography and reporting period")]
    [NoAccessBlock]
    [GeographyAccess]
    [ReportingPeriodAccess]
    [HttpGet("geographies/{geographyID}/reporting-periods/{reportingPeriodID}/usage-locations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<UsageLocationConsumerDto>> ListUsageLocations([FromRoute] int geographyID, [FromRoute] int reportingPeriodID)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var usageLocationIDs = ListUsageLocationIDsByReportingPeriodAndCallingUser(geographyID, reportingPeriodID, callingUser);

        var usageLocationConsumerDtos = dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.Parcel)
            .ThenInclude(x => x.WaterAccount)
            .Include(x => x.Parcel)
            .ThenInclude(x => x.ParcelZones)
            .ThenInclude(x => x.Zone)
            .ThenInclude(x => x.ZoneGroup)
            .Select(x => new UsageLocationConsumerDto()
            {
                UsageLocationID = x.UsageLocationID,
                Name = x.Name,
                Area = x.Area,
                UsageLocationType = x.UsageLocationType.Name,
                WaterAccountID = x.Parcel.WaterAccountID,
                WaterAccountNumber = x.Parcel.WaterAccount.WaterAccountNumber,
                ParcelID = x.ParcelID,
                ParcelZones = x.Parcel.ParcelZones.Select(z => new ZoneConsumerDto()
                {
                    ZoneID = z.ZoneID,
                    ZoneName = z.Zone.ZoneName,
                    ZoneGroupID = z.Zone.ZoneGroupID,
                    ZoneGroupName = z.Zone.ZoneGroup.ZoneGroupName
                }).ToList(),
                ReportingPeriodID = x.ReportingPeriodID,
                GeographyID = x.GeographyID,
            }).Where(x => usageLocationIDs.Contains(x.UsageLocationID) && x.ReportingPeriodID == reportingPeriodID).ToList();

        return Ok(usageLocationConsumerDtos);
    }

    [EndpointSummary("List by Geography (GeoJSON)")]
    [EndpointDescription("List all usage locations as a feature collection (GeoJSON) for a specified geography and reporting period")]
    [NoAccessBlock]
    [GeographyAccess]
    [ReportingPeriodAccess]
    [HttpGet("geographies/{geographyID}/reporting-periods/{reportingPeriodID}/usage-locations/feature-collection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<FeatureCollection> ListUsageLocationsAsFeatureCollection([FromRoute] int geographyID, [FromRoute] int reportingPeriodID)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var usageLocationIDs = ListUsageLocationIDsByReportingPeriodAndCallingUser(geographyID, reportingPeriodID, callingUser);

        var usageLocations = dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.UsageLocationGeometry)
            .Include(x => x.Parcel)
            .ThenInclude(x => x.WaterAccount)
            .Include( x => x.Parcel)
            .ThenInclude( x => x.ParcelZones)
            .ThenInclude(x => x.Zone)
            .ThenInclude(x => x.ZoneGroup)
            .Include(x => x.UsageLocationType)
            .AsNoTracking()
            .Where(x => usageLocationIDs.Contains(x.UsageLocationID)).ToList();

        var featureCollection = new FeatureCollection();

        foreach (var usageLocation in usageLocations)
        {
            if (usageLocation.UsageLocationGeometry != null)
            {
                var attributesTable = new AttributesTable
                {
                    { "Name", usageLocation.Name },
                    { "Area", usageLocation.Area },
                    { "Usage Location Type", usageLocation.UsageLocationType.Name },
                    { "Water Account ID", usageLocation.Parcel.WaterAccountID },
                    { "Water Account Number" , usageLocation.Parcel.WaterAccount?.WaterAccountNumber },
                    { "ParcelID" , usageLocation.ParcelID},
                    { "Parcel Zones", string.Join(", ", usageLocation.Parcel.ParcelZones.Select(z => $"{z.Zone.ZoneGroup.ZoneGroupName} : {z.Zone.ZoneName}"))},
                    { "Reporting Period ID", usageLocation.ReportingPeriodID },
                    { "Geography ID", usageLocation.GeographyID },
                };

                var feature = new Feature(usageLocation.UsageLocationGeometry.Geometry4326, attributesTable);
                featureCollection.Add(feature);
            }
        }

        return Ok(featureCollection);
    }

    private List<int> ListUsageLocationIDsByReportingPeriodAndCallingUser(int geographyID, int reportingPeriodID, UserDto callingUser)
    {
        List<int> usageLocationIDs;

        if (UserPermissions.UserIsSystemAdmin(callingUser))
        {
            usageLocationIDs = dbContext.UsageLocations.AsNoTracking().Where(x => x.ReportingPeriodID == reportingPeriodID)
                .Select(x => x.UsageLocationID).ToList();
        }
        else
        {
            var managedGeographyIDs = callingUser.GeographyFlags
                .Where(x => x.Value[Flag.HasManagerDashboard.FlagName]).Select(x => x.Key);
            var associatedWaterAccountIDs = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, callingUser);

            usageLocationIDs = dbContext.UsageLocations.AsNoTracking()
                .Include(x => x.Parcel)
                .Where(x => managedGeographyIDs.Contains(x.GeographyID) ||
                            (x.Parcel.WaterAccountID.HasValue && associatedWaterAccountIDs.Contains(x.Parcel.WaterAccountID.Value)))
                .Select(x => x.UsageLocationID)
                .ToList();
        }

        return usageLocationIDs;
    }
}