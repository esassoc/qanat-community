using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.API.Services;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/water-accounts/{waterAccountID}/water-measurement-self-reports/{selfReportID}/file-resources")]
public class WaterMeasurementSelfReportFileResourceController(QanatDbContext dbContext, ILogger<WaterMeasurementSelfReportFileResourceController> logger, IOptions<QanatConfiguration> qanatConfiguration, FileService fileService, UserDto callingUser)
    : SitkaController<WaterMeasurementSelfReportFileResourceController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReport), "selfReportID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)] //Water Account Holder
    public async Task<ActionResult<WaterMeasurementSelfReportFileResourceDto>> Create([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int selfReportID, [FromForm] WaterMeasurementSelfReportFileResourceCreateDto selfReportFileResourceUpsertDto)
    {
        var fileErrors = FileResources.ValidateFileUpload(selfReportFileResourceUpsertDto.File);

        var selfReport = await WaterMeasurementSelfReports.GetSingleAsSimpleDtoAsync(_dbContext, geographyID, waterAccountID, selfReportID);
        if (selfReport.WaterMeasurementSelfReportStatusID == SelfReportStatus.Approved.SelfReportStatusID)
        {
            return BadRequest("Cannot modify approved self reports.");
        }

        if (!ModelState.IsValid || fileErrors.Any())
        {
            fileErrors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
            return BadRequest(ModelState);
        }

        var fileResource = await fileService.CreateFileResource(_dbContext, selfReportFileResourceUpsertDto.File, callingUser.UserID);
        var newSelfReportFileResourceDto = await WaterMeasurementSelfReportFileResources.CreateAsync(_dbContext, selfReportID, fileResource.FileResourceID, selfReportFileResourceUpsertDto);
        return CreatedAtAction("Get", new { geographyID, waterAccountID, selfReportID, selfReportFileResourceID = newSelfReportFileResourceDto.WaterMeasurementSelfReportFileResourceID }, newSelfReportFileResourceDto);
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReport), "selfReportID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)] //Water Account Holder
    public async Task<ActionResult<List<WaterMeasurementSelfReportFileResourceDto>>> List([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int selfReportID)
    {
        var selfReportFileResourcesDtos = await WaterMeasurementSelfReportFileResources.ListAsync(_dbContext, selfReportID);
        return Ok(selfReportFileResourcesDtos);
    }

    [HttpPut("{selfReportFileResourceID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReport), "selfReportID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReportFileResource), "selfReportFileResourceID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)] //Water Account Holder
    public async Task<ActionResult<WaterMeasurementSelfReportFileResourceDto>> Update([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int selfReportID, [FromRoute] int selfReportFileResourceID, [FromBody] WaterMeasurementSelfReportFileResourceUpdateDto selfReportFileUpdateDto)
    {
        var selfReport = await WaterMeasurementSelfReports.GetSingleAsSimpleDtoAsync(_dbContext, geographyID, waterAccountID, selfReportID);
        if (selfReport.WaterMeasurementSelfReportStatusID == SelfReportStatus.Approved.SelfReportStatusID)
        {
            return BadRequest("Cannot modify approved self reports.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedSelfReportFileResourceDto = await WaterMeasurementSelfReportFileResources.UpdateAsync(_dbContext, selfReportFileResourceID, selfReportFileUpdateDto);
        return Ok(updatedSelfReportFileResourceDto);
    }

    [HttpDelete("{selfReportFileResourceID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReport), "selfReportID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReportFileResource), "selfReportFileResourceID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)] //Water Account Holder
    public async Task<ActionResult> Delete([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int selfReportID, [FromRoute] int selfReportFileResourceID)
    {
        var selfReport = await WaterMeasurementSelfReports.GetSingleAsSimpleDtoAsync(_dbContext, geographyID, waterAccountID, selfReportID);
        if (selfReport.WaterMeasurementSelfReportStatusID == SelfReportStatus.Approved.SelfReportStatusID)
        {
            return BadRequest("Cannot modify approved self reports.");
        }

        var selfReportFileResource = await WaterMeasurementSelfReportFileResources.GetAsync(_dbContext, selfReportFileResourceID);
        fileService.DeleteFileStreamFromBlobStorage(selfReportFileResource.FileResource.FileResourceCanonicalName);
        await WaterMeasurementSelfReportFileResources.DeleteAsync(_dbContext, selfReportFileResourceID);

        return NoContent();
    }
}