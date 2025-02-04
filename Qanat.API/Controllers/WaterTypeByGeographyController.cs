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

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/water-types")]
public class WaterTypeByGeographyController : SitkaController<WaterTypeByGeographyController>
{
    public WaterTypeByGeographyController(QanatDbContext dbContext, ILogger<WaterTypeByGeographyController> logger,
        IOptions<QanatConfiguration> qanatConfiguration)
        : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public ActionResult<List<WaterTypeSimpleDto>> GetWaterTypes([FromRoute] int geographyID)
    {
        var waterTypeDtos = WaterTypes.ListAsSimpleDto(_dbContext, geographyID);
        return Ok(waterTypeDtos);
    }

    [HttpGet("active")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public ActionResult<List<WaterTypeSimpleDto>> GetActiveWaterTypes([FromRoute] int geographyID)
    {
        var waterTypeDtos = _dbContext.WaterTypes.Where(x => x.IsActive == true && x.GeographyID == geographyID)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.AsSimpleDto()).ToList();
        return Ok(waterTypeDtos);
    }

    [HttpPut]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Update)]
    public ActionResult<List<WaterTypeSimpleDto>> UpdateWaterTypes([FromBody] List<WaterTypeSimpleDto> waterTypeDtos, [FromRoute] int geographyID)
    {
        WaterTypes.UpdateForGeography(_dbContext, geographyID, waterTypeDtos);
        var updatedWaterTypeDtos = WaterTypes.ListAsSimpleDto(_dbContext, geographyID);
        return Ok(updatedWaterTypeDtos);
    }
}