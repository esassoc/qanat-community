using Microsoft.AspNetCore.Http;
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/statement-templates")]
public class StatementTemplateByGeographyController : SitkaController<StatementTemplateByGeographyController>
{
    private readonly StatementService _statementService;

    public StatementTemplateByGeographyController(QanatDbContext dbContext, ILogger<StatementTemplateByGeographyController> logger, IOptions<QanatConfiguration> qanatConfiguration, StatementService statementService)
        : base(dbContext, logger, qanatConfiguration)
    {
        _statementService = statementService;
    }

    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Create)]
    public async Task<ActionResult<StatementTemplateSimpleDto>> CreateStatementTemplate([FromRoute] int geographyID, [FromBody] StatementTemplateUpsertDto statementTemplateUpsertDto)
    {
        if (geographyID != statementTemplateUpsertDto.GeographyID)
        {
            return BadRequest();
        }

        var errors = StatementTemplates.ValidateStatementTemplate(_dbContext, statementTemplateUpsertDto);
        if (errors.Any())
        {
            errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
            return BadRequest(ModelState);
        }

        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var statementTemplateSimpleDto = await StatementTemplates.CreateStatementTemplate(_dbContext, currentUser.UserID, statementTemplateUpsertDto);
        return Ok(statementTemplateSimpleDto);
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Read)]
    public async Task<ActionResult<List<StatementTemplateDto>>> ListStatementTemplatesByGeographyID([FromRoute] int geographyID)
    {
        var statementTemplateSimpleDto = await StatementTemplates.ListByGeographyIDAsDto(_dbContext, geographyID);
        return Ok(statementTemplateSimpleDto);
    }
    
    [HttpGet("{statementTemplateID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(StatementTemplate), "statementTemplateID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Read)]
    public async Task<ActionResult<StatementTemplateDto>> GetStatementTemplateByID([FromRoute] int geographyID, [FromRoute] int statementTemplateID)
    {
        var statementTemplate = await StatementTemplates.GetByID(_dbContext, statementTemplateID);
        if (statementTemplate.GeographyID != geographyID)
        {
            return BadRequest();
        }

        var statementTemplateDto = statementTemplate?.AsDto();
        return Ok(statementTemplateDto);
    }

    [HttpPut("{statementTemplateID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(StatementTemplate), "statementTemplateID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Update)]
    public async Task<ActionResult<StatementTemplateSimpleDto>> UpdateStatementTemplate([FromRoute] int geographyID, [FromRoute] int statementTemplateID, [FromBody] StatementTemplateUpsertDto statementTemplateUpsertDto)
    {
        if (geographyID != statementTemplateUpsertDto.GeographyID)
        {
            return BadRequest();
        }

        var errors = StatementTemplates.ValidateStatementTemplate(_dbContext, statementTemplateUpsertDto, statementTemplateID);
        if (errors.Any())
        {
            errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
            return BadRequest(ModelState);
        }

        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var statementTemplateSimpleDto = await StatementTemplates.UpdateStatementTemplate(_dbContext, statementTemplateID, currentUser.UserID, statementTemplateUpsertDto);
        return Ok(statementTemplateSimpleDto);
    }

    [HttpPut("{statementTemplateID}/duplicate")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(StatementTemplate), "statementTemplateID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Update)]
    public async Task<ActionResult<StatementTemplateSimpleDto>> DuplicateStatementTemplate([FromRoute] int geographyID, [FromRoute] int statementTemplateID)
    {
        var statementTemplate = _dbContext.StatementTemplates.AsNoTracking().Single(x => x.StatementTemplateID == statementTemplateID);
        if (geographyID != statementTemplate.GeographyID)
        {
            return BadRequest();
        }

        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var statementTemplateSimpleDto = await StatementTemplates.DuplicateStatementTemplateByID(_dbContext, statementTemplateID, currentUser.UserID);
        return Ok(statementTemplateSimpleDto);
    }

    [HttpPut("pdf/preview")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Read)]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GenerateStatementTemplatePdf([FromRoute] int geographyID, [FromBody] StatementTemplatePdfPreviewRequestDto requestDto)
    {
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, requestDto.ReportingPeriodYear);
        var waterAccount = _dbContext.WaterAccounts.AsNoTracking().SingleOrDefault(x => x.WaterAccountID == requestDto.WaterAccountID);
        if (waterAccount == null || reportingPeriod == null)
        {
            return NotFound();
        }

        var dtoToSend = UsageStatementWaterAccounts.GetForPreviewByWaterAccountID(_dbContext, geographyID, reportingPeriod.ReportingPeriodID, requestDto).First();

        var pdf = await _statementService.GetStatementTemplatePreview(dtoToSend);

        Response.Headers.Append("Content-Disposition", $"attachment; filename=template.pdf");
        return File(pdf, "application/pdf");
    }
}