using System.Collections.Generic;
using Qanat.API.Services;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Services.GDAL;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class ParcelController : SitkaController<ParcelController>
{
    private readonly FileService _fileService;
    private readonly GDALAPIService _gdalApiService;
    private readonly HierarchyContext _hierarchyContext;

    public ParcelController(QanatDbContext dbContext, ILogger<ParcelController> logger,
        IOptions<QanatConfiguration> qanatConfiguration, FileService fileService, GDALAPIService gdalApiService, HierarchyContext hierarchyContext)
        : base(dbContext, logger, qanatConfiguration)
    {
        _fileService = fileService;
        _gdalApiService = gdalApiService;
        _hierarchyContext = hierarchyContext;
    }

    [HttpPut("parcels/review")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public async Task<ActionResult> MarkParcelAsReviewed([FromBody] List<int> parcelIDs)
    {
        await ParcelHistories.MarkAsReviewedByParcelIDs(_dbContext, parcelIDs);
        return Ok();
    }


    [HttpGet("parcels/search/{searchString}")]
    [WithRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<ParcelDisplayDto>> SearchByParcelNumber([FromRoute] string searchString)
    {
        var parcelDisplayDtos = Parcels.SearchParcelNumber(_dbContext, searchString);
        return Ok(parcelDisplayDtos);
    }

    [HttpPost("parcels/boundingBox")]
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

    [HttpGet("parcels/{parcelID}")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<ParcelMinimalDto> GetByID([FromRoute] int parcelID)
    {
        var parcelMinimalDto = Parcels.GetByID(_dbContext, parcelID).AsParcelMinimalDto();
        return RequireNotNullThrowNotFound(parcelMinimalDto, "ParcelMinimalDto", parcelID);
    }

    [HttpGet("parcels/{parcelID}/map-popup")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<ParcelPopupDto> GetParcelPopupDtoByID([FromRoute] int parcelID)
    {
        var parcelPopupDto = Parcels.GetParcelPopupDtoByID(_dbContext, parcelID);

        return RequireNotNullThrowNotFound(parcelPopupDto, "ParcelPopupDto", parcelID);
    }

    [HttpGet("parcels/{parcelID}/zones")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<ParcelDetailDto> GetParcelWithZonesDtoByID([FromRoute] int parcelID)
    {
        var parcelWithZonesDto = Parcels.GetByID(_dbContext, parcelID).AsDetailDto();
        return Ok(parcelWithZonesDto);
    }

    [HttpGet("geographies/{geographyID}/parcels/current-user")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<List<ParcelIndexGridDto>> ListByGeographyIDForCurrentUser([FromRoute] int geographyID)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var hasPermission = new WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read).HasPermission(user, _hierarchyContext);
        var  parcelIndexGridDtos = hasPermission 
            ? Parcels.ListByGeographyIDAsIndexGridDtos(_dbContext, geographyID) 
            : Parcels.ListByGeographyIDAndUserIDAsIndexGridDtos(_dbContext, geographyID, user.UserID);

        return Ok(parcelIndexGridDtos);
    }

    [HttpGet("parcels/{parcelID}/getSupplyEntries")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<ParcelSupplyDetailDto>> GetAllSupplyEntriesByParcelID([FromRoute] int parcelID)
    {
        var parcelSupplyDetailDtos = ParcelSupplies.ListByParcelIDAsDetailDto(_dbContext, parcelID);
        return Ok(parcelSupplyDetailDtos);
    }

    [HttpGet("parcels/{parcelID}/waterAccountParcels")]
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

    [HttpGet("parcels/{parcelID}/history")]
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

    [HttpGet("parcels/{parcelID}/wells")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<WellLocationDto>> GetWellLocationsForParcel([FromRoute] int parcelID)
    {
        var wells = _dbContext.Wells
            .Include(x => x.Parcel)
                .ThenInclude(x => x.ParcelGeometry)
            .Where(x => x.ParcelID == parcelID)
            .Select(x => x.AsLocationDto(true)).ToList();
        return Ok(wells);
    }

    [HttpPost("geographies/{geographyID}/parcels/water-account/start-year")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<int>> GetYearsForParcels([FromRoute] int geographyID, [FromBody] List<int> parcelIDs)
    {
        var years = Parcels.ListWaterAccountParcelEffectiveYearsByParcelIDs(_dbContext, geographyID, parcelIDs);
        return Ok(years);
    }

    [HttpPost("parcels/{parcelID}/update-water-account/{waterAccountID}")]
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

    [HttpPost("parcels/{parcelID}/edit-zone-assignments")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    public async Task<ActionResult> EditParcelZoneAssignments([FromRoute] int parcelID, [FromBody] ParcelZoneAssignmentFormDto zoneAssignmentForm)
    {
        await Parcels.UpdateParcelZoneAssignments(_dbContext, zoneAssignmentForm);
        return Ok();
    }

    [HttpPut("parcels/{parcelID}/ownership")]
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

    [HttpGet("geographies/{geographyID}/parcels")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<ParcelIndexGridDto>> List([FromRoute] int geographyID)
    {
        var parcelIndexGridDtos = _dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsIndexGridDto()).ToList();

        return Ok(parcelIndexGridDtos);
    }

    [HttpGet("geographies/{geographyID}/parcels/search/{searchString}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanRegisterWells)]
    public ActionResult<List<ParcelDisplayDto>> SearchParcelsByGeographyID([FromRoute] int geographyID, [FromRoute] string searchString)
    {
        var parcelDisplayDtos = Parcels.SearchParcelNumberWithinGeography(_dbContext, geographyID, searchString);
        return Ok(parcelDisplayDtos);
    }

    [HttpPost("geographies/{geographyID}/parcels/geojson")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<ParcelWithGeoJSONDto>> GetParcelGeoJsons([FromRoute] int geographyID, [FromBody] List<int> parcelIDs)
    {
        var parcelsWithGeoJSON = Parcels.GetParcelsWithGeoJSONDtos(_dbContext, parcelIDs);
        return Ok(parcelsWithGeoJSON);
    }

    [HttpPost("geographies/{geographyID}/parcels/latest-effective-year")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<int> GetLatestEffectiveYearForParcels([FromRoute] int geographyID, [FromBody] List<int> parcelIDs)
    {
        var latestEffectiveYear = _dbContext.WaterAccountParcels
            .Where(x => x.GeographyID == geographyID && parcelIDs.Contains(x.ParcelID)).ToList()
            .MaxBy(x => x.EffectiveYear)?.EffectiveYear;

        return Ok(latestEffectiveYear);
    }

    [HttpGet("geographies/{geographyID}/parcels-water-supply/{year}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<IEnumerable<ParcelWaterSupplyDto>> ListParcelsWithWaterSupplyByYearAndGeography([FromRoute] int geographyID, [FromRoute] int year)
    {
        var reportingPeriod = ReportingPeriods.GetByGeographyID(_dbContext, geographyID);
        if (reportingPeriod == null)
        {
            return BadRequest("Cannot display grid because there is no reporting period for this geography.");
        }

        var fReportingPeriod = ReportingPeriods.GetByGeographyIDAndYear(_dbContext, geographyID, year);
        var endDate = fReportingPeriod.EndDate;

        var parcelWaterSupplyBreakdownForYear = ParcelSupplies
            .GetParcelWaterSupplyBreakdownForYearAsDto(_dbContext, endDate)
            .ToDictionary(x => x.ParcelID, x => x.WaterSupplyByWaterType);
        var parcelZones = _dbContext.ParcelZones.AsNoTracking().Include(x => x.Zone).ThenInclude(x => x.ZoneGroup).ToList().GroupBy(x => x.ParcelID).ToDictionary(x => x.Key, x => x.Select(y => y.Zone.AsZoneMinimalDto()).ToList());

        var parcelWaterSupplyAndUsageDtos =
            _dbContext.Parcels.AsNoTracking().Where(x => x.GeographyID == geographyID).Select(x =>
                    new ParcelWaterSupplyDto()
                    {
                        ParcelID = x.ParcelID,
                        ParcelNumber = x.ParcelNumber,
                        ParcelArea = x.ParcelArea,
                        ParcelStatusDisplayName = x.ParcelStatus.ParcelStatusDisplayName
                    })
                .ToList();
        foreach (var parcelWaterSupplyAndUsageDto in parcelWaterSupplyAndUsageDtos)
        {
            parcelWaterSupplyAndUsageDto.Zones =
                parcelZones.TryGetValue(parcelWaterSupplyAndUsageDto.ParcelID, out var zoneSimpleDtos)
                    ? zoneSimpleDtos
                    : [];
            parcelWaterSupplyAndUsageDto.WaterSupplyByWaterType =
                parcelWaterSupplyBreakdownForYear.TryGetValue(parcelWaterSupplyAndUsageDto.ParcelID, out var value)
                    ? value
                    : null;
        }
        return Ok(parcelWaterSupplyAndUsageDtos);
    }

    [HttpGet("geographies/{geographyID}/water-accounts/{waterAccountID}/year/{year}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<List<ParcelMinimalDto>> ListParcelsByAccountID([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int year)
    {
        var reportingPeriod = ReportingPeriods.GetByGeographyID(_dbContext, geographyID);
        if (reportingPeriod == null)
        {
            return BadRequest("Cannot display grid because there is no reporting period for this geography.");
        }

        var endDate = ReportingPeriods.GetByGeographyIDAndYear(_dbContext, geographyID, year).EndDate;
        var parcels = Parcels.ListParcelsFromAccountIDAndEndDate(_dbContext, waterAccountID, endDate);

        return Ok(parcels);
    }

    [HttpGet("geographies/{geographyID}/parcels/review-changes-grid-items")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public ActionResult<List<ParcelChangesGridItemDto>> GetParcelReviewChangesGridItems([FromRoute] int geographyID)
    {
        var parcelChangesGridItemDtos = ParcelHistories.ListParcelChangesGridItemDtosByGeographyID(_dbContext, geographyID);
        return Ok(parcelChangesGridItemDtos);
    }

    [HttpGet("water-accounts/{waterAccountID}/parcel-minimals")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<ParcelMinimalDto>> GetCurrentParcelsFromAccountID([FromRoute] int waterAccountID)
    {
        var parcels = Parcels.ListByWaterAccountID(_dbContext, waterAccountID)
            .Select(x => x.AsParcelMinimalDto()).ToList();

        return Ok(parcels);
    }

    [HttpGet("geographies/{geographyID}/effectiveYears")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<int>> GetEffectiveYearsForGeography([FromRoute] int geographyID)
    {
        var startYear = WaterAccountParcels.GetNextAvailableEffectiveYearForGeography(_dbContext, geographyID);
        var maxYear = DateTime.UtcNow.Year + 1;
        var years = Enumerable.Range(startYear, maxYear - startYear + 1).ToList();
        return Ok(years);
    }

    [HttpPost("geographies/{geographyID}/upload-parcel-gdb")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L)]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Create)]
    public async Task<ActionResult> UploadGDBAndParseFeatureClasses([FromForm] UploadedGdbRequestDto uploadedGdbRequestDto, [FromRoute] int geographyID)
    {
        if (ParcelHistories.GeographyHasUnreviewedParcels(_dbContext, geographyID))
        {
            return BadRequest(
                "This geography has unreviewed parcel changes. Please review all current changes before uploading any new parcel data.");
        }

        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        // save the gdb file contents to UploadedGdb so user doesn't have to wait for upload of file again
        var uploadedGdb = await UploadedGdbs.CreateNew(_dbContext, user.UserID, geographyID);
        await using (var stream = uploadedGdbRequestDto.File.OpenReadStream())
        {
            await _fileService.SaveFileStreamToAzureBlobStorage(uploadedGdb.CanonicalName, stream);
        }

        var ogrInfoRequestDto = new OgrInfoRequestDto { BlobContainer = FileService.FileContainerName, CanonicalName = uploadedGdb.CanonicalName };
        var srid = await _gdalApiService.OgrInfoGdbGetSRID(ogrInfoRequestDto);

        uploadedGdb.SRID = srid;
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("geographies/{geographyID}/parcel/getFeatureClassInfo")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public async Task<ActionResult<UploadParcelLayerInfoDto>> GetFeatureClassInfo([FromRoute] int geographyID)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var uploadedGdbDto = GetLatestUploadedGdbForUserAndGeography(user.UserID, geographyID);
        if (uploadedGdbDto == null)
        {
            return BadRequest(
                "You have not uploaded a file for review. Please upload one.");
        }

        try
        {
            var ogrInfoRequestDto = new OgrInfoRequestDto { BlobContainer = FileService.FileContainerName, CanonicalName = uploadedGdbDto.CanonicalName};
            var featureClassInfos = await _gdalApiService.OgrInfoGdbToFeatureClassInfo(ogrInfoRequestDto);
            var uploadParcelLayerInfoDto = new UploadParcelLayerInfoDto()
            {
                UploadedGdbID = uploadedGdbDto.UploadedGdbID,
                FeatureClasses = featureClassInfos
            };

            return Ok(uploadParcelLayerInfoDto);
        }
        catch (ValidationException e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest("Error reading GDB file!");
        }
    }

    [HttpPost("geographies/{geographyID}/parcels/previewGDBChanges")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Create)]
    public async Task<ActionResult> PreviewParcelLayerGDBChangesViaGeoJsonFeatureCollectionAndUploadToStaging([FromBody] ParcelLayerUpdateDto parcelLayerUpdateDto, [FromRoute] int geographyID)
    {
        if (ParcelHistories.GeographyHasUnreviewedParcels(_dbContext, geographyID))
        {
            return BadRequest(
                "This geography has unreviewed parcel changes. Please review all current changes before uploading any new parcel data.");
        }

        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var geography = Geographies.GetByID(_dbContext, geographyID);

        var uploadedGdbDto = GetLatestUploadedGdbForUserAndGeography(user.UserID, geographyID);
        if (uploadedGdbDto == null)
        {
            return BadRequest(
                "You have not uploaded a file for review. Please upload one.");
        }

        var errors = UploadedGdbs.ValidateEffectiveYear(_dbContext, parcelLayerUpdateDto.EffectiveYear, geographyID);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // these need to match the ParcelStaging schema
        var columns = new List<string>
        {
            $"{geography.GeographyID} as GeographyID",
            $"{parcelLayerUpdateDto.ParcelNumberColumn} as ParcelNumber",
            $"{parcelLayerUpdateDto.OwnerNameColumn} as OwnerName",
            $"{parcelLayerUpdateDto.OwnerAddressColumn} as OwnerAddress"
        };

        var apiRequest = new GdbToGeoJsonRequestDto()
        {
            BlobContainer = FileService.FileContainerName,
            CanonicalName = uploadedGdbDto.CanonicalName,
            GdbLayerOutputs = new()
            {
                new()
                {
                    Columns = columns,
                    FeatureLayerName = parcelLayerUpdateDto.ParcelLayerNameInGDB,
                    NumberOfSignificantDigits = 4,
                    Filter = "",
                    CoordinateSystemID = uploadedGdbDto.SRID,
                    Extent = null
                }
            }
        };
        var geoJson = await _gdalApiService.Ogr2OgrGdbToGeoJson(apiRequest);
        var parcelStagings = await GeoJsonSerializer.DeserializeFromFeatureCollection<ParcelStaging>(geoJson, GeoJsonSerializer.CreateGeoJSONSerializerOptions(14, 4), uploadedGdbDto.SRID);

        var results = ValidateParcelGdb(uploadedGdbDto, parcelStagings);
        results.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await SaveToParcelStagingTable(parcelLayerUpdateDto, uploadedGdbDto, parcelStagings, geography);
            return Ok();
        }
        catch (ValidationException e)
        {
            _logger.LogError(e.Message);

            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            return BadRequest("Error generating preview of changes!");
        }
    }

    private async Task SaveToParcelStagingTable(ParcelLayerUpdateDto parcelLayerUpdateDto, UploadedGdbDto uploadedGdbDto,
        List<ParcelStaging> parcelStagings, Geography geography)
    {
        var uploadedGdb = _dbContext.UploadedGdbs.Single(x => x.UploadedGdbID == uploadedGdbDto.UploadedGdbID);
        uploadedGdb.EffectiveYear = parcelLayerUpdateDto.EffectiveYear;
        await _dbContext.SaveChangesAsync();

        var geographyCoordinateSystemWkt = await _gdalApiService.GdalSrsInfoGetWktForCoordinateSystem(geography.CoordinateSystem);

        foreach (var parcelStaging in parcelStagings)
        {
            var parcelNumber = parcelStaging.ParcelNumber;

            if (!Parcels.IsValidParcelNumber(geography.APNRegexPattern, parcelNumber))
            {
                throw new ValidationException(
                    $"Parcel number found that does not comply to format {geography.APNRegexPatternDisplay}. Please ensure that that correct column is selected and all Parcel Numbers follow the specified format and try again.");
            }

            parcelStaging.Geometry = parcelStaging.Geometry.SRID != geography.CoordinateSystem
                ? parcelStaging.Geometry.ProjectToSrid(geography.CoordinateSystem, geographyCoordinateSystemWkt)
                : parcelStaging.Geometry;

            parcelStaging.Geometry4326 = parcelStaging.Geometry.ProjectTo4326(geographyCoordinateSystemWkt);
        }

        if (parcelStagings.Any())
        {
            // Make sure staging table is empty before proceeding; we need to call a sproc to do it since we need elevated permissions to use truncate
            await _dbContext.Database.ExecuteSqlRawAsync("EXECUTE dbo.pParcelStagingTruncate");
            _dbContext.ParcelStagings.AddRange(parcelStagings);
            await _dbContext.SaveChangesAsync();
            await _dbContext.Database.ExecuteSqlRawAsync("EXECUTE dbo.pParcelStagingMakeGeometryValid");
        }
    }

    private List<ErrorMessage> ValidateParcelGdb(UploadedGdbDto uploadedGdbDto, List<ParcelStaging> parcelStagings)
    {
        var results = new List<ErrorMessage>();

        var apns = parcelStagings.Select(x => x.ParcelNumber).ToList();
        var distinctApns = apns.Distinct().ToList();

        if (apns.Count != distinctApns.Count)
        {
            results.Add(new ErrorMessage()
            {
                Type = "Unique APN",
                Message =
                    $"The APNs in your uploaded GDB are required to be unique. Found {apns.Count()} total APNs and {distinctApns.Count()} distinct APNs. Please upload another GDB with unique APN values per geometry or select another column."
            });
        }

        return results;
    }

    private UploadedGdbDto GetLatestUploadedGdbForUserAndGeography(int userID, int geographyID)
    {
        var uploadedGdbDto = _dbContext.UploadedGdbs.Include(x => x.User)
            .Include(x => x.Geography).AsNoTracking()
            .OrderByDescending(x => x.UploadDate).FirstOrDefault(x => x.UserID == userID && x.GeographyID == geographyID)?.AsDto();
        return uploadedGdbDto;
    }

    [HttpGet("geographies/{geographyID}/parcels/getExpectedResults")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<ParcelUpdateExpectedResultsDto> GetExpectedResults([FromRoute] int geographyID)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var errors = UploadedGdbs.ValidateUploadedGDB(_dbContext, user.UserID);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return ParcelStagings.GetExpectedResultsDto(_dbContext, geographyID);
    }



    [HttpPost("geographies/{geographyID}/parcels/enactGDBChanges")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Create)]
    public async Task<ActionResult> EnactGDBChanges([FromRoute] int geographyID)
    {
        if (ParcelHistories.GeographyHasUnreviewedParcels(_dbContext, geographyID))
        {
            return BadRequest(
                "This geography has unreviewed parcel changes. Please review all current changes before uploading any new parcel data.");
        }

        await using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var uploadedGdbDto = GetLatestUploadedGdbForUserAndGeography(user.UserID, geographyID);
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                "EXECUTE dbo.pParcelUpdateFromStaging @geographyID={0}, @effectiveYear={1}, @uploadUserID={2}",
                geographyID, uploadedGdbDto.EffectiveYear, user.UserID);

            await dbContextTransaction.CommitAsync();
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await dbContextTransaction.RollbackAsync();
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        //var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
        //var mailMessage = GenerateParcelUpdateCompletedEmail(_rioConfiguration.WEB_URL, waterYearDto, expectedResults, smtpClient);
        //SitkaSmtpClientService.AddCcRecipientsToEmail(mailMessage,
        //    EFModels.Entities.User.GetEmailAddressesForAdminsThatReceiveSupportEmails(_dbContext));
        //await SendEmailMessage(smtpClient, mailMessage);

        var uploadedGdb = _dbContext.UploadedGdbs.Single(x => x.UploadedGdbID == uploadedGdbDto.UploadedGdbID);
        uploadedGdb.Finalized = true;
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("geographies/{geographyID}/parcels/uploaded-gdb/latest")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<UploadedGdbSimpleDto> GetLatestFinalizedGDBUploadForGeography([FromRoute] int geographyID)
    {
        var gdbUploadSimpleDto = _dbContext.UploadedGdbs.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.Finalized).ToList()
            .MaxBy(x => x.UploadDate)?.AsSimpleDto();

        return (gdbUploadSimpleDto);

    }

    [HttpPost("geographies/{geographyID}/parcel/account")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public async Task<IActionResult> ChangeOwnershipOfParcel([FromBody] ParcelOwnershipUpdateDto parcelOwnershipUpdateDto, [FromRoute] int geographyID)
    {
        if (parcelOwnershipUpdateDto.ToBeInactivated == true)
        {
            var parcel = _dbContext.Parcels.Single(x => x.ParcelID == parcelOwnershipUpdateDto.ParcelID);
            parcel.ParcelStatusID = (int)ParcelStatusEnum.Inactive;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        var accountParcel = _dbContext.WaterAccountParcels.SingleOrDefault(x => x.GeographyID == geographyID
                                                                           && x.WaterAccountID == parcelOwnershipUpdateDto.WaterAccountID
                                                                           && x.ParcelID == parcelOwnershipUpdateDto.ParcelID
                                                                           && x.EffectiveYear == parcelOwnershipUpdateDto.EffectiveYear);
        if (accountParcel != null)
        {
            return BadRequest("There is already a matching entry.");
        }
        await _dbContext.WaterAccountParcels.AddAsync(new WaterAccountParcel()
        {
            GeographyID = geographyID,
            WaterAccountID = parcelOwnershipUpdateDto.WaterAccountID,
            ParcelID = parcelOwnershipUpdateDto.ParcelID,
            EffectiveYear = parcelOwnershipUpdateDto.EffectiveYear,
        });
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("geographies/{geographyID}/parcels/{parcelID}/allocation-plans")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<AllocationPlanManageDto>> GetParcelAllocationPlansByParcelID([FromRoute] int geographyID, [FromRoute] int parcelID)
    {
        var geographyAllocationPlan = GeographyAllocationPlanConfigurations.GetByGeographyID(_dbContext, geographyID);
        if (geographyAllocationPlan == null)
        {
            return NotFound();
        }

        var parcelZoneIDs = _dbContext.ParcelZones
            .Include(x => x.Zone).AsNoTracking()
            .Where(x => x.ParcelID == parcelID && x.Zone.ZoneGroupID == geographyAllocationPlan.ZoneGroupID)
            .Select(x => x.ZoneID).ToList();

        var allocationPlans =
            AllocationPlans.ListByGeographyIDAndZoneIDsAsSimpleDto(_dbContext, geographyID, parcelZoneIDs);

        var allocationPlanManageDtos =
            AllocationPlans.GetAllocationPlanManageDtos(_dbContext, allocationPlans.Select(x => x.AllocationPlanID));

        return Ok(allocationPlanManageDtos);
    }

    [HttpPost("geographies/{geographyID}/parcels/bulk-update-parcel-status/")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public ActionResult BulkUpdateParcelStatus([FromRoute] int geographyID, [FromBody] ParcelBulkUpdateParcelStatusDto parcelBulkUpdateParcelStatusDto)
    {
        var validParcelStatuses = new List<int>
        {
            (int)ParcelStatusEnum.Excluded,
            (int)ParcelStatusEnum.Inactive,
            (int)ParcelStatusEnum.Unassigned
        };
        if (validParcelStatuses.Contains(parcelBulkUpdateParcelStatusDto.ParcelStatusID))
        {
            var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
            Parcels.UpdateParcelStatus(_dbContext, parcelBulkUpdateParcelStatusDto, user.UserID);
        }
        else
        {
            return BadRequest("Invalid Parcel Status for bulk update.");
        }
        return Ok();
    }
}