using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Qanat.API.Services;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services.Authorization;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]

public class WellRegistrationFileResourceController : SitkaController<WellRegistrationFileResourceController>
{
    private FileService _fileService { get; set; }

    public WellRegistrationFileResourceController(QanatDbContext dbContext, ILogger<WellRegistrationFileResourceController> logger, IOptions<QanatConfiguration> qanatConfiguration, FileService fileService) : base(dbContext, logger, qanatConfiguration)
    {
        _fileService = fileService;
    }

    [HttpPost("well-registrations/{wellRegistrationID}/file-resources")]
    [WithWellRegistrationOwnerContextPermission]
    public async Task<ActionResult<List<WellRegistrationFileResourceDto>>> CreateWellFileResource([FromRoute] int wellRegistrationID, [FromForm] WellRegistrationFileResourceUpsertDto wellRegistrationFileResourceUpsertDto)
    {
        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        ValidateFileUpload(wellRegistrationFileResourceUpsertDto.File);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var fileResource = await _fileService.CreateFileResource(_dbContext, wellRegistrationFileResourceUpsertDto.File, currentUser.UserID);

        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationFileResourceUpsertDto.WellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.AddAttachment(wellRegistrationFileResourceUpsertDto, fileResource);

        var wellRegistrationFileResources = WellRegistrationFileResources.ListByWellRegistrationID(_dbContext, wellRegistrationFileResourceUpsertDto.WellRegistrationID);

        return Ok(wellRegistrationFileResources);
    }

    private void ValidateFileUpload(IFormFile inputFile)
    {
        var acceptedExtensions = new List<string> { ".pdf", ".png", ".jpg", ".docx", ".doc", ".xlsx" };
        var extension = Path.GetExtension(inputFile.FileName);

        if (string.IsNullOrEmpty(extension) || !acceptedExtensions.Contains(extension.ToLower()))
        {
            ModelState.AddModelError("FileResource", $"{extension[1..].ToUpper()} is not an accepted file extension");
        }

        const double maxFileSize = 200d * 1024d * 1024d;
        if (inputFile.Length > maxFileSize)
        {
            ModelState.AddModelError("FileResource", "File size cannot exceed 200MB.");
        }
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

        _fileService.DeleteFileStreamFromBlobStorage(wellFileResource.FileResource.FileResourceCanonicalName);

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