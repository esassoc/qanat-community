using System.Collections.Generic;
using Qanat.API.Services;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("parcels")]
public class ParcelController : SitkaController<ParcelController>
{
    private UserDto _callingUser;
    public ParcelController(QanatDbContext dbContext, ILogger<ParcelController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
        : base(dbContext, logger, qanatConfiguration)
    {
        _callingUser = callingUser;
    }

    #region CRUD

    [HttpGet("{parcelID}")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<ParcelMinimalDto> GetByID([FromRoute] int parcelID)
    {
        var parcelMinimalDto = Parcels.GetByID(_dbContext, parcelID).AsParcelMinimalDto();
        return RequireNotNullLogIfNotFound(parcelMinimalDto, "ParcelMinimalDto", parcelID);
    }

    #endregion

    [HttpPut("review")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public async Task<ActionResult> MarkParcelAsReviewed([FromBody] List<int> parcelIDs)
    {
        await ParcelHistories.MarkAsReviewedByParcelIDs(_dbContext, parcelIDs);
        return Ok();
    }

    [HttpGet("search/{searchString}")]
    [WithRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<ParcelDisplayDto>> SearchByParcelNumber([FromRoute] string searchString)
    {
        var parcelDisplayDtos = Parcels.SearchParcelNumber(_dbContext, searchString);
        return Ok(parcelDisplayDtos);
    }

    [HttpPost("boundingBox")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<BoundingBoxDto> GetBoundingBoxByParcelIDs([FromBody] List<int> parcelIDs)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var boundingBoxDto = Parcels.GetBoundingBoxByParcelIDs(_dbContext, parcelIDs);
        return Ok(boundingBoxDto);
    }

    [HttpGet("{parcelID}/map-popup")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<ParcelPopupDto> GetParcelPopupDtoByID([FromRoute] int parcelID)
    {
        var parcelPopupDto = Parcels.GetParcelPopupDtoByID(_dbContext, parcelID);

        return RequireNotNullLogIfNotFound(parcelPopupDto, "ParcelPopupDto", parcelID);
    }

    [HttpGet("{parcelID}/zones")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<ParcelDetailDto> GetParcelWithZonesDtoByID([FromRoute] int parcelID)
    {
        var parcelWithZonesDto = Parcels.GetByID(_dbContext, parcelID).AsDetailDto();
        return Ok(parcelWithZonesDto);
    }

    [HttpGet("{parcelID}/getSupplyEntries")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<ParcelSupplyDetailDto>> GetAllSupplyEntriesByParcelID([FromRoute] int parcelID)
    {
        var parcelSupplyDetailDtos = ParcelSupplies.ListByParcelIDAsDetailDto(_dbContext, parcelID);
        return Ok(parcelSupplyDetailDtos);
    }

    [HttpGet("{parcelID}/waterAccountParcels")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<WaterAccountParcelDto>> GetWaterAccountParcelsByParcelID([FromRoute] int parcelID)
    {
        var waterAccountParcelDtos = _dbContext.WaterAccountParcels.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Where(x => x.ParcelID == parcelID)
            .OrderBy(x => x.EndYear != null).ThenByDescending(x => x.EndYear)
            .Select(x => x.AsWaterAccountParcelDto()).ToList();

        return Ok(waterAccountParcelDtos);
    }

    [HttpGet("{parcelID}/history")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<ParcelHistoryDto>> GetHistory([FromRoute] int parcelID)
    {
        var parcelHistoryDtos = _dbContext.ParcelHistories.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Include(x => x.UpdateUser)
            .Where(x => x.ParcelID == parcelID).ToList()
            .Select(x => new ParcelHistoryDto()
            {
                ParcelHistoryID = x.ParcelHistoryID,
                GeographyID = x.GeographyID,
                ParcelID = x.ParcelID,
                EffectiveYear = x.EffectiveYear,
                UpdateDate = x.UpdateDate,
                UpdateUserID = x.UpdateUserID,
                UpdateUserFullName = x.UpdateUser.FullName,
                ParcelArea = x.ParcelArea,
                OwnerName = x.OwnerName,
                OwnerAddress = x.OwnerAddress,
                ParcelStatusID = x.ParcelStatusID,
                IsReviewed = x.IsReviewed,
                IsManualOverride = x.IsManualOverride,
                ReviewDate = x.ReviewDate,
                WaterAccountID = x.WaterAccountID,
                WaterAccount = x.WaterAccount?.AsDisplayDto(),
                ParcelStatus = x.ParcelStatus.AsSimpleDto()

            }).ToList()
            .OrderByDescending(x => x.UpdateDate);

        return Ok(parcelHistoryDtos);
    }

    [HttpGet("{parcelID}/wells")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<WellLocationDto>> GetWellLocationsForParcel([FromRoute] int parcelID)
    {
        var wells = _dbContext.Wells
            .Include(x => x.Parcel).ThenInclude(x => x.ParcelGeometry)
            .Where(x => x.ParcelID == parcelID)
            .Select(x => x.AsLocationDto(true)).ToList();
        return Ok(wells);
    }

    [HttpPost("{parcelID}/update-water-account/{waterAccountID}")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    public async Task<ActionResult> UpdateParcelWaterAccount([FromRoute] int parcelID, [FromRoute] int waterAccountID, [FromBody] int effectiveYear)
    {
        // take advantage of the update water accounts method that handles everything as a pinch point for updating parcel relationships.
        var updatedParcelIDsForWaterAccount = _dbContext.WaterAccountParcels.AsNoTracking()
            .Where(x => x.WaterAccountID == waterAccountID).Select(x => x.ParcelID).ToList();

        updatedParcelIDsForWaterAccount.Add(parcelID);
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        await WaterAccounts.UpdateWaterAccountParcels(_dbContext, waterAccountID, effectiveYear, updatedParcelIDsForWaterAccount, user.UserID);
        return Ok();
    }

    [HttpPost("{parcelID}/edit-zone-assignments")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    public async Task<ActionResult> EditParcelZoneAssignments([FromRoute] int parcelID, [FromBody] ParcelZoneAssignmentFormDto zoneAssignmentForm)
    {
        await Parcels.UpdateParcelZoneAssignments(_dbContext, zoneAssignmentForm);
        return Ok();
    }

    [HttpPut("{parcelID}/ownership")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public async Task<ActionResult> UpdateParcelOwnership([FromRoute] int parcelID, [FromBody] ParcelUpdateOwnershipRequestDto ownershipRequestDto)
    {
        var errors = Parcels.ValidateUpdate(_dbContext, ownershipRequestDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        await Parcels.UpdateOwnership(_dbContext, parcelID, ownershipRequestDto, user.UserID);
        return Ok();
    }

    //MK 11/25/2024 -- This might be more RESTful to be a patch, but I don't think we've got patches patterned out well.
    [HttpPut("{parcelID}/acres")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)] //Water Managers have the ability to update acres
    public async Task<ActionResult<ParcelMinimalDto>> UpdateParcelAcres([FromRoute] int parcelID, [FromBody] ParcelAcreUpdateDto parcelUpdateAcresDto)
    {
        var errors = Parcels.ValidateAcresUpdate(_dbContext, parcelID, parcelUpdateAcresDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedParcel = await Parcels.UpdateAcres(_dbContext, parcelID, parcelUpdateAcresDto, _callingUser);
        return Ok(updatedParcel);
    }
}