using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Services.GDAL;
using Qanat.Common.Util;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class WellController : SitkaController<WellController>
{
    private readonly FileService _fileService;
    private readonly GDALAPIService _gdalApiService;
    private readonly HierarchyContext _hierarchyContext;

    public WellController(QanatDbContext dbContext, ILogger<WellController> logger,
        IOptions<QanatConfiguration> qanatConfiguration, FileService fileService, GDALAPIService gdalApiService, HierarchyContext hierarchyContext) : base(
        dbContext, logger, qanatConfiguration)
    {
        _fileService = fileService;
        _gdalApiService = gdalApiService;
        _hierarchyContext = hierarchyContext;
    }

    [HttpGet("geographies/{geographyID}/wells")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<List<WellMinimalDto>> ListWells([FromRoute] int geographyID)
    {
        var wellMinimalDtos = Wells.ListByGeographyIDAsMinimalDto(_dbContext, geographyID);
        return Ok(wellMinimalDtos);
    }

    [HttpGet("wells/{wellID}")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<WellMinimalDto> GetWellAsMinimalDto([FromRoute] int wellID)
    {
        var wellMinimalDto = _dbContext.Wells.AsNoTracking()
            .SingleOrDefault(x => x.WellID == wellID)?.AsMinimalDto();

        return Ok(wellMinimalDto);
    }

    [HttpGet("wells/{wellID}/details")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<WellDetailDto> GetWellAsDetailDto([FromRoute] int wellID)
    {
        var well = _dbContext.Wells.AsNoTracking()
            .Include(x => x.Geography).ThenInclude(x => x.GeographyConfiguration)
            .Include(x => x.Parcel).ThenInclude(x => x.WaterAccount)
            .Include(x => x.WellRegistrations)
            .Include(x => x.WellIrrigatedParcels).ThenInclude(x => x.Parcel)
            .Include(x => x.WellMeters).ThenInclude(x => x.Meter)
            .SingleOrDefault(x => x.WellID == wellID);

        var wellDetailDto = new WellDetailDto()
        {
            WellID = well.WellID,
            WellName = well.WellName,
            WellStatus = well.WellStatus.AsSimpleDto(),
            WellRegistration = well.WellRegistrations.Any()
                ? well.WellRegistrations.MaxBy(x => x.ApprovalDate).AsMinimalDto()
                : null,
            Parcel = well.Parcel?.AsParcelMinimalDto(),
            IrrigatedParcels = well.WellIrrigatedParcels.Any()
                ? well.WellIrrigatedParcels.Select(x => x.Parcel.AsDisplayDto()).ToList()
                : null,
            Geography = well.Geography.AsDisplayDto(),
            Meter = well.WellMeters.FirstOrDefault(x => !x.EndDate.HasValue)?.Meter.AsGridDto(),
            StateWCRNumber = well.StateWCRNumber,
            CountyWellPermitNumber = well.CountyWellPermitNumber,
            Latitude = well.Latitude,
            Longitude = well.Longitude,
            DateDrilled = well.DateDrilled,
            WellDepth = well.WellDepth,
            MetersEnabled = well.Geography.GeographyConfiguration.MetersEnabled,
            Notes = well.Notes
        };

        return Ok(wellDetailDto);
    }

    [HttpGet("wells/{wellID}/popup")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<WellPopupDto> GetWellForPopup([FromRoute] int wellID)
    {
        var well = _dbContext.Wells
            .Include(x => x.Parcel)
            .Include(x => x.Geography)
            .Include(x => x.WellRegistrations)
            .SingleOrDefault(x => x.WellID == wellID);
        if (well == null)
        {
            return BadRequest("Well could not be found. ");
        }

        return Ok(new WellPopupDto()
        {
            WellID = well.WellID,
            Parcel = well.Parcel?.AsSimpleDto(),
            WellName = well.WellName,
            Geography = well.Geography.AsDisplayDto(),
            WellRegistrationID = well.WellRegistrations.Any() ? well.WellRegistrations.First().WellRegistrationID : null
        });
    }

    [HttpGet("geographies/{geographyID}/wells/current-user")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<List<WellMinimalDto>> ListWellsByGeographyIDAndCurrentUser([FromRoute] int geographyID)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var hasPermission = new WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update).HasPermission(user, _hierarchyContext);
        var wellMinimalDtos = hasPermission 
            ? Wells.ListByGeographyIDAsMinimalDto(_dbContext, geographyID) 
            : Wells.ListByUserIDAndGeographyIDAsMinimalDto(_dbContext, geographyID, user.UserID);

        return Ok(wellMinimalDtos);
    }

    [HttpGet("geographies/{geographyID}/wells/location")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<List<WellLocationDto>> ListWellLocationsByGeographyID([FromRoute] int geographyID)
    {
        var wellLocations = _dbContext.Wells
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsLocationDto(false))
            .ToList();

        return Ok(wellLocations);
    }

    [HttpGet("wells/{wellID}/location")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<WellLocationDto> GetLocationByWellID([FromRoute] int wellID)
    {
        var well = Wells.GetByID(_dbContext, wellID);
        var wellLocationDto = well.AsLocationDto(true);
        return Ok(wellLocationDto);
    }

    [HttpPut("wells/{wellID}/location/preview")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult<WellLocationPreviewDto> PreviewWellLocationUpdate([FromRoute] int wellID, [FromBody] WellLocationDto requestDto)
    {
        var parcel = Wells.GetWellParcelFromGeometryIntersection(_dbContext, requestDto);

        var responseDto = new WellLocationPreviewDto()
        {
            WellID = requestDto.WellID,
            ParcelID = parcel?.ParcelID,
            ParcelNumber = parcel?.ParcelNumber
        };

        return Ok(responseDto);
    }

    [HttpPut("wells/{wellID}/location")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult UpdateWellLocation([FromRoute] int wellID, [FromBody] WellLocationDto requestDto)
    {
        Wells.UpdateWellLocation(_dbContext, wellID, requestDto);
        return Ok();
    }

    [HttpPost("wells/bounding-box")]
    [WithRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<BoundingBoxDto> GetBoundingBoxByWellRegistrationIDs([FromBody] List<int> wellIDs)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var boundingBoxDto = Wells.GetBoundingBoxByWellIDs(_dbContext, wellIDs);
        return Ok(boundingBoxDto);
    }

    [HttpPut("wells/{wellID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult<WellMinimalDto> UpdateWellDetails([FromRoute] int wellID, [FromBody] ManagerWellUpdateRequestDto requestDto)
    {
        Wells.UpdateWellDetails(_dbContext, requestDto);

        var wellMinimalDto = Wells.GetByID(_dbContext, requestDto.WellID).AsMinimalDto();
        return Ok(wellMinimalDto);
    }

    [HttpGet("wells/{wellID}/irrigated-parcels")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult<WellIrrigatedParcelsResponseDto> GetWellRegistrationIrrigatedParcels([FromRoute] int wellID)
    {
        var dto = Wells.GetWellIrrigatedParcelsDto(_dbContext, wellID);
        return Ok(dto);
    }

    [HttpPut("wells/{wellID}/irrigated-parcels")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult UpdateWellIrrigatedParcels([FromRoute] int wellID, [FromBody] WellIrrigatedParcelsRequestDto requestDto)
    {
        Wells.UpdateWellIrrigatedParcels(_dbContext, wellID, requestDto);
        return Ok();
    }

    [HttpGet("geographies/{geographyID}/reference-wells")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanRegisterWells)]
    public ActionResult<List<ReferenceWellMapMarkerDto>> GetGeographyReferenceWellsForMap([FromRoute] int geographyID)
    {
        var wells = _dbContext.ReferenceWells.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsReferenceWellMapMarkerDto())
            .ToList();

        return wells;
    }

    [HttpGet("geographies/{geographyID}/reference-wells/grid")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanRegisterWells)]
    public ActionResult<List<ReferenceWellManageGridDto>> GetGeographyReferenceWellsForGrid([FromRoute] int geographyID)
    {
        var wells = _dbContext.ReferenceWells.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .Select(referenceWell => new ReferenceWellManageGridDto()
            {
                ReferenceWellID = referenceWell.ReferenceWellID,
                GeographyID = referenceWell.GeographyID,
                WellName = referenceWell.WellName,
                WellDepth = referenceWell.WellDepth,
                CountyWellPermitNo = referenceWell.CountyWellPermitNo,
                StateWCRNumber = referenceWell.StateWCRNumber,
                Latitude = referenceWell.LocationPoint4326.Coordinate.Y,
                Longitude = referenceWell.LocationPoint4326.Coordinate.X,
            })
            .ToList();

        return wells;
    }

    [HttpPost("geographies/{geographyID}/upload-wells")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Create)]
    public async Task<ActionResult> UploadGDBAndParseFeatureClasses([FromForm] UploadedGdbRequestDto uploadedGdbRequestDto, [FromRoute] int geographyID)
    {
        var geography = Geographies.GetByID(_dbContext, geographyID);
        var uploadedWellGdb = await SaveUploadedWellGdbToBlobStorage(uploadedGdbRequestDto, geographyID);

        try
        {
            var ogrInfoRequestDto = new OgrInfoRequestDto
                { BlobContainer = FileService.FileContainerName, CanonicalName = uploadedWellGdb.CanonicalName };
            var featureClassInfos = await _gdalApiService.OgrInfoGdbToFeatureClassInfo(ogrInfoRequestDto);
            if (featureClassInfos.Count > 1)
            {
                return BadRequest("Too many feature classes. There can only be one.");
            }

            var featureClassInfo = featureClassInfos[0];
            if (!featureClassInfo.Columns.Contains("WellName", StringComparer.InvariantCultureIgnoreCase))
            {
                ModelState.AddModelError("Wells", "Well Name is a required column.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var uploadColumnList = new List<string>()
            {
                "WellName",
                "CountyPermitNumber",
                "DateDrilled",
                "StateWCRNumber",
                "WellDepth",
                "WellStatus"
            };

            var wellStagings = await GdbToWellList<WellStaging>(featureClassInfo, uploadedWellGdb, uploadColumnList);

            if (HasDuplicateWellNames(wellStagings))
            {
                ModelState.AddModelError("Well Name", "There are duplicate well names in gdb. Please review.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedWells = new List<Well>();
            var wellStatusDictionary = WellStatus.All.ToDictionary(x => x.WellStatusDisplayName);
            var geographyCoordinateSystemWkt = await _gdalApiService.GdalSrsInfoGetWktForCoordinateSystem(geography.CoordinateSystem);

            foreach (var wellStaging in wellStagings)
            {
                var well = new Well
                {
                    GeographyID = geographyID,
                    WellName = wellStaging.WellName?.Trim(),
                    LocationPoint4326 = wellStaging.Geometry,
                    StateWCRNumber = wellStaging.StateWCRNumber?.Trim(),
                    CountyWellPermitNumber = wellStaging.CountyPermitNumber?.Trim(),
                };

                well.LocationPoint = well.LocationPoint4326.ProjectToSrid(geography.CoordinateSystem, geographyCoordinateSystemWkt);
                if (wellStaging.DateDrilled.HasValue)
                {
                    well.DateDrilled = DateOnly.FromDateTime(wellStaging.DateDrilled.Value.AddHours(8));
                }
                well.WellDepth = wellStaging.WellDepth;
                if (!string.IsNullOrEmpty(wellStaging.WellStatus) && wellStatusDictionary.TryGetValue(wellStaging.WellStatus, out var wellStatus))
                {
                    well.WellStatusID = wellStatus.WellStatusID;
                }

                updatedWells.Add(well);
            }

            var existingWells = _dbContext.Wells.Where(x => x.GeographyID == geographyID).ToList();
            var allInDatabase = _dbContext.Wells;

            existingWells.MergeNew(updatedWells, allInDatabase,
                (x, y) => x.WellName == y.WellName && x.GeographyID == y.GeographyID);
            existingWells.MergeUpdate(updatedWells,
                (x, y) => x.WellName == y.WellName && x.GeographyID == y.GeographyID,
                (x, y) =>
                {
                    x.CountyWellPermitNumber = string.IsNullOrWhiteSpace(y.CountyWellPermitNumber) ? x.CountyWellPermitNumber : y.CountyWellPermitNumber;
                    x.LocationPoint = y.LocationPoint;
                    x.LocationPoint4326 = y.LocationPoint4326;
                    x.StateWCRNumber = string.IsNullOrWhiteSpace(y.StateWCRNumber) ? x.StateWCRNumber : y.StateWCRNumber;
                    x.DateDrilled = y.DateDrilled ?? x.DateDrilled;
                    x.WellDepth = y.WellDepth ?? x.WellDepth;
                });

            await _dbContext.SaveChangesAsync();

            return Ok();
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

    private async Task<List<T>> GdbToWellList<T>(FeatureClassInfo featureClassInfo, UploadedWellGdb uploadedWellGdb, IReadOnlyCollection<string> wellUploadColumnList) where T: IHasGeometry
    {
        var columnMappingStrings = new List<string>();
        columnMappingStrings.AddRange(wellUploadColumnList
            .Where(column => featureClassInfo.Columns.Contains(column, StringComparer.InvariantCultureIgnoreCase))
            .Select(column => $"{column}"));

        var apiRequest = new GdbToGeoJsonRequestDto()
        {
            BlobContainer = FileService.FileContainerName,
            CanonicalName = uploadedWellGdb.CanonicalName,
            GdbLayerOutputs = new()
            {
                new()
                {
                    Columns = columnMappingStrings,
                    FeatureLayerName = featureClassInfo.LayerName,
                    NumberOfSignificantDigits = 4,
                    Filter = "",
                    CoordinateSystemID = uploadedWellGdb.SRID,
                    Extent = null
                }
            }
        };
        var geoJson = await _gdalApiService.Ogr2OgrGdbToGeoJson(apiRequest);
        var fromFeatureCollection = await GeoJsonSerializer.DeserializeFromFeatureCollection<T>(geoJson,
            GeoJsonSerializer.CreateGeoJSONSerializerOptions(14, 4), uploadedWellGdb.SRID);
        return fromFeatureCollection;
    }

    public static bool HasDuplicateWellNames(List<WellStaging> wellStagings)
    {
        var wellNames = wellStagings.Select(x => x.WellName?.Trim()).ToList();
        return wellNames.Count != wellNames.Distinct().Count();
    }


    private async Task<UploadedWellGdb> SaveUploadedWellGdbToBlobStorage(UploadedGdbRequestDto uploadedGdbRequestDto, int geographyID)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var uploadedWellGdb = await UploadedWellGdbs.CreateNew(_dbContext, user.UserID, geographyID);
        await using (var stream = uploadedGdbRequestDto.File.OpenReadStream())
        {
            await _fileService.SaveFileStreamToAzureBlobStorage(uploadedWellGdb.CanonicalName, stream);
        }
        var ogrInfoRequestDto = new OgrInfoRequestDto { BlobContainer = FileService.FileContainerName, CanonicalName = uploadedWellGdb.CanonicalName };
        var srid = await _gdalApiService.OgrInfoGdbGetSRID(ogrInfoRequestDto);
        uploadedWellGdb.SRID = srid;
        await _dbContext.SaveChangesAsync();
        return uploadedWellGdb;
    }


    [HttpPost("geographies/{geographyID}/upload-reference-wells")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Create)]
    public async Task<ActionResult> UploadGDBAndParseFeatureClassesForReferenceWells([FromForm] UploadedGdbRequestDto uploadedGdbRequestDto, [FromRoute] int geographyID)
    {
        var uploadedWellGdb = await SaveUploadedWellGdbToBlobStorage(uploadedGdbRequestDto, geographyID);
        try
        {
            var ogrInfoRequestDto = new OgrInfoRequestDto
                { BlobContainer = FileService.FileContainerName, CanonicalName = uploadedWellGdb.CanonicalName };
            var featureClassInfos = await _gdalApiService.OgrInfoGdbToFeatureClassInfo(ogrInfoRequestDto);

            if (featureClassInfos.Count > 1)
            {
                return BadRequest("Too many feature classes. There can only be one.");
            }

            var featureClassInfo = featureClassInfos[0];
            var uploadColumnList = new List<string>()
            {
                "StateWCRNumber",
                "WellDepth",
                "CountyPermitNumber"
            };

            var referenceWellStagings = await GdbToWellList<ReferenceWellStaging>(featureClassInfo, uploadedWellGdb, uploadColumnList);
            var updatedWells = new List<ReferenceWell>();
            foreach (var referenceWellStaging in referenceWellStagings)
            {
                var well = new ReferenceWell()
                {
                    GeographyID = geographyID,
                    LocationPoint4326 = referenceWellStaging.Geometry
                };

                if (!string.IsNullOrWhiteSpace(referenceWellStaging.StateWCRNumber))
                {
                    well.StateWCRNumber = referenceWellStaging.StateWCRNumber.Trim();
                    well.WellName = well.StateWCRNumber;
                }

                if (!string.IsNullOrWhiteSpace(referenceWellStaging.CountyPermitNumber))
                {
                    well.CountyWellPermitNo = referenceWellStaging.CountyPermitNumber.Trim();
                    well.WellName = well.CountyWellPermitNo;
                }
                updatedWells.Add(well);
            };

            var existingWells = _dbContext.ReferenceWells.Where(x => x.GeographyID == geographyID).ToList();
            var allInDatabase = _dbContext.ReferenceWells;

            existingWells.MergeNew(updatedWells, allInDatabase, (x, y) => x.WellName == y.WellName);

            existingWells.MergeUpdate(updatedWells,
                (x, y) => x.WellName == y.WellName,
                (x, y) =>
                {
                    x.LocationPoint4326 = y.LocationPoint4326;
                    x.WellDepth = y.WellDepth ?? x.WellDepth;
                });

            await _dbContext.SaveChangesAsync();

            return Ok();
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

    [HttpPost("wells/{wellID}/meters")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult<MeterGridDto> AddWellMeter([FromRoute] int wellID, [FromBody] AddWellMeterRequestDto requestDto)
    {
        var errors = WellMeters.ValidateAddWellMeter(_dbContext, requestDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        WellMeters.AddWellMeter(_dbContext, requestDto);
        var meterDto = Meters.GetByIDAsGridDto(_dbContext, requestDto.MeterID);

        return Ok(meterDto);
    }

    [HttpPut("wells/{wellID}/meters")]
    [WithGeographyRolePermission(PermissionEnum.WellRights, RightsEnum.Update)]
    public ActionResult<MeterGridDto> RemoveWellMeter([FromRoute] int wellID, [FromBody] RemoveWellMeterRequestDto requestDto)
    {
        var wellMeter = _dbContext.WellMeters.SingleOrDefault(x => x.WellID == requestDto.WellID && x.MeterID == requestDto.MeterID && !x.EndDate.HasValue);

        var errors = WellMeters.ValidateRemoveWellMeter(wellMeter, requestDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        WellMeters.RemoveWellMeter(_dbContext, wellMeter, requestDto);

        return Ok();
    }

    [HttpGet("well-status")]
    [Authorize]
    public ActionResult<List<WellStatusSimpleDto>> ListWellStatus()
    {
        return Ok(WellStatus.AllAsSimpleDto);
    }

    [HttpGet("water-accounts/{waterAccountID}/wells")]
    [WithWaterAccountRolePermission(PermissionEnum.WellRights, RightsEnum.Read)]
    public ActionResult<List<WellMinimalDto>> ListWellsByWaterAccountID([FromRoute] int waterAccountID)
    {
        var wellMinimalDtos = Wells.ListByWaterAccountIDAsMinimalDto(_dbContext, waterAccountID);
        return Ok(wellMinimalDtos);
    }
}