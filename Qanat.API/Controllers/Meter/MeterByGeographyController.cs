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
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/meters")]
public class MeterByGeographyController(QanatDbContext dbContext, ILogger<MeterByGeographyController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<MeterByGeographyController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.MeterRights, RightsEnum.Create)]
    public async Task<ActionResult<MeterGridDto>> AddMeter([FromRoute] int geographyID, [FromBody] MeterGridDto meterGridDto)
    {
        var errors = await Meters.ValidateMeterUpsertAsync(_dbContext, geographyID, meterGridDto);
        if (errors.Any())
        {
            errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
            return BadRequest(ModelState);
        }

        var responseDto = Meters.AddMeter(_dbContext, meterGridDto);
        return Ok(responseDto);
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.MeterRights, RightsEnum.Read)]
    public ActionResult<List<MeterGridDto>> GetMeters([FromRoute] int geographyID)
    {
        var meterDtos = Meters.GetGeographyIDAsGridDto(_dbContext, geographyID);
        return Ok(meterDtos);
    }

    [HttpGet("unassigned")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.MeterRights, RightsEnum.Read)]
    public ActionResult<List<MeterLinkDisplayDto>> ListUnassignedMeters([FromRoute] int geographyID)
    {
        var meterLinkDisplayDtos = Meters.ListUnassignedAsLinkDisplayDtos(_dbContext, geographyID);
        return Ok(meterLinkDisplayDtos);
    }

    [HttpGet("{meterID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Meter), "meterID")]
    [WithGeographyRolePermission(PermissionEnum.MeterRights, RightsEnum.Read)]
    public ActionResult<MeterGridDto> GetByID([FromRoute] int geographyID, [FromRoute] int meterID)
    {
        return Meters.GetByIDAsGridDto(_dbContext, meterID);
    }

    [HttpPut("{meterID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Meter), "meterID")]
    [WithGeographyRolePermission(PermissionEnum.MeterRights, RightsEnum.Create)]
    public async Task<ActionResult<MeterGridDto>> UpdateMeter([FromRoute] int geographyID, [FromRoute] int meterID, [FromBody] MeterGridDto meterGridDto)
    {
        var errors = await Meters.ValidateMeterUpsertAsync(_dbContext, geographyID, meterGridDto, meterID);
        if (errors.Any())
        {
            errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
            return BadRequest(ModelState);
        }

        var responseDto = Meters.UpdateMeter(_dbContext, meterID, meterGridDto);
        return Ok(responseDto);
    }
}