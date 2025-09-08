using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Features;
using Qanat.Common.GeoSpatial;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Swagger.Entities;
using Qanat.Swagger.Filters;
using Qanat.Swagger.Filters.Qanat.Swagger.Filters;
using Qanat.Swagger.Models;
using FeatureCollection = NetTopologySuite.Features.FeatureCollection;

namespace Qanat.Swagger.Controllers;

[Authorize]
[ApiController]
[Tags("Parcels")]
public class ParcelController(QanatDbContext dbContext, ILogger<ParcelController> logger) : ControllerBase
{

    [EndpointSummary("List by Geography")]
    [EndpointDescription("List all parcels for a specified geography")]
    [NoAccessBlock]
    [GeographyAccess]
    [HttpGet("geographies/{geographyID}/parcels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<ParcelConsumerDto>> ListParcels([FromRoute] int geographyID)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var parcelIDs = ListParcelIDsByGeographyIDAndCallingUser(geographyID, callingUser);
        var parcelConsumerDtos = ListParcelsByIDsGeographyIDAndCallingUserAsConsumerDtos(geographyID, parcelIDs, callingUser);

        return Ok(parcelConsumerDtos);
    }

    [EndpointSummary("List by Geography (GeoJSON)")]
    [EndpointDescription("List all parcels as a feature collection (GeoJSON) for a specified geography")]
    [NoAccessBlock]
    [GeographyAccess]
    [HttpGet("geographies/{geographyID}/parcels/feature-collection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<FeatureCollection> ListParcelsAsFeatureCollection([FromRoute] int geographyID)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var parcelIDs = ListParcelIDsByGeographyIDAndCallingUser(geographyID, callingUser);

        var parcels = dbContext.Parcels.Include(x => x.ParcelGeometry)
            .Include(x => x.WaterAccount)
            .Include(x => x.WellIrrigatedParcels).ThenInclude(x => x.Well)
            .Include(x => x.Wells)
            .Include(x => x.ParcelZones).ThenInclude(x => x.Zone)
            .Include(x => x.ParcelCustomAttribute)
            .AsNoTracking()
            .Where(x => parcelIDs.Contains(x.ParcelID)).ToList();

        var zoneGroups = dbContext.ZoneGroups.Include(x => x.Zones).AsNoTracking().Where(x => x.GeographyID == geographyID)
            .ToList();
        var customAttributes = dbContext.CustomAttributes.AsNoTracking().Where(x => x.GeographyID == geographyID && x.CustomAttributeTypeID == (int)CustomAttributeTypeEnum.Parcel).ToList();
        var featureCollection = new FeatureCollection();

        foreach (var parcel in parcels)
        {
            if (parcel.ParcelGeometry != null)
            {
                var attributesTable = new AttributesTable
                {
                    { "ParcelID", parcel.ParcelID },
                    { "ParcelNumber", parcel.ParcelNumber },
                    { "ParcelArea", parcel.ParcelArea },
                    { "WaterAccountNumber", parcel.WaterAccount?.WaterAccountNumber },
                    { "WaterAccountName", parcel.WaterAccount?.WaterAccountName },
                    { "WellsOnParcel", string.Join(", ", parcel.Wells.Select(x => x.WellName)) },
                    { "IrrigatedBy", string.Join(", ", parcel.WellIrrigatedParcels.Select(x => x.Well.WellName)) },
                    { "OwnerName", parcel.OwnerName },
                    { "OwnerAddress", parcel.OwnerAddress },
                    { "ParcelStatus", parcel.ParcelStatus.ParcelStatusDisplayName },
                };

                var parcelZoneGroupDict = parcel.ParcelZones.ToDictionary(x => x.Zone.ZoneGroupID, x => x.Zone.ZoneName);
                foreach (var zoneGroup in zoneGroups)
                {
                    attributesTable.Add(zoneGroup.ZoneGroupName, parcelZoneGroupDict.TryGetValue(zoneGroup.ZoneGroupID, out var value) ? value : null);
                }

                var customAttributeDict = string.IsNullOrWhiteSpace(parcel.ParcelCustomAttribute?.CustomAttributes)
                    ? null
                    : JsonSerializer.Deserialize<Dictionary<string, string>>(
                        parcel.ParcelCustomAttribute.CustomAttributes, GeoJsonSerializer.DefaultSerializerOptions);

                foreach (var customAttribute in customAttributes)
                {
                    attributesTable.Add(customAttribute.CustomAttributeName, customAttributeDict != null && customAttributeDict.TryGetValue(customAttribute.CustomAttributeName, out var value) ? value : null);
                }

                var feature = new Feature(parcel.ParcelGeometry.Geometry4326, attributesTable);
                featureCollection.Add(feature);
            }
        }

        return Ok(featureCollection);
    }

    [EndpointSummary("List by Geography and Water Account ID")]
    [EndpointDescription("List all parcels for a specified geography and water account")]
    [NoAccessBlock]
    [GeographyAccess]
    [WaterAccountAccess]
    [HttpGet("geographies/{geographyID}/water-accounts/{waterAccountID}/parcels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<ParcelConsumerDto>> ListParcelsByWaterAccountID([FromRoute] int geographyID, [FromRoute] int waterAccountID)
    {
        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        var parcelIDs = dbContext.Parcels.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID)
            .Select(x => x.ParcelID).ToList();

        var parcelConsumerDtos = ListParcelsByIDsGeographyIDAndCallingUserAsConsumerDtos(geographyID, parcelIDs, callingUser);

        return Ok(parcelConsumerDtos);
    }

    [EndpointSummary("Search by APN")]
    [EndpointDescription("Search for Parcels by APN within a given {geographyID}.  Returns Parcels that have parcel numbers that start with the {parcelNumber} field (minimum 3 characters to search), including geometry")]
    [NoAccessBlock]
    [GeographyAccess]
    [HttpGet("geographies/{geographyID}/parcels/{parcelNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<ParcelConsumerDto>> GetParcelByGeographyAndParcelNumber([FromRoute] int geographyID, [FromRoute] string parcelNumber)
    {
        if (parcelNumber.Length < 3)
        {
            return BadRequest($"Please enter at least 3 characters for the Parcel Number!");
        }

        var userID = HttpContext.User.GetUserID()!;
        var callingUser = Users.GetByUserID(dbContext, userID.Value);

        List<int> parcelIDs;
        if (UserPermissions.UserIsSystemAdmin(callingUser) || UserPermissions.UserIsGeographyManager(callingUser, geographyID))
        {
            parcelIDs = dbContext.Parcels.AsNoTracking().Where(x => x.GeographyID == geographyID && x.ParcelNumber.StartsWith(parcelNumber)).Select(x => x.ParcelID).ToList();
        }
        else
        {
            var associatedWaterAccountIDs = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, callingUser);

            parcelIDs = dbContext.Parcels.AsNoTracking()
                .Where(x => x.GeographyID == geographyID && x.WaterAccountID.HasValue && associatedWaterAccountIDs.Contains(x.WaterAccountID.Value) && x.ParcelNumber.StartsWith(parcelNumber))
                .Select(x => x.ParcelID)
                .ToList();
        }

        var parcelConsumerDtos = ListParcelsByIDsGeographyIDAndCallingUserAsConsumerDtos(geographyID, parcelIDs, callingUser);

        return Ok(parcelConsumerDtos);
    }

    private List<ParcelConsumerDto> ListParcelsByIDsGeographyIDAndCallingUserAsConsumerDtos(int geographyID, List<int> parcelIDs, UserDto callingUser)
    {
        var userCanViewManagerData = UserPermissions.UserIsSystemAdmin(callingUser) 
                                 || UserPermissions.UserIsGeographyManager(callingUser, geographyID);
        
        var parcelConsumerDtos = dbContext.Parcels.AsNoTracking()
            .Include(x => x.ParcelZones).ThenInclude(x => x.Zone).ThenInclude(x => x.ZoneGroup)
            .Include(x => x.ParcelCustomAttribute)
            .Where(x => x.GeographyID == geographyID && parcelIDs.Contains(x.ParcelID))
            .Select(x => new ParcelConsumerDto()
            {
                ParcelID = x.ParcelID,
                ParcelNumber = x.ParcelNumber,
                ParcelArea = x.ParcelArea,
                OwnerName = x.OwnerName,
                OwnerAddress = x.OwnerAddress,
                GeographyID = x.GeographyID,
                WaterAccountID = x.WaterAccountID,
                Zones = x.ParcelZones.Where(x => userCanViewManagerData || x.Zone.ZoneGroup.DisplayToAccountHolders)
                    .Select(z => new ZoneConsumerDto()
                    {
                        ZoneID = z.ZoneID,
                        ZoneName = z.Zone.ZoneName,
                        ZoneGroupID = z.Zone.ZoneGroupID,
                        ZoneGroupName = z.Zone.ZoneGroup.ZoneGroupName
                    }).ToList(),
            }).ToList();

        return parcelConsumerDtos;
    }

    private List<int> ListParcelIDsByGeographyIDAndCallingUser(int geographyID, UserDto callingUser)
    {
        List<int> parcelIDs;

        if (UserPermissions.UserIsSystemAdmin(callingUser) || UserPermissions.UserIsGeographyManager(callingUser, geographyID))
        {
            parcelIDs = dbContext.Parcels.AsNoTracking().Where(x => x.GeographyID == geographyID).Select(x => x.ParcelID).ToList();
        }
        else
        {
            var associatedWaterAccountIDs = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, callingUser);

            parcelIDs = dbContext.Parcels.AsNoTracking()
                .Where(x => x.GeographyID == geographyID && x.WaterAccountID.HasValue && associatedWaterAccountIDs.Contains(x.WaterAccountID.Value))
                .Select(x => x.ParcelID)
                .ToList();
        }

        return parcelIDs;
    }
}