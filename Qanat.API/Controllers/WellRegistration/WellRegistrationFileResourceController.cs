using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class WellRegistrationFileResourceController(QanatDbContext dbContext, ILogger<WellRegistrationFileResourceController> logger, IOptions<QanatConfiguration> qanatConfiguration, FileService fileService)
    : SitkaController<WellRegistrationFileResourceController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost("well-registrations/{wellRegistrationID}/file-resources")]
    [WithWellRegistrationOwnerContextPermission]
    public async Task<ActionResult<List<WellRegistrationFileResourceDto>>> CreateWellFileResource([FromRoute] int wellRegistrationID, [FromForm] WellRegistrationFileResourceUpsertDto wellRegistrationFileResourceUpsertDto)
    {
        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var errors = FileResources.ValidateFileUpload(wellRegistrationFileResourceUpsertDto.File);
        if (!ModelState.IsValid || errors.Any())
        {
            errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
            return BadRequest(ModelState);
        }

        var fileResource = await fileService.CreateFileResource(_dbContext, wellRegistrationFileResourceUpsertDto.File, currentUser.UserID);

        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationFileResourceUpsertDto.WellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.AddAttachment(wellRegistrationFileResourceUpsertDto, fileResource);

        var wellRegistrationFileResources = WellRegistrationFileResources.ListByWellRegistrationID(_dbContext, wellRegistrationFileResourceUpsertDto.WellRegistrationID);

        return Ok(wellRegistrationFileResources);
    }

    [HttpPut("well-registrations/{wellRegistrationID}/file-resources/{wellRegistrationFileResourceID}")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult UpdateWellFileResource([FromRoute] int wellRegistrationID, [FromRoute] int wellRegistrationFileResourceID, [FromBody] WellRegistrationFileResourceUpdateDto wellRegistrationFileResourceUpdateDto)
    {
        if (!GetWellRegistrationFileResourceIfExists(wellRegistrationFileResourceID, out var wellRegistrationFileResource))
        {
            return BadRequest(ModelState);
        }

        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationFileResource.WellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.UpdateAttachment(wellRegistrationFileResourceUpdateDto);

        return Ok();
    }

    [HttpDelete("well-registrations/{wellRegistrationID}/file-resources/{wellRegistrationFileResourceID}")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult DeleteWellFileResource([FromRoute] int wellRegistrationID, [FromRoute] int wellRegistrationFileResourceID)
    {
        if (!GetWellRegistrationFileResourceIfExists(wellRegistrationFileResourceID, out var wellFileResource))
        {
            return BadRequest(ModelState);
        }

        fileService.DeleteFileStreamFromBlobStorage(wellFileResource.FileResource.FileResourceCanonicalName);

        WellRegistrationFileResources.Delete(_dbContext, wellFileResource);
        return Ok();
    }

    private bool GetWellRegistrationFileResourceIfExists(int wellRegistrationFileResourceID, out WellRegistrationFileResource wellRegistrationFileResource)
    {
        wellRegistrationFileResource = _dbContext.WellRegistrationFileResources.Include(x => x.FileResource)
            .SingleOrDefault(x => x.WellRegistrationFileResourceID == wellRegistrationFileResourceID);

        if (wellRegistrationFileResource != null)
        {
            return true;
        }

        ModelState.AddModelError("WellFileResource", $"WellFileResource with ID {wellRegistrationFileResourceID} does not exist.");
        return false;
    }
}