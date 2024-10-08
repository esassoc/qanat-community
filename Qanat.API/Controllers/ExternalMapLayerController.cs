using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class ExternalMapLayerController : SitkaController<ExternalMapLayerController>
{
    public ExternalMapLayerController(QanatDbContext dbContext, ILogger<ExternalMapLayerController> logger, IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    private static IQueryable<ExternalMapLayer> GetImpl(QanatDbContext dbContext)
    {
        return dbContext.ExternalMapLayers.AsNoTracking()
            .Include(x => x.Geography);
    }

    private static List<ErrorMessage> ValidateExternalMapLayerDto(ExternalMapLayerSimpleDto externalMapLayer)
    {
        var results = new List<ErrorMessage>();
        if (string.IsNullOrWhiteSpace(externalMapLayer.ExternalMapLayerDisplayName))
        {
            results.Add(new ErrorMessage()
            {
                Type = "External Map Layer",
                Message = "Please enter a Name."
            });
        }

        if (externalMapLayer.ExternalMapLayerTypeID == 0)
        {
            results.Add(new ErrorMessage()
            {
                Type = "External Map Layer",
                Message = "Please select a Layer Type."
            });
        }

        if (string.IsNullOrWhiteSpace(externalMapLayer.ExternalMapLayerURL))
        {
            results.Add(new ErrorMessage()
            {
                Type = "External Map Layer",
                Message = "Please enter a URL."
            });
        }

        if (externalMapLayer.ExternalMapLayerTypeID == (int)ExternalMapLayerTypeEnum.ESRIFeatureServer 
            && (externalMapLayer.MinZoom == null
                || (externalMapLayer.MinZoom < 0 || externalMapLayer.MinZoom > 25)))
        {
            results.Add(new ErrorMessage()
            {
                Type = "External Map Layer",
                Message = "Minimum zoom must be between 0 and 25."
            });
        }
        return results;
    }

    [HttpGet("geographies/{geographyID}/external-map-layers")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ExternalMapLayerRights, RightsEnum.Read)]
    public ActionResult<List<ExternalMapLayerDto>> Get([FromRoute] int geographyID)
    {
        return Ok(GetImpl(_dbContext).Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsExternalMapLayerDto()).ToList());
    }

    [HttpGet("geographies/{geographyID}/external-map-layers/active")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ExternalMapLayerRights, RightsEnum.Read)]
    public ActionResult<List<ExternalMapLayerDto>> GetActiveExternalMapLayers([FromRoute] int geographyID)
    {
        return Ok(GetImpl(_dbContext).Where(x => x.GeographyID == geographyID && x.IsActive == true)
            .Select(x => x.AsExternalMapLayerDto()).ToList());
    }

    [HttpGet("geographies/{geographyID}/external-map-layer/{externalMapLayerID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ExternalMapLayerRights, RightsEnum.Read)]
    public ActionResult<ExternalMapLayerSimpleDto> GetExternalMapLayerByID([FromRoute] int geographyID, int externalMapLayerID)
    {
        return Ok(_dbContext.ExternalMapLayers.Single(x => x.GeographyID == geographyID && x.ExternalMapLayerID == externalMapLayerID).AsSimpleDto());
    }

    [HttpPost("geographies/{geographyID}/external-map-layers")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ExternalMapLayerRights, RightsEnum.Create)]
    public ActionResult Add([FromRoute] int geographyID, [FromBody] ExternalMapLayerSimpleDto externalMapLayerDto)
    {
        var errors = ValidateExternalMapLayerDto(externalMapLayerDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var eml = _dbContext.ExternalMapLayers.SingleOrDefault(x =>
            x.ExternalMapLayerDisplayName == externalMapLayerDto.ExternalMapLayerDisplayName && x.GeographyID == geographyID);
        if (eml != null)
        {
            return BadRequest("There is already a geospatial data layer with this name for this geography.");
        }
        var externalMapLayer = new ExternalMapLayer()
        {
            GeographyID = externalMapLayerDto.GeographyID,
            ExternalMapLayerDisplayName = externalMapLayerDto.ExternalMapLayerDisplayName,
            ExternalMapLayerTypeID = externalMapLayerDto.ExternalMapLayerTypeID,
            ExternalMapLayerURL = externalMapLayerDto.ExternalMapLayerURL,
            ExternalMapLayerDescription = externalMapLayerDto.ExternalMapLayerDescription,
            LayerIsOnByDefault = externalMapLayerDto.LayerIsOnByDefault,
            IsActive = externalMapLayerDto.IsActive,
            PopUpField = externalMapLayerDto.PopUpField,
            MinZoom = externalMapLayerDto.MinZoom,
        };
        _dbContext.ExternalMapLayers.Add(externalMapLayer);
        _dbContext.SaveChanges();
        return Ok();
    }

    [HttpPut("geographies/{geographyID}/external-map-layers")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ExternalMapLayerRights, RightsEnum.Update)]
    public ActionResult Update([FromRoute] int geographyID, [FromBody] ExternalMapLayerSimpleDto externalMapLayerDto)
    {
        var errors = ValidateExternalMapLayerDto(externalMapLayerDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var externalMapLayer =
            _dbContext.ExternalMapLayers.Single(x => x.ExternalMapLayerID == externalMapLayerDto.ExternalMapLayerID);

        externalMapLayer.ExternalMapLayerDisplayName = externalMapLayerDto.ExternalMapLayerDisplayName;
        externalMapLayer.ExternalMapLayerTypeID = externalMapLayerDto.ExternalMapLayerTypeID;
        externalMapLayer.ExternalMapLayerURL = externalMapLayerDto.ExternalMapLayerURL;
        externalMapLayer.ExternalMapLayerDescription = externalMapLayerDto.ExternalMapLayerDescription;
        externalMapLayer.LayerIsOnByDefault = externalMapLayerDto.LayerIsOnByDefault;
        externalMapLayer.IsActive = externalMapLayerDto.IsActive;
        externalMapLayer.PopUpField = externalMapLayerDto.PopUpField;
        externalMapLayer.MinZoom = externalMapLayerDto.MinZoom;

        _dbContext.SaveChanges();
        return Ok();
    }
}