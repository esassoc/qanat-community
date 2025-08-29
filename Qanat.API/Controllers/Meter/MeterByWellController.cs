using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("wells/{wellID}/meters")]
public class MeterByWellController(QanatDbContext dbContext, ILogger<MeterByWellController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<MeterByWellController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult<MeterGridDto> AddWellMeter([FromRoute] int wellID, [FromBody] AddWellMeterRequestDto requestDto)
    {
        var errors = WellMeters.ValidateAddWellMeter(_dbContext, requestDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        WellMeters.AddWellMeter(_dbContext, requestDto);
        var meterDto = Meters.GetByIDAsGridDto(_dbContext, requestDto.MeterID);

        return Ok(meterDto);
    }

    [HttpGet("current")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public async Task<ActionResult<WellMeterDto>> GetWellMeterByWellID([FromRoute] int wellID)
    {
        var wellMeterDto = await Meters.GetCurrentWellMeterByWellIDAsDtoAsync(_dbContext, wellID);
        if (wellMeterDto == null)
        {
            return NotFound();
        }

        return Ok(wellMeterDto);
    }

    [HttpPut("{meterID}/remove")]
    [EntityNotFound(typeof(Well), "wellID")]
    [EntityNotFound(typeof(Meter), "meterID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult<MeterGridDto> RemoveWellMeter([FromRoute] int wellID, [FromRoute] int meterID, [FromBody] RemoveWellMeterRequestDto requestDto)
    {
        var wellMeter = _dbContext.WellMeters.SingleOrDefault(x => x.WellID == requestDto.WellID && x.MeterID == requestDto.MeterID && !x.EndDate.HasValue);

        var errors = WellMeters.ValidateRemoveWellMeter(wellMeter, requestDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        WellMeters.RemoveWellMeter(_dbContext, wellMeter, requestDto);

        return Ok();
    }
}