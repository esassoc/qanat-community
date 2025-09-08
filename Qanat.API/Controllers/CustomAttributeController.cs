using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class CustomAttributeController : SitkaController<CustomAttributeController>
{
    public CustomAttributeController(QanatDbContext dbContext, ILogger<CustomAttributeController> logger,
        IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("geographies/{geographyID}/custom-attributes/{customAttributeTypeID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.CustomAttributeRights, RightsEnum.Read)]
    public ActionResult<List<CustomAttributeSimpleDto>> ListCustomAttributesForGeography([FromRoute] int geographyID, [FromRoute] int customAttributeTypeID)
    {
        return Ok(CustomAttributes.ListByGeographyIDAndTypeIDAsSimpleDto(_dbContext, geographyID, customAttributeTypeID));
    }

    [HttpPut("geographies/{geographyID}/custom-attributes/{customAttributeTypeID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRolePermission(PermissionEnum.CustomAttributeRights, RightsEnum.Update)]
    public ActionResult MergeCustomAttributes([FromRoute] int geographyID, [FromRoute] int customAttributeTypeID, [FromBody] List<CustomAttributeSimpleDto> customAttributeSimpleDtos)
    {
        CustomAttributes.MergeByGeographyIDAndTypeID(_dbContext, geographyID, customAttributeTypeID, customAttributeSimpleDtos);
        return Ok();
    }

    [HttpGet("custom-attributes/water-accounts/{waterAccountID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithGeographyRolePermission(PermissionEnum.CustomAttributeRights, RightsEnum.Read)]
    public ActionResult<EntityCustomAttributesDto> ListAllWaterAccountCustomAttributes([FromRoute] int waterAccountID)
    {
        var waterAccountCustomAttributeDto = CustomAttributes.ListAllByWaterAccountIDAsEntityCustomAttributeDtos(_dbContext, waterAccountID);
        return Ok(waterAccountCustomAttributeDto);
    }

    [HttpPost("custom-attributes/water-accounts/{waterAccountID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithGeographyRolePermission(PermissionEnum.CustomAttributeRights, RightsEnum.Update)]
    public ActionResult UpdateWaterAccountCustomAttributes([FromRoute] int waterAccountID, [FromBody] EntityCustomAttributesDto entityCustomAttributesDto)
    {
        var sparseCustomAttributesDictionary = entityCustomAttributesDto.CustomAttributes
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

        var waterAccountCustomAttribute = _dbContext.WaterAccountCustomAttributes.SingleOrDefault(x => x.WaterAccountID == waterAccountID);
        if (waterAccountCustomAttribute == null)
        {
            _dbContext.WaterAccountCustomAttributes.Add(new WaterAccountCustomAttribute()
            {
                WaterAccountID = waterAccountID,
                CustomAttributes = JsonSerializer.Serialize(sparseCustomAttributesDictionary)
            });
        }
        else
        {
            waterAccountCustomAttribute.CustomAttributes = JsonSerializer.Serialize(sparseCustomAttributesDictionary);
        }
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpGet("custom-attributes/parcels/{parcelID}")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.CustomAttributeRights, RightsEnum.Read)]
    public ActionResult<EntityCustomAttributesDto> ListAllParcelCustomAttributes([FromRoute] int parcelID)
    {
        var parcelCustomAttributeDto = CustomAttributes.ListAllByParcelIDAsEntityCustomAttributeDtos(_dbContext, parcelID);
        return Ok(parcelCustomAttributeDto);
    }

    [HttpPost("custom-attributes/parcels/{parcelID}")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.CustomAttributeRights, RightsEnum.Update)]
    public ActionResult UpdateParcelCustomAttributes([FromRoute] int parcelID, [FromBody] EntityCustomAttributesDto entityCustomAttributesDto)
    {
        var sparseCustomAttributesDictionary = entityCustomAttributesDto.CustomAttributes
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

        var parcelCustomAttribute = _dbContext.ParcelCustomAttributes.SingleOrDefault(x => x.ParcelID == parcelID);
        if (parcelCustomAttribute == null)
        {
            _dbContext.ParcelCustomAttributes.Add(new ParcelCustomAttribute()
            {
                ParcelID = parcelID,
                CustomAttributes = JsonSerializer.Serialize(sparseCustomAttributesDictionary)
            });
        }
        else
        {
            parcelCustomAttribute.CustomAttributes = JsonSerializer.Serialize(sparseCustomAttributesDictionary);
        }
        _dbContext.SaveChanges();

        return Ok();
    }
}