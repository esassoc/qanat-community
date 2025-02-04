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
[Route("geographies/{geographyID}/water-measurement-types/")]
public class WaterMeasurementTypeController : SitkaController<WaterMeasurementTypeController>
{
    public WaterMeasurementTypeController(QanatDbContext dbContext, ILogger<WaterMeasurementTypeController> logger, IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public ActionResult<List<WaterMeasurementTypeSimpleDto>> GetWaterMeasurementTypes([FromRoute] int geographyID)
    {
        var waterMeasurementTypeDtos = WaterMeasurementTypes.ListAsSimpleDto(_dbContext, geographyID);
        return Ok(waterMeasurementTypeDtos);
    }

    [HttpGet("active")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public ActionResult<List<WaterMeasurementTypeSimpleDto>> GetActiveAndEditableWaterMeasurementTypes([FromRoute] int geographyID)
    {
        var waterMeasurementTypeDtos = WaterMeasurementTypes.ListActiveAndEditableAsSimpleDto(_dbContext, geographyID);
        return Ok(waterMeasurementTypeDtos);
    }

    [HttpGet("self-reportable")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public ActionResult<List<WaterMeasurementTypeSimpleDto>> GetActiveAndSelfReportableWaterMeasurementTypes([FromRoute] int geographyID)
    {
        var waterMeasurementTypeDtos = WaterMeasurementTypes.ListActiveAndSelfReportableAsSimpleDto(_dbContext, geographyID);
        return Ok(waterMeasurementTypeDtos);
    }
}