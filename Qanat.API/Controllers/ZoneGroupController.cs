using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class ZoneGroupController : SitkaController<ZoneGroupController>
{
    public ZoneGroupController(QanatDbContext dbContext, ILogger<ZoneGroupController> logger, IOptions<QanatConfiguration> qanatConfiguration)
        : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("geographies/{geographyID}/zone-groups")]
    [WithGeographyRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Read)]
    public ActionResult<List<ZoneGroupMinimalDto>> Get([FromRoute] int geographyID)
    {
        var zoneGroupMinimalDtos = ZoneGroups.GetZoneGroupsByGeography(_dbContext, geographyID);
        return zoneGroupMinimalDtos;
    }

    [HttpGet("geographies/{geographyID}/zones")]
    [WithGeographyRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Read)]
    public ActionResult<List<ZoneDetailedDto>> GetZones([FromRoute] int geographyID)
    {
        var zoneDetailedDtos = Zones.ListByGeographyIDAsZoneDetailedDto(_dbContext, geographyID);
        return zoneDetailedDtos;
    }

    [HttpGet("public/geography/{geographyID}/zone-group/{zoneGroupSlug}")]
    [GeographyAllocationPlansPublic]
    public ActionResult<ZoneGroupMinimalDto> GetByID([FromRoute] int geographyID, string zoneGroupSlug)
    {
        return ZoneGroups.GetByZoneGroupSlugAsMinimalDto(_dbContext, zoneGroupSlug, geographyID);
    }

    [HttpPost("zone-group/bounding-box")]
    [WithRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Read)]
    public ActionResult<BoundingBoxDto> GetBoundingBoxForZoneGroup([FromBody] List<int> zoneGroupIDs)
    {
        var boundingBox = ZoneGroups.GetBoundingBoxForZoneGroup(_dbContext, zoneGroupIDs);
        return Ok(boundingBox);
    }

    [HttpGet("geographies/{geographyID}/zone-groups/{zoneGroupSlug}")]
    [WithGeographyRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Read)]
    [SwaggerResponse(statusCode: 200, Description = "CSV of Zone Group Data", ContentTypes = new string[] { "text/csv" }, Type = typeof(FileContentResult))]
    public IActionResult ListZoneGroupData([FromRoute] int geographyID, [FromRoute] string zoneGroupSlug)
    {
        var zoneGroup = ZoneGroups.GetByZoneGroupSlugAsMinimalDto(_dbContext, zoneGroupSlug, geographyID);
        var result = ZoneGroups.ListZoneGroupDataAsCsvByteArray(_dbContext, geographyID, zoneGroup.ZoneGroupID);
        return new FileContentResult(result, "text/csv") { FileDownloadName = $"ZoneGroupDataFor{zoneGroup.ZoneGroupName}.csv" };
    }

    [HttpGet("geographies/{geographyID}/zone-group/{zoneGroupSlug}/water-usage")]
    [WithGeographyRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Read)]
    public ActionResult<List<ZoneGroupMonthlyUsageDto>> GetWaterUsageByZoneGroupSlug([FromRoute] int geographyID, string zoneGroupSlug)
    {
        var zoneGroup = ZoneGroups.GetByZoneGroupSlug(_dbContext, zoneGroupSlug, geographyID);
        if (zoneGroup == null)
        {
            return NotFound();
        }

        var zoneGroupMonthlyUsageDtos = ZoneGroupMonthlyUsage.ListByZoneGroupID(_dbContext, geographyID, zoneGroup.ZoneGroupID);
        return Ok(zoneGroupMonthlyUsageDtos);
    }

    [HttpPost("geographies/{geographyID}/zone-groups")]
    [WithGeographyRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Update)]
    public ActionResult<List<ZoneGroupMinimalDto>> Update([FromRoute] int geographyID, [FromBody] ZoneGroupMinimalDto zoneGroupMinimalDto)
    {
        var errors = ZoneGroups.ValidateZoneGroup(_dbContext, geographyID, zoneGroupMinimalDto);
        var zoneErrors = Zones.ValidateZones(zoneGroupMinimalDto.ZoneList);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        zoneErrors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ZoneGroups.CreateOrUpdateZoneGroup(_dbContext, zoneGroupMinimalDto);
        var zoneGroups = ZoneGroups.GetZoneGroupsByGeography(_dbContext, geographyID);
        return zoneGroups;
    }

    [HttpPut("geographies/{geographyID}/zone-groups/sort-order")]
    [WithGeographyRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Update)]
    public ActionResult UpdateSortOrder([FromRoute] int geographyID,
        [FromBody] List<ZoneGroupMinimalDto> zoneGroupMinimalDtos)
    {
        ZoneGroups.UpdateZoneGroupSortOrder(_dbContext, zoneGroupMinimalDtos);
        return Ok();
    }

    [HttpDelete("geographies/{geographyID}/zone-group/{zoneGroupID}")]
    [WithGeographyRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Delete)]
    public ActionResult<List<ZoneGroupMinimalDto>> Delete([FromRoute] int geographyID, int zoneGroupID)
    {
        var zoneGroup = ZoneGroups.GetByID(_dbContext, zoneGroupID);
        if (zoneGroup.GeographyAllocationPlanConfigurations.Any())
        {
            ModelState.AddModelError("Zone Group", "This Zone Group cannot be deleted because it is currently associated with an Allocation Plan.");
            return BadRequest();
        }

        ZoneGroups.DeleteZoneGroup(_dbContext, zoneGroupID);
        var zoneGroups = ZoneGroups.GetZoneGroupsByGeography(_dbContext, geographyID);
        return zoneGroups;
    }

    [HttpDelete("geographies/{geographyID}/zone-group/{zoneGroupID}/clear")]
    [WithGeographyRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Delete)]
    public ActionResult ClearAllZoneGroupData([FromRoute] int geographyID, int zoneGroupID)
    {
        ZoneGroups.ClearZoneGroupData(_dbContext, zoneGroupID);
        return Ok();
    }

    [HttpPost("geographies/{geographyID}/zone-group/{zoneGroupID}/csv")]
    [RequestSizeLimit(524288000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
    [WithGeographyRolePermission(PermissionEnum.ZoneGroupRights, RightsEnum.Create)]
    public async Task<IActionResult> UploadZoneGroupData([FromRoute] int geographyID, int zoneGroupID, [FromForm] ZoneGroupCsvUpsertDto zoneGroupCsvUpsertDto)
    {
        var extension = Path.GetExtension(zoneGroupCsvUpsertDto.UploadedFile.FileName);
        if (extension != ".csv")
        {
            ModelState.AddModelError("FileResource", $"{extension[1..].ToUpper()} is not an accepted file extension");
            return BadRequest(ModelState);
        }
        if (zoneGroupCsvUpsertDto.APNColumnName == zoneGroupCsvUpsertDto.ZoneColumnName)
        {
            ModelState.AddModelError("Value Column", "The selected Value column cannot match the selected APN column. Two distinct header names are required.");
            return BadRequest(ModelState);
        }

        var fileData = await HttpUtilities.GetIFormFileData(zoneGroupCsvUpsertDto.UploadedFile);

        if (!ParseCsvUpload(fileData, zoneGroupCsvUpsertDto.APNColumnName, zoneGroupCsvUpsertDto.ZoneColumnName, out var records))
        {
            return BadRequest(ModelState);
        }

        if (!ValidateCsvUploadData(records, geographyID))
        {
            return BadRequest(ModelState);
        }

        var errors = ZoneGroups.ValidateCsv(_dbContext, records, geographyID, zoneGroupID);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = ZoneGroups.CreateFromCSV(_dbContext, records, geographyID, zoneGroupID);

        return Ok(response);
    }


    private bool ParseCsvUpload(byte[] fileData, string apnColumnName, string zoneColumnName, out List<ZoneGroupCSV> records)
    {
        try
        {
            using var memoryStream = new MemoryStream(fileData);
            using var reader = new StreamReader(memoryStream);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            csvReader.Context.RegisterClassMap(new ZoneGroupCSVMap(apnColumnName, zoneColumnName));
            records = csvReader.GetRecords<ZoneGroupCSV>().ToList();
            return true;
        }
        catch (HeaderValidationException e)
        {
            var headerMessage = e.Message.Split('.')[0];
            ModelState.AddModelError("UploadedFile",
                $"{headerMessage}. Please check that the column name is not missing or misspelled.");
            records = null;
            return false;
        }
        catch (CsvHelper.MissingFieldException e)
        {
            var headerMessage = e.Message.Split('.')[0];
            ModelState.AddModelError("UploadedFile",
                $"{headerMessage}. Please check that the column name is not missing or misspelled.");
            records = null;
            return false;
        }
        catch
        {
            ModelState.AddModelError("UploadedFile",
                "There was an error parsing the CSV. Please ensure the file is formatted correctly.");
            records = null;
            return false;
        }
    }


    private bool ValidateCsvUploadData(List<ZoneGroupCSV> records, int geographyID)
    {
        var isValid = true;

        // no null APNs
        var nullAPNsCount = records.Count(x => x.APN == "");
        if (nullAPNsCount > 0)
        {
            ModelState.AddModelError("UploadedFile",
                $"The uploaded file contains {nullAPNsCount} {(nullAPNsCount > 1 ? "rows" : "row")} specifying a value with no corresponding APN.");
            isValid = false;
        }

        // no null quantities
        var nullQuantities = records.Where(x => x.Zone == null).ToList();
        if (nullQuantities.Any())
        {
            ModelState.AddModelError("UploadedFile",
                $"The following {(nullQuantities.Count > 1 ? "APNs" : "APN")} had no usage quantity entered: {string.Join(", ", nullQuantities.Select(x => x.APN))}");
            isValid = false;
        }

        return isValid;
    }


}