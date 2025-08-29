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
[Tags("Wells")]
public class WellController(QanatDbContext dbContext, ILogger<WellController> logger) : ControllerBase
{
    [EndpointSummary("List by Geography")]
    [EndpointDescription("List all wells for a specified geography")]
    [NoAccessBlock]
    [GeographyAccess]
    [HttpGet("geographies/{geographyID}/wells")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<WellConsumerDto>> ListWells([FromRoute] int geographyID)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var wells = ListWellsByGeographyIDAndCallingUser(geographyID, callingUser);

        var wellConsumerDtos = wells
            .Select(x => new WellConsumerDto
            {
                WellID = x.WellID,
                WellName = x.WellName,
                WellStatus = x.WellStatus.WellStatusDisplayName,
                StateWCRNumber = x.StateWCRNumber,
                CountyPermitNumber = x.CountyWellPermitNumber,
                WellDepth = x.WellDepth,
                DateDrilled = x.DateDrilled,
                ParcelID = x.ParcelID,
                IrrigatedParcelIDs = x.WellIrrigatedParcels.Select(y => y.ParcelID).ToList(),
                GeographyID = x.GeographyID,
            }).ToList();

        return Ok(wellConsumerDtos);
    }

    [EndpointSummary("List by Geography (GeoJSON)")]
    [EndpointDescription("List all wells as a feature collection (GeoJSON) for a specified geography")]
    [NoAccessBlock]
    [GeographyAccess]
    [HttpGet("geographies/{geographyID}/wells/feature-collection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<FeatureCollection> ListWellsAsFeatureCollection([FromRoute] int geographyID)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var wells = ListWellsByGeographyIDAndCallingUser(geographyID, callingUser);

        var featureCollection = new FeatureCollection();

        foreach (var well in wells)
        {
            if (well.LocationPoint != null)
            {
                var attributesTable = new AttributesTable
                {
                    { "Well ID", well.WellID},
                    { "Well Name", well.WellName},
                    { "Default APN", well.Parcel?.ParcelNumber },
                    { "Irrigates", string.Join(", ", well.WellIrrigatedParcels.Select(x => x.Parcel.ParcelNumber))},
                    { "County Permit #", well.CountyWellPermitNumber },
                    { "State WCR #", well.StateWCRNumber },
                    { "Date Drilled", well.DateDrilled},
                    { "Well Depth", well.WellDepth},
                    { "Well Status", well.WellStatus.WellStatusDisplayName },
                    { "Latitude", well.LocationPoint4326.Coordinate.Y },
                    { "Longitude", well.LocationPoint4326.Coordinate.X },
                };

                var feature = new Feature(well.LocationPoint, attributesTable);
                featureCollection.Add(feature);
            }
        }

        return Ok(featureCollection);
    }

    [EndpointSummary("List Meter Readings by Geography")]
    [EndpointDescription("List all meter readings for a specified well")]
    [NoAccessBlock]
    [GeographyAccess]
    [HttpGet("geographies/{geographyID}/wells/{wellID}/meter-readings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<WellConsumerDto>> ListWellMeterReadings([FromRoute] int geographyID, [FromRoute] int wellID)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var wells = ListWellsByGeographyIDAndCallingUser(geographyID, callingUser);
        if (!wells.Any(x => x.WellID == wellID))
        {
            return NotFound();
        }

        var meterReadingConsumerDtos = dbContext.MeterReadings.AsNoTracking()
            .Include(x => x.Meter)
            .Where(x => x.WellID == wellID && x.GeographyID == geographyID)
            .Select(x => new MeterReadingConsumerDto
            {
                MeterReadingID = x.MeterReadingID,
                MeterSerialNumber = x.Meter.SerialNumber,
                WellID = x.WellID,
                ReadingDate = x.ReadingDate,
                PreviousReading = x.PreviousReading,
                CurrentReading = x.CurrentReading,
                VolumeInAcreFeet = x.VolumeInAcreFeet,
                ReaderInitials = x.ReaderInitials,
                Comment = x.Comment
            }).ToList();


        return Ok(meterReadingConsumerDtos);
    }

    private List<Well> ListWellsByGeographyIDAndCallingUser(int geographyID, UserDto callingUser)
    {
        List<Well> wells;

        if (UserPermissions.UserIsSystemAdmin(callingUser))
        {
            wells = dbContext.Wells.AsNoTracking()
                .Include(x => x.Parcel)
                .Include(x => x.WellIrrigatedParcels).ThenInclude(x => x.Parcel)
                .Where(x => x.GeographyID == geographyID).ToList();
        }
        else
        {
            var managedGeographyIDs = callingUser.GeographyFlags
                .Where(x => x.Value[Flag.HasManagerDashboard.FlagName]).Select(x => x.Key);
            var associatedWaterAccountIDs = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, callingUser);

            wells = dbContext.Wells.AsNoTracking()
                .Include(x => x.Parcel)
                .Include(x => x.WellIrrigatedParcels).ThenInclude(x => x.Parcel)
                .Where(x => managedGeographyIDs.Contains(x.GeographyID) || 
                            (x.ParcelID.HasValue && x.Parcel.WaterAccountID.HasValue && associatedWaterAccountIDs.Contains(x.Parcel.WaterAccountID.Value)))
                .ToList();
        }

        return wells;
    }
}