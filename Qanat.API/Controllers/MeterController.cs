using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class MeterController : SitkaController<MeterController>
{
    public MeterController(QanatDbContext dbContext, ILogger<MeterController> logger, IOptions<QanatConfiguration> qanatConfiguration) : base(
        dbContext, logger, qanatConfiguration)
    { }

    [HttpGet("geographies/{geographyID}/meters/dropdown-items")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult<List<MeterLinkDisplayDto>> ListUnassignedMeters([FromRoute] int geographyID)
    {
        var meterLinkDisplayDtos = Meters.ListAsLinkDisplayDtos(_dbContext, geographyID);
        return Ok(meterLinkDisplayDtos);
    }

    [HttpGet("geographies/{geographyID}/meters")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<List<MeterGridDto>> GetMeters([FromRoute] int geographyID)
    {
        return Meters.GetGeographyIDAsGridDto(_dbContext, geographyID);
    }

    [HttpGet("geographies/{geographyID}/meter/{meterID}")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<MeterGridDto> GetByID([FromRoute] int geographyID, [FromRoute] int meterID)
    {
        return Meters.GetByIDAsGridDto(_dbContext, meterID);
    }


    [HttpGet("meter-status")]
    [Authorize]
    public ActionResult<List<MeterStatusSimpleDto>> GetMeterStatus()
    {
        return MeterStatus.AllAsSimpleDto;
    }

    [HttpPost("geographies/{geographyID}/meter")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Create)]
    public ActionResult<MeterGridDto> AddMeter([FromRoute] int geographyID, [FromBody] MeterGridDto meterGridDto)
    {
        var responseDto = Meters.AddMeter(_dbContext, meterGridDto);
        return Ok(responseDto);
    }

    [HttpPost("geographies/{geographyID}/meter/{meterID}")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Create)]
    public ActionResult<MeterGridDto> UpdateMeter([FromRoute] int geographyID, [FromRoute] int meterID, [FromBody] MeterGridDto meterGridDto)
    {
        var responseDto = Meters.UpdateMeter(_dbContext, meterID, meterGridDto);
        return Ok(responseDto);
    }
}