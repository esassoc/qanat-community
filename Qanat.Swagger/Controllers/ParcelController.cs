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
using Qanat.Swagger.Models;
using FeatureCollection = NetTopologySuite.Features.FeatureCollection;

namespace Qanat.Swagger.Controllers;

[Authorize]
[ApiController]
[Tags("Parcels")]
public class ParcelController(QanatDbContext dbContext, ILogger<ParcelController> logger) : ControllerBase
{

    [EndpointSummary("List by Geography")]
    [EndpointDescription("List all parcel numbers for a specified geography")]
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

        var parcelConsumerDtos = ListParcelsByGeographyIDAndCallingUserAsConsumerDtos(geographyID, callingUser);

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
                    { "APN", parcel.ParcelNumber },
                    { "Area (ac)", parcel.ParcelArea },
                    { "Account #", parcel.WaterAccount?.WaterAccountNumber },
                    { "Water Account Name", parcel.WaterAccount?.WaterAccountName },
                    { "Wells on Parcel", string.Join(", ", parcel.Wells.Select(x => x.WellName)) },
                    { "Irrigated By", string.Join(", ", parcel.WellIrrigatedParcels.Select(x => x.Well.WellName)) },
                    { "Owner Name", parcel.OwnerName },
                    { "Owner Address", parcel.OwnerAddress },
                    { "Parcel Status", parcel.ParcelStatus.ParcelStatusDisplayName },
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

        var parcelConsumerDtos = ListParcelsByGeographyIDAndCallingUserAsConsumerDtos(geographyID, callingUser);

        return Ok(parcelConsumerDtos);
    }

    private List<ParcelConsumerDto> ListParcelsByGeographyIDAndCallingUserAsConsumerDtos(int geographyID, UserDto callingUser)
    {
       var parcelIDs = ListParcelIDsByGeographyIDAndCallingUser(geographyID, callingUser);

        var parcelConsumerDtos = dbContext.Parcels.AsNoTracking()
            .Where(x => parcelIDs.Contains(x.ParcelID))
            .Select(x => new ParcelConsumerDto()
            {
                ParcelID = x.ParcelID,
                ParcelNumber = x.ParcelNumber,
                ParcelArea = x.ParcelArea,
                OwnerName = x.OwnerName,
                OwnerAddress = x.OwnerAddress,
                GeographyID = x.GeographyID,
                WaterAccountID = x.WaterAccountID
            }).ToList();

        return parcelConsumerDtos;
    }

    private List<int> ListParcelIDsByGeographyIDAndCallingUser(int geographyID, UserDto callingUser)
    {
        List<int> parcelIDs;

        if (UserPermissions.UserIsSystemAdmin(callingUser))
        {
            parcelIDs = dbContext.Parcels.AsNoTracking().Where(x => x.GeographyID == geographyID).Select(x => x.ParcelID).ToList();
        }
        else
        {
            var managedGeographyIDs = UserPermissions.ListManagedGeographyIDsByUser(callingUser);
            var associatedWaterAccountIDs = UserPermissions.ListAssociatedWaterAccountsByGeographyIDAndUser(dbContext, geographyID, callingUser);

            parcelIDs = dbContext.Parcels.AsNoTracking()
                .Where(x => managedGeographyIDs.Contains(x.GeographyID) || (x.WaterAccountID.HasValue && associatedWaterAccountIDs.Contains(x.WaterAccountID.Value)))
                .Select(x => x.ParcelID)
                .ToList();
        }

        return parcelIDs;
    }
}