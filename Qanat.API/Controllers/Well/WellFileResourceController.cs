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
[Route("wells/{wellID}/file-resources")]
public class WellFileResourceController(QanatDbContext dbContext, ILogger<WellFileResourceController> logger, IOptions<QanatConfiguration> qanatConfiguration, FileService fileService, UserDto callingUser)
    : SitkaController<WellFileResourceController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public async Task<ActionResult<List<WellFileResourceDto>>> CreateWellFileResource([FromRoute] int wellID, [FromForm] WellFileResourceCreateDto wellFileResourceCreateDto)
    {
        var errors = FileResources.ValidateFileUpload(wellFileResourceCreateDto.File);
        if (!ModelState.IsValid || errors.Any())
        {
            errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
            return BadRequest(ModelState);
        }

        var fileResource = await fileService.CreateFileResource(_dbContext, wellFileResourceCreateDto.File, callingUser.UserID);

        await WellFileResources.CreateAsync(_dbContext, wellID, fileResource.FileResourceID, wellFileResourceCreateDto);

        var wellFileResources = await WellFileResources.ListByWellIDAsync(_dbContext, wellID);
        return Ok(wellFileResources);
    }

    [HttpGet]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public async Task<ActionResult<List<WellFileResourceDto>>> List([FromRoute] int wellID)
    {
        var wellFileResources = await WellFileResources.ListByWellIDAsync(_dbContext, wellID);
        return Ok(wellFileResources);
    }

    [HttpPut("{wellFileResourceID}")]
    [EntityNotFound(typeof(Well), "wellID")]
    [EntityNotFound(typeof(WellFileResource), "wellFileResourceID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public async Task<ActionResult<WellFileResourceDto>> UpdateWellFileResource([FromRoute] int wellID, [FromRoute] int wellFileResourceID, [FromBody] WellFileResourceUpdateDto wellFileResourceUpdateDto)
    {
        var updatedWelLFileResource = await WellFileResources.UpdateAsync(_dbContext, wellFileResourceID, wellFileResourceUpdateDto);
        return Ok(updatedWelLFileResource);
    }

    [HttpDelete("{wellFileResourceID}")]
    [EntityNotFound(typeof(Well), "wellID")]
    [EntityNotFound(typeof(WellFileResource), "wellFileResourceID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public async Task<ActionResult> DeleteWellFileResource([FromRoute] int wellID, [FromRoute] int wellFileResourceID)
    {
        var wellFileResource = await WellFileResources.GetByIDAsDtoAsync(_dbContext, wellFileResourceID);

        fileService.DeleteFileStreamFromBlobStorage(wellFileResource.FileResource.FileResourceCanonicalName);
        await WellFileResources.DeleteAsync(_dbContext, wellFileResourceID);

        return NoContent();
    }
}