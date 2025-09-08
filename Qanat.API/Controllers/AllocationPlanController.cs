using System.Collections.Generic;
using System.Linq;
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

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class AllocationPlanController : SitkaController<AllocationPlanController>
{
    public AllocationPlanController(QanatDbContext dbContext, ILogger<AllocationPlanController> logger, IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("geographies/{geographyID}/allocation-plan-configuration")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.AllocationPlanRights, RightsEnum.Read)]
    public ActionResult<GeographyAllocationPlanConfigurationDto> GetAllocationPlanConfigurationByGeographyID([FromRoute] int geographyID)
    {
        var geographyAllocationPlanConfigurationDto = GeographyAllocationPlanConfigurations.GetByGeographyIDAsConfigurationDto(_dbContext, geographyID);
        return Ok(geographyAllocationPlanConfigurationDto);
    }

    [HttpPost("geographies/{geographyID}/allocation-plan-configuration")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.AllocationPlanRights, RightsEnum.Create)]
    public ActionResult<GeographyAllocationPlanConfigurationDto> CreateGeographyAllocationPlanConfiguration([FromRoute] int geographyID, [FromBody] GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        var allocationPlan = GeographyAllocationPlanConfigurations.GetByGeographyIDAsConfigurationDto(_dbContext, geographyAllocationPlanConfigurationDto.GeographyID.Value);
        if (allocationPlan != null)
        {
            ModelState.AddModelError("Geography", "An Allocation Plan has already been created for this Geography.");
            return BadRequest(ModelState);
        }

        if (!ValidateConfiguration(geographyAllocationPlanConfigurationDto))
        {
            return BadRequest(ModelState);
        }

        var newGeographyAllocationPlanConfigurationDto = GeographyAllocationPlanConfigurations.Create(_dbContext, geographyAllocationPlanConfigurationDto);

        return Ok(newGeographyAllocationPlanConfigurationDto);
    }

    [HttpPut("geographies/{geographyID}/allocation-plan-configuration/preview")]
    [EntityNotFound(typeof(AllocationPlan), "allocationPlanID")]
    [WithGeographyRolePermission(PermissionEnum.AllocationPlanRights, RightsEnum.Read)]
    public ActionResult<List<AllocationPlanPreviewChangesDto>> PreviewGeographyAllocationPlanConfigurationUpdate([FromRoute]int geographyID, [FromBody] GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        var allocationPlanConfigurationPreviewDtos = GeographyAllocationPlanConfigurations.PreviewConfigurationUpdates(_dbContext, geographyID, geographyAllocationPlanConfigurationDto);

        return Ok(allocationPlanConfigurationPreviewDtos);
    }

    [HttpPut("geographies/{geographyID}/allocation-plan-configuration")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [EntityNotFound(typeof(AllocationPlan), "allocationPlanID")]
    [WithGeographyRolePermission(PermissionEnum.AllocationPlanRights, RightsEnum.Update)]
    public ActionResult<GeographyAllocationPlanConfigurationDto> UpdateAllocationPlanConfiguration([FromRoute] int geographyID, [FromBody] GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        if (!ValidateConfiguration(geographyAllocationPlanConfigurationDto))
        {
            return BadRequest(ModelState);
        }

        GeographyAllocationPlanConfigurations.Update(_dbContext, geographyID, geographyAllocationPlanConfigurationDto);
        AllocationPlans.MergeAllocationPlans(_dbContext, geographyID, geographyAllocationPlanConfigurationDto);

        var updatedAllocationPlan = GeographyAllocationPlanConfigurations.GetByGeographyID(_dbContext, geographyID).AsConfigurationDto();
        return Ok(updatedAllocationPlan);
    }

    private bool ValidateConfiguration(GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        var isValid = true;

        // check that StartYear is before EndYear
        if (geographyAllocationPlanConfigurationDto.StartYear >= geographyAllocationPlanConfigurationDto.EndYear)
        {
            ModelState.AddModelError("End Year", "Please choose an End Year that is greater than the specified Start Year.");
            isValid = false;
        }

        var existingWaterTypeIDs = _dbContext.WaterTypes.Where(x => geographyAllocationPlanConfigurationDto.WaterTypeIDs.Contains(x.WaterTypeID))
            .Select(x => x.WaterTypeID).ToList();

        // check that at least one Water Type was selected & all selected water types exist in the db
        if (!geographyAllocationPlanConfigurationDto.WaterTypeIDs.Any())
        {
            ModelState.AddModelError("Water Supply Types", "Please select at least one water supply type.");
            isValid = false;
        } 
        else if (geographyAllocationPlanConfigurationDto.WaterTypeIDs.Count != existingWaterTypeIDs.Count)
        {
            var invalidWaterTypeIDs = geographyAllocationPlanConfigurationDto.WaterTypeIDs.Where(x => !existingWaterTypeIDs.Contains(x)).ToList();

            ModelState.AddModelError("Water Supply Types", $"The following Water Type IDs are not valid: { string.Join(", ", invalidWaterTypeIDs) }");
            isValid = false;
        }

        // check that ZoneGroup exists & is within the same geography (just in case)
        var zoneGroup = _dbContext.ZoneGroups.AsNoTracking().SingleOrDefault(x => x.ZoneGroupID == geographyAllocationPlanConfigurationDto.ZoneGroupID.Value);
        if (zoneGroup == null)
        {
            ModelState.AddModelError("Zone Group", $"Zone Group with ID {geographyAllocationPlanConfigurationDto.ZoneGroupID} was not found.");
            isValid = false;
        }
        else if (zoneGroup.GeographyID != geographyAllocationPlanConfigurationDto.GeographyID)
        {
            ModelState.AddModelError("Zone Group", $"The geography for this Allocation Plan and the geography for the specified Zone Group do not match.");
            isValid = false;
        }

        return isValid;
    }

    [HttpPost("geographies/{geographyID}/allocation-plans/{allocationPlanID}")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.AllocationPlanRights, RightsEnum.Create)]
    [EntityNotFound(typeof(AllocationPlan), "allocationPlanID")]
    public ActionResult<AllocationPlanManageDto> CreateAllocationPlanPeriod([FromRoute] int geographyID, [FromRoute] int allocationPlanID, [FromBody] AllocationPlanPeriodUpsertDto upsertDto)
    {
        if (!ValidateAllocationPlanPeriod(upsertDto))
        {
            return BadRequest(ModelState);
        }

        var newAllocationPlanPeriod = AllocationPlanPeriods.CreateNewForAllocationPlan(_dbContext, allocationPlanID, upsertDto);
        AllocationPlans.MarkLastUpdated(_dbContext, allocationPlanID);
        var allocationPlan = AllocationPlans.GetAllocationPlanManageDto(_dbContext, newAllocationPlanPeriod.AllocationPlanID);
        return Ok(allocationPlan);
    }

    private bool ValidateAllocationPlanPeriod(AllocationPlanPeriodUpsertDto upsertDto)
    {
        var isValid = true;

        var otherAllocationPlansForPeriod = _dbContext.AllocationPlanPeriods
            .AsNoTracking()
            .Where(x => x.AllocationPlanID == upsertDto.AllocationPlanID && x.AllocationPlanPeriodID != upsertDto.AllocationPlanPeriodID).ToList();

        // make sure name is unique
        if (otherAllocationPlansForPeriod.Select(x => x.AllocationPeriodName).Contains(upsertDto.AllocationPeriodName))
        {
            ModelState.AddModelError("Allocation Period Name", "Allocation Period Names must be unique per Allocation Plan.");
            isValid = false;
        }

        return isValid;
    }

    [HttpPost("geographies/{geographyID}/allocation-plans/{copyToAllocationPlanID}/copy-from/{copyFromAllocationPlanID}")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.AllocationPlanRights, RightsEnum.Create)]
    [EntityNotFound(typeof(AllocationPlan), "copyToAllocationPlanID")]
    [EntityNotFound(typeof(AllocationPlan), "copyFromAllocationPlanID")]
    public ActionResult CopyAllocationPlan([FromRoute] int geographyID, [FromRoute] int copyToAllocationPlanID, [FromRoute] int copyFromAllocationPlanID)
    {
        AllocationPlans.CopyFromTo(_dbContext, copyFromAllocationPlanID, copyToAllocationPlanID);
        AllocationPlans.MarkLastUpdated(_dbContext, copyToAllocationPlanID);
        return Ok();
    }

    [HttpPut("geographies/{geographyID}/allocation-plans/{allocationPlanID}/{allocationPlanPeriodID}")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.AllocationPlanRights, RightsEnum.Delete)]
    [EntityNotFound(typeof(AllocationPlanPeriod), "allocationPlanPeriodID")]
    [EntityNotFound(typeof(AllocationPlan), "allocationPlanID")]
    public ActionResult UpdateAllocationPlanPeriod([FromRoute] int geographyID, [FromRoute] int allocationPlanID, [FromRoute] int allocationPlanPeriodID, [FromBody] AllocationPlanPeriodUpsertDto upsertDto)
    {
        if (!ValidateAllocationPlanPeriod(upsertDto))
        {
            return BadRequest(ModelState);
        }

        AllocationPlanPeriods.UpdateAllocationPlanPeriod(_dbContext, allocationPlanPeriodID, upsertDto);
        AllocationPlans.MarkLastUpdated(_dbContext, allocationPlanID);
        return Ok();
    }

    [HttpDelete("geographies/{geographyID}/allocation-plans/{allocationPlanID}/{allocationPlanPeriodID}")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.AllocationPlanRights, RightsEnum.Delete)]
    [EntityNotFound(typeof(AllocationPlanPeriod), "allocationPlanPeriodID")]
    [EntityNotFound(typeof(AllocationPlan), "allocationPlanID")]
    public ActionResult DeleteAllocationPlanPeriod([FromRoute] int geographyID, [FromRoute] int allocationPlanID, [FromRoute] int allocationPlanPeriodID)
    {
        AllocationPlanPeriods.DeleteAllocationPlanPeriod(_dbContext, allocationPlanPeriodID);
        AllocationPlans.MarkLastUpdated(_dbContext, allocationPlanID);
        return Ok();
    }

}