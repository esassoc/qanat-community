using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    [EndpointSummary("List by Geography and Year")]
    [EndpointDescription("List all usage locations for a specified geography and year")]
    [NoAccessBlock]
    [GeographyAccess]
    [ReportingPeriodAccess]
    [HttpGet("geographies/{geographyID}/years/{year}/usage-locations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<ActionResult<List<UsageLocationConsumerDto>>> ListUsageLocations([FromRoute] int geographyID, [FromRoute] int year)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        // ReportingPeriodAccess guarantees existence
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, year);

        var usageLocationIDs = ListUsageLocationIDsByReportingPeriodAndCallingUser(geographyID, reportingPeriod.ReportingPeriodID, callingUser);
        var userCanViewManagerData = UserPermissions.UserIsSystemAdmin(callingUser) ||
                               UserPermissions.UserIsGeographyManager(callingUser, geographyID);

        var usageLocationConsumerDtos = dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccount)
            .Include(x => x.Parcel).ThenInclude(x => x.ParcelZones).ThenInclude(x => x.Zone).ThenInclude(x => x.ZoneGroup)
            .Select(x => new UsageLocationConsumerDto()
            {
                UsageLocationID = x.UsageLocationID,
                Name = x.Name,
                Area = x.Area,
                UsageLocationType = x.UsageLocationType.Name,
                WaterAccountID = x.Parcel.WaterAccountID,
                WaterAccountNumber = x.Parcel.WaterAccount.WaterAccountNumber,
                ParcelID = x.ParcelID,
                ParcelNumber = x.Parcel.ParcelNumber,
                ParcelZones = x.Parcel.ParcelZones
                    .Where(x => userCanViewManagerData || x.Zone.ZoneGroup.DisplayToAccountHolders)
                    .Select(z => new ZoneConsumerDto()
                    {
                        ZoneID = z.ZoneID,
                        ZoneName = z.Zone.ZoneName,
                        ZoneGroupID = z.Zone.ZoneGroupID,
                        ZoneGroupName = z.Zone.ZoneGroup.ZoneGroupName
                    }).ToList(),
                ReportingPeriodID = x.ReportingPeriodID,
                ReportingPeriodName = x.ReportingPeriod.Name,
                GeographyID = x.GeographyID,
            }).Where(x => usageLocationIDs.Contains(x.UsageLocationID) && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID).ToList();

        return Ok(usageLocationConsumerDtos);
    }

    [EndpointSummary("List by Geography and Year (GeoJSON)")]
    [EndpointDescription("List all usage locations as a feature collection (GeoJSON) for a specified geography and year")]
    [NoAccessBlock]
    [GeographyAccess]
    [ReportingPeriodAccess]
    [HttpGet("geographies/{geographyID}/years/{year}/usage-locations/feature-collection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<ActionResult<FeatureCollection>> ListUsageLocationsAsFeatureCollection([FromRoute] int geographyID, [FromRoute] int year)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var userCanViewManagerData = UserPermissions.UserIsSystemAdmin(callingUser) ||
                                     UserPermissions.UserIsGeographyManager(callingUser, geographyID);

        // ReportingPeriodAccess guarantees existence
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, year);

        var usageLocationIDs = ListUsageLocationIDsByReportingPeriodAndCallingUser(geographyID, reportingPeriod.ReportingPeriodID, callingUser);

        var usageLocations = dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.UsageLocationGeometry)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccount)
            .Include( x => x.Parcel).ThenInclude( x => x.ParcelZones).ThenInclude(x => x.Zone).ThenInclude(x => x.ZoneGroup)
            .Include(x => x.UsageLocationType)
            .Include(x => x.ReportingPeriod)
            .AsNoTracking()
            .Where(x => usageLocationIDs.Contains(x.UsageLocationID)).ToList();

        var featureCollection = new FeatureCollection();

        foreach (var usageLocation in usageLocations)
        {
            if (usageLocation.UsageLocationGeometry != null)
            {
                var attributesTable = new AttributesTable
                {
                    { "UsageLocationID", usageLocation.UsageLocationID },
                    { "Name", usageLocation.Name },
                    { "Area", usageLocation.Area },
                    { "UsageLocationType", usageLocation.UsageLocationType.Name },
                    { "WaterAccountID", usageLocation.Parcel.WaterAccountID },
                    { "WaterAccountNumber" , usageLocation.Parcel.WaterAccount?.WaterAccountNumber },
                    { "ParcelID" , usageLocation.ParcelID},
                    { "ParcelNumber", usageLocation.Parcel.ParcelNumber },
                    { "ParcelZones", string.Join(", ", usageLocation.Parcel.ParcelZones
                        .Where(x => userCanViewManagerData || x.Zone.ZoneGroup.DisplayToAccountHolders)
                        .Select(z => $"{z.Zone.ZoneGroup.ZoneGroupName} : {z.Zone.ZoneName}"))},
                    { "ReportingPeriodID", usageLocation.ReportingPeriodID },
                    { "ReportingPeriodName", usageLocation.ReportingPeriod.Name },
                    { "GeographyID", usageLocation.GeographyID },
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

        if (UserPermissions.UserIsSystemAdmin(callingUser) || UserPermissions.UserIsGeographyManager(callingUser, geographyID))
        {
            usageLocationIDs = dbContext.UsageLocations.AsNoTracking().Where(x => x.ReportingPeriodID == reportingPeriodID)
                .Select(x => x.UsageLocationID).ToList();
        }
        else
        {
            var associatedWaterAccountIDs = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, callingUser);

            usageLocationIDs = dbContext.UsageLocations.AsNoTracking()
                .Include(x => x.Parcel)
                .Where(x => x.ReportingPeriodID == reportingPeriodID && 
                            x.Parcel.WaterAccountID.HasValue && associatedWaterAccountIDs.Contains(x.Parcel.WaterAccountID.Value))
                .Select(x => x.UsageLocationID)
                .ToList();
        }

        return usageLocationIDs;
    }
}