using Hangfire;
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
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/statement-batches/")]
public class StatementBatchByGeographyController : SitkaController<StatementBatchByGeographyController>
{
    private readonly FileService _fileService;
    private readonly StatementService _statementService;

    public StatementBatchByGeographyController(QanatDbContext dbContext,
        ILogger<StatementBatchByGeographyController> logger,
        IOptions<QanatConfiguration> qanatConfiguration, FileService fileService, StatementService statementService) : base(dbContext, logger,
        qanatConfiguration)
    {
        _fileService = fileService; 
        _statementService = statementService;
    }

    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Create)]
    public async Task<ActionResult<StatementBatchDto>> CreateStatementBatch([FromRoute] int geographyID, [FromBody] StatementBatchUpsertDto statementBatchUpsertDto)
    {
        var errors = StatementBatches.ValidateStatementBatch(_dbContext, geographyID, statementBatchUpsertDto);
        if (errors.Any())
        {
            errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
            return BadRequest(ModelState);
        }

        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var statementBatchDto = await StatementBatches.Create(_dbContext, geographyID, currentUser.UserID, statementBatchUpsertDto);

        BackgroundJob.Enqueue(() => _statementService.GenerateStatementBatchPdfsByStatementBatchID(statementBatchDto.StatementBatchID, currentUser.UserID));

        return Ok(statementBatchDto);
    }

   [HttpGet]
   [EntityNotFound(typeof(Geography), "geographyID")]
   [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Read)]
    public async Task<ActionResult<List<StatementBatchDto>>> ListStatementBatchesByGeographyID([FromRoute] int geographyID)
    {
        var statementBatchDtos = await StatementBatches.ListByGeographyIDAsDto(_dbContext, geographyID);
        return Ok(statementBatchDtos);
    }

   [HttpGet("{statementBatchID}")]
   [EntityNotFound(typeof(Geography), "geographyID")]
   [EntityNotFound(typeof(StatementBatch), "statementBatchID")]
   [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Read)]
    public async Task<ActionResult<StatementBatchDto>> GetStatementBatchByID([FromRoute] int geographyID, [FromRoute] int statementBatchID)
    {
        var statementBatchDto = await StatementBatches.GetByIDAsDto(_dbContext, statementBatchID);
        return Ok(statementBatchDto);
    }

   [HttpDelete("{statementBatchID}")]
   [EntityNotFound(typeof(Geography), "geographyID")]
   [EntityNotFound(typeof(StatementBatch), "statementBatchID")]
   [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Delete)]
    public async Task<ActionResult> DeleteStatementBatchByID([FromRoute] int geographyID, [FromRoute] int statementBatchID)
    {
        await _statementService.DeleteStatementBatchWaterAccountFileResources(statementBatchID);
        await StatementBatches.DeleteByID(_dbContext, statementBatchID);

        return Ok();
    }

    [HttpGet("{statementBatchID}/water-accounts")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(StatementBatch), "statementBatchID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Read)]
    public async Task<ActionResult<List<StatementBatchWaterAccountDto>>> ListStatementBatchWaterAccounts([FromRoute] int geographyID, [FromRoute] int statementBatchID)
    {
        var statementBatchWaterAccountDtos = await StatementBatchWaterAccounts.ListAsDto(_dbContext, statementBatchID);
        return Ok(statementBatchWaterAccountDtos);
    }

    [HttpPut("{statementBatchID}/generate-statements")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(StatementBatch), "statementBatchID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Update)]
    public async Task<ActionResult<List<StatementBatchWaterAccountDto>>> GenerateBatchStatements([FromRoute] int geographyID, [FromRoute] int statementBatchID)
    {
        var statementBatch = await _dbContext.StatementBatches.SingleOrDefaultAsync(x => x.StatementBatchID == statementBatchID);
        if (statementBatch == null)
        {
            return NotFound();
        }

        statementBatch.StatementsGenerated = false;

        // first delete existing files
        await _statementService.DeleteStatementBatchWaterAccountFileResources(statementBatchID);

        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        BackgroundJob.Enqueue(() => _statementService.GenerateStatementBatchPdfsByStatementBatchID(statementBatchID, currentUser.UserID));

        return Ok();
    }

    [HttpPut("{statementBatchID}/water-accounts/{waterAccountID}/generate-statements")]
    [EntityNotFound(typeof(StatementBatch), "statementBatchID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Update)]
    public async Task<ActionResult<StatementBatchWaterAccountDto>> GenerateWaterAccountStatement([FromRoute] int geographyID, [FromRoute] int statementBatchID, [FromRoute] int waterAccountID)
    {
        var statementBatchWaterAccount = await _dbContext.StatementBatchWaterAccounts
            .Include(x => x.FileResource)
            .SingleOrDefaultAsync(x => x.StatementBatchID == statementBatchID && x.WaterAccountID == waterAccountID);
        
        if (statementBatchWaterAccount == null)
        {
            return NotFound();
        }

        // first delete existing file if exists
        if (statementBatchWaterAccount.FileResourceID.HasValue)
        {
            var fileResource = statementBatchWaterAccount.FileResource;

            _fileService.DeleteFileStreamFromBlobStorage(fileResource.FileResourceCanonicalName);
            _dbContext.FileResources.Remove(fileResource);

            statementBatchWaterAccount.FileResource = null;
            await _dbContext.SaveChangesAsync();
        }

        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var dtosToSend = UsageStatementWaterAccounts.GetByWaterAccountIDAndStatementBatchID(_dbContext, waterAccountID, statementBatchID);
        await _statementService.GenerateStatementBatchPdfs(dtosToSend, statementBatchID, currentUser.UserID);

        var statementBatchWaterAccountDto = await StatementBatchWaterAccounts.GetByIDAsDto(_dbContext, statementBatchWaterAccount.StatementBatchWaterAccountID);
        return Ok(statementBatchWaterAccountDto);
    }

    [HttpGet("{statementBatchID}/download-statements")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(StatementBatch), "statementBatchID")]
    [WithGeographyRolePermission(PermissionEnum.StatementTemplateRights, RightsEnum.Read)]
    [SwaggerResponse(statusCode: 200, Type = typeof(FileStreamResult))]
    public async Task<ActionResult> DownloadStatements([FromRoute] int geographyID, [FromRoute] int statementBatchID)
    {
        var fileStream = await _statementService.GetStatementBatchFileResourcesAsZipFile(statementBatchID);
        return File(fileStream, "application/zip");
    }
}