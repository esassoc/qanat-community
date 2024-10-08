using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.EFModels.Workflows;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;
using System.Linq;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class WellRegistrationController : SitkaController<WellRegistrationController>
{
    public WellRegistrationController(QanatDbContext dbContext, ILogger<WellRegistrationController> logger, IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    { }

    [HttpGet("geographies/{geographyID}/well-registrations")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<List<WellRegistrationMinimalDto>> ListWells([FromRoute] int geographyID)
    {
        var wellRegistrationMinimalDtos = WellRegistrations.ListByGeographyID(_dbContext, geographyID);
        return Ok(wellRegistrationMinimalDtos);
    }

    [HttpGet("geographies/{geographyID}/well-registrations/grid-rows")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<List<WellRegistrationGridRowDto>> ListWellRegistrationGridRows([FromRoute] int geographyID)
    {
        var wellRegistrations = WellRegistrations.ListByGeographyID(_dbContext, geographyID);
        return Ok(wellRegistrations);
    }

    [HttpGet("well-registrations/{wellRegistrationID}")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<WellRegistrationDetailedDto> GetWellRegistrationDetailsByID([FromRoute] int wellRegistrationID)
    {
        var wellRegistrationDetailedDto = WellRegistrations.GetByIDAsDetailedDto(_dbContext, wellRegistrationID);
        return Ok(wellRegistrationDetailedDto);
    }

    [HttpGet("geographies/{geographyID}/well-registrations/submitted")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)] // because people viewing these will need to review them
    public ActionResult<List<SubmittedWellRegistrationListItemDto>> ListSubmittedWellRegistrations([FromRoute] int geographyID)
    {
        var wellRegistrations = WellRegistrations.ListSubmittedWellsByGeographyID(_dbContext, geographyID);
        return Ok(wellRegistrations);
    }

    #region WellRegistryWorkflow

    [HttpGet("well-registrations/water-use-types")]
    [WithRoleFlag(FlagEnum.CanRegisterWells)]
    public ActionResult<List<WellRegistrationWaterUseTypeSimpleDto>> ListWellWaterUseTypes()
    {
        var dtos = WellRegistrationWaterUseType.AllAsSimpleDto;
        return dtos;
    }

    [HttpGet("well-registrations/pump-fuel-types")]
    [WithRoleFlag(FlagEnum.CanRegisterWells)]
    public ActionResult<List<FuelTypeSimpleDto>> ListPumpFuelTypes()
    {
        var dtos = FuelType.AllAsSimpleDto;
        return Ok(dtos);
    }

    [HttpPost("geographies/{geographyID}/well-registrations")]
    [WithRoleFlag(FlagEnum.CanRegisterWells)]
    public ActionResult<WellRegistrationMinimalDto> CreateWellRegistration([FromRoute] int geographyID, [FromBody] BeginWellRegistryRequestDto requestDto)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        GeographyUsers.AddGeographyNormalUserIfAbsent(_dbContext, user.UserID, geographyID);

        var newWellRegistration = new WellRegistration()
        {
            GeographyID = geographyID,
            CreateUserID = user.UserID,
            CreateUserGuid = user.UserGuid,
            WellRegistrationStatusID = WellRegistrationStatus.Draft.WellRegistrationStatusID,
            ParcelID = requestDto.ParcelID,
            WellRegistrationIrrigatedParcels = requestDto.ParcelID != null ? 
                new List<WellRegistrationIrrigatedParcel>(){ new(){ ParcelID = (int)requestDto.ParcelID } } : null,
        };

        var newWellWorkflow = newWellRegistration.GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        newWellWorkflow.Create();

        return Ok(WellRegistrations.GetByID(_dbContext, newWellWorkflow.GetWellRegistrationID()).AsMinimalDto());
    }

    [HttpPut("well-registrations/{wellRegistrationID}/parcel")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult UpdateWellRegistrationParcel([FromRoute] int wellRegistrationID, [FromBody] int? parcelID)
    {
        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.UpdateSelectedParcel(parcelID);
        return Ok();
    }

    [HttpPut("well-registrations/{wellRegistrationID}/location")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult UpdateLocation([FromRoute] int wellRegistrationID, [FromBody] WellRegistrationLocationDto wellRegistrationLocationDto)
    {
        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.UpdateWellRegistrationLocation(wellRegistrationLocationDto);
        return Ok();
    }

    [HttpGet("well-registrations/{wellRegistrationID}/location/confirm")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult<ConfirmWellRegistrationLocationDto> GetConfirmLocation([FromRoute] int wellRegistrationID)
    {
        var dto = WellRegistrations.GetConfirmWellRegistrationLocationDto(_dbContext, wellRegistrationID);
        return dto;
    }

    [HttpPut("well-registrations/{wellRegistrationID}/location/confirm")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult ConfirmLocation([FromRoute] int wellRegistrationID, [FromBody] ConfirmWellRegistrationLocationDto dto)
    {
        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.ConfirmWellRegistrationLocation(dto);
        return Ok();
    }

    [HttpGet("well-registrations/{wellRegistrationID}/irrigated-parcels")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult<WellRegistrationIrrigatedParcelsResponseDto> GetWellRegistrationIrrigatedParcels([FromRoute] int wellRegistrationID)
    {
        var dto = WellRegistrations.GetWellRegistrationIrrigatedParcelsDto(_dbContext, wellRegistrationID);
        return dto;
    }

    [HttpPut("well-registrations/{wellRegistrationID}/irrigated-parcels")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult UpdateWellIrrigatedParcels([FromRoute] int wellRegistrationID, [FromBody] WellRegistrationIrrigatedParcelsRequestDto dto)
    {
        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.UpdateIrrigatedParcels(dto);
        return Ok();
    }

    [HttpPost("well-registrations/{wellRegistrationID}/contacts")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult<bool> AddWellRegistrationContact([FromRoute] int wellRegistrationID, [FromBody] WellRegistrationContactsUpsertDto wellRegistrationContactsUpsertDto)
    {
        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.UpdateWellRegistrationContacts(wellRegistrationContactsUpsertDto);

        return Ok();
    }

    [HttpPut("well-registrations/{wellRegistrationID}/basic-info")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult<WellRegistrationBasicInfoFormDto> UpdateBasicInfo([FromRoute] int wellRegistrationID, [FromBody] WellRegistrationBasicInfoFormDto basicInfoFormDto)
    {
        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.UpdateBasicInfo(basicInfoFormDto);

        var updatedBasicInfo = WellRegistrations.GetWellRegistrationBasicInfoDto(_dbContext, wellRegistrationID);
        return Ok(updatedBasicInfo);
    }

    [HttpGet("well-registrations/{wellRegistrationID}/basic-info")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult<WellRegistrationBasicInfoFormDto> GetBasicInfo([FromRoute] int wellRegistrationID)
    {
        var basicInfoDto = WellRegistrations.GetWellRegistrationBasicInfoDto(_dbContext, wellRegistrationID);
        return basicInfoDto;
    }

    [HttpPut("well-registrations/{wellRegistrationID}/supporting-info")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult<WellRegistrySupportingInfoDto> UpdateSupportingInfo([FromRoute] int wellRegistrationID, [FromBody] WellRegistrySupportingInfoDto supportingInfoDto)
    {
        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        wellWorkflow.UpdateSupportingInfo(supportingInfoDto);

        var updatedBasicInfo = WellRegistrations.GetWellRegistrySupportingInfoDto(_dbContext, wellRegistrationID);
        return updatedBasicInfo;
    }

    [HttpGet("well-registrations/{wellRegistrationID}/supporting-info")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult<WellRegistrySupportingInfoDto> GetSupportingInfo([FromRoute] int wellRegistrationID)
    {
        return WellRegistrations.GetWellRegistrySupportingInfoDto(_dbContext, wellRegistrationID);
    }

    [HttpPost("well-registrations/{wellRegistrationID}/submit-well")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult SubmitWelRegistrationl([FromRoute] int wellRegistrationID)
    {
        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        
        wellWorkflow.Submit();

        return Ok();
    }

    [HttpDelete("well-registrations/{wellRegistrationID}")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult DeleteWellRegistration([FromRoute] int wellRegistrationID)
    {
        var wellWorkflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));

        wellWorkflow.Delete();

        return Ok();
    }

    [HttpGet("well-registrations/{wellRegistrationID}/progress")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult<WellRegistryWorkflowProgress.WellRegistryWorkflowProgressDto> GetWellRegistrationProgress([FromRoute] int wellRegistrationID)
    {
        var wellWorkflowProgressDto = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext)).GetProgressDto();

        return wellWorkflowProgressDto;
    }

    [HttpPost("well-registrations/{wellRegistrationID}/approve")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithGeographyRoleFlag(FlagEnum.CanReviewWells)]
    public ActionResult ApproveWellRegistration([FromRoute] int wellRegistrationID)
    {
        var workflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        workflow.Approve();

        return Ok();
    }

    [HttpPost("well-registrations/{wellRegistrationID}/return")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithGeographyRoleFlag(FlagEnum.CanReviewWells)]
    public ActionResult ReturnWellRegistration([FromRoute] int wellRegistrationID)
    {
        var workflow = WellRegistrations.GetByIDWithTrackingForWorkflow(_dbContext, wellRegistrationID)
            .GetWorkflow(_dbContext, UserContext.GetUserFromHttpContext(_dbContext, HttpContext));
        workflow.Return();

        return Ok();
    }

    #endregion


    [HttpGet("well-registrations/{wellRegistrationID}/parcel")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithWellRegistrationOwnerContextPermission]
    public ActionResult<ParcelDisplayDto> GetParcelByWellRegistrationID([FromRoute] int wellRegistrationID)
    {
        var wellRegistration = WellRegistrations.GetByID(_dbContext, wellRegistrationID);
        if (!wellRegistration.ParcelID.HasValue)
        {
            return Ok();
        }

        var parcelDisplayDto = Parcels.GetByIDAsDisplayDto(_dbContext, wellRegistration.ParcelID.Value);
        return Ok(parcelDisplayDto);
    }

    [HttpGet("well-registrations/{wellRegistrationID}/location")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<WellRegistrationLocationDto> GetLocationByWellRegistrationID([FromRoute] int wellRegistrationID)
    {
        var wellRegistration = WellRegistrations.GetByID(_dbContext, wellRegistrationID);
        var wellLocationDto = wellRegistration.AsLocationDto(true);
        return Ok(wellLocationDto);
    }


    [HttpGet("well-registrations/{wellRegistrationID}/file-resources")]
    [EntityNotFound(typeof(WellRegistration), "wellRegistrationID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<List<WellRegistrationFileResourceDto>> ListWellRegistrationFileResourcesByWellRegistrationID([FromRoute] int wellRegistrationID)
    {
        var wellRegistrationFileResourceSimpleDtos = WellRegistrationFileResources.ListByWellRegistrationID(_dbContext, wellRegistrationID);
        return Ok(wellRegistrationFileResourceSimpleDtos);
    }
    
    [HttpGet("well-registrations/{wellRegistrationID}/contacts")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<WellRegistrationContactsUpsertDto> GetWellRegistrationContactsUpsertDto([FromRoute] int wellRegistrationID)
    {
        var wellRegistrationContacts = _dbContext.WellRegistrationContacts.AsNoTracking()
            .Where(x => x.WellRegistrationID == wellRegistrationID).ToList();
        
        var wellRegistrationContactsUpsertDto = new WellRegistrationContactsUpsertDto();
        var landowner = wellRegistrationContacts
            .SingleOrDefault(x => x.WellRegistrationContactTypeID == (int)WellRegistrationContactTypeEnum.Landowner);
        if (landowner != null)
        {
            wellRegistrationContactsUpsertDto.LandownerWellRegistrationContactID = landowner.WellRegistrationContactID;
            wellRegistrationContactsUpsertDto.LandownerWellRegistrationID = landowner.WellRegistrationID;
            wellRegistrationContactsUpsertDto.LandownerWellRegistrationContactTypeID = landowner.WellRegistrationContactTypeID;
            wellRegistrationContactsUpsertDto.LandownerContactName = landowner.ContactName;
            wellRegistrationContactsUpsertDto.LandownerBusinessName = landowner.BusinessName;
            wellRegistrationContactsUpsertDto.LandownerStreetAddress = landowner.StreetAddress;
            wellRegistrationContactsUpsertDto.LandownerCity = landowner.City;
            wellRegistrationContactsUpsertDto.LandownerStateID = landowner.StateID;
            wellRegistrationContactsUpsertDto.LandownerZipCode = landowner.ZipCode;
            wellRegistrationContactsUpsertDto.LandownerPhone = landowner.Phone;
            wellRegistrationContactsUpsertDto.LandownerEmail = landowner.Email;
        }
        var ownerOperator = wellRegistrationContacts
            .SingleOrDefault(x => x.WellRegistrationContactTypeID == (int)WellRegistrationContactTypeEnum.OwnerOperator);
        if (ownerOperator != null)
        {
            wellRegistrationContactsUpsertDto.OwnerOperatorWellRegistrationContactID = ownerOperator.WellRegistrationContactID;
            wellRegistrationContactsUpsertDto.OwnerOperatorWellRegistrationID = ownerOperator.WellRegistrationID;
            wellRegistrationContactsUpsertDto.OwnerOperatorWellRegistrationContactTypeID = ownerOperator.WellRegistrationContactTypeID;
            wellRegistrationContactsUpsertDto.OwnerOperatorContactName = ownerOperator.ContactName;
            wellRegistrationContactsUpsertDto.OwnerOperatorBusinessName = ownerOperator.BusinessName;
            wellRegistrationContactsUpsertDto.OwnerOperatorStreetAddress = ownerOperator.StreetAddress;
            wellRegistrationContactsUpsertDto.OwnerOperatorCity = ownerOperator.City;
            wellRegistrationContactsUpsertDto.OwnerOperatorStateID = ownerOperator.StateID;
            wellRegistrationContactsUpsertDto.OwnerOperatorZipCode = ownerOperator.ZipCode;
            wellRegistrationContactsUpsertDto.OwnerOperatorPhone = ownerOperator.Phone;
            wellRegistrationContactsUpsertDto.OwnerOperatorEmail = ownerOperator.Email;
        }

        var isLandownerSameAsOwnerOperator = true;
        if (ownerOperator != null && landowner != null)
        {
            isLandownerSameAsOwnerOperator = WellRegistrations.IsLandownerSameAsOwnerOperator(landowner.AsSimpleDto(), ownerOperator.AsSimpleDto());
        }
        wellRegistrationContactsUpsertDto.LandownerSameAsOwnerOperator = isLandownerSameAsOwnerOperator;
        return Ok(wellRegistrationContactsUpsertDto);
    }
}