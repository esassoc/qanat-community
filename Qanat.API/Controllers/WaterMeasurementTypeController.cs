using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
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
public class WaterMeasurementTypeController : SitkaController<WaterMeasurementTypeController>
{
    public WaterMeasurementTypeController(QanatDbContext dbContext, ILogger<WaterMeasurementTypeController> logger,
        IOptions<QanatConfiguration> qanatConfiguration)
        : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("geographies/{geographyID}/water-measurement-types/")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public ActionResult<List<WaterMeasurementTypeSimpleDto>> GetWaterMeasurementTypes([FromRoute] int geographyID)
    {
        var waterMeasurementTypeDtos = WaterMeasurementTypes.ListAsSimpleDto(_dbContext, geographyID);
        return Ok(waterMeasurementTypeDtos);
    }

    [HttpGet("geographies/{geographyID}/water-measurement-types/active")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public ActionResult<List<WaterMeasurementTypeSimpleDto>> GetActiveAndEditableWaterMeasurementTypes([FromRoute] int geographyID)
    {
        var waterMeasurementTypeDtos = WaterMeasurementTypes.ListActiveAndEditableAsSimpleDto(_dbContext, geographyID);
        return Ok(waterMeasurementTypeDtos);
    }

}