using System;
using System.Data.SqlTypes;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/wells/{wellID}/meters/{meterID}/meter-readings")]
public class MeterReadingByMeterController(QanatDbContext dbContext, ILogger<MeterReadingByMeterController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<MeterReadingByMeterController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Well), "wellID")]
    [EntityNotFound(typeof(Meter), "meterID")]
    [WithWaterAccountRolePermission(PermissionEnum.WellMeterReadingRights, RightsEnum.Update)]
    public async Task<ActionResult<MeterReadingDto>> CreateMeterReading([FromRoute] int geographyID, [FromRoute] int wellID, [FromRoute] int meterID, [FromBody] MeterReadingUpsertDto meterReadingUpsertDto)
    {
        var errors = MeterReadings.ValidateMeterReadingUpsertAsync(_dbContext, geographyID, wellID, meterID, meterReadingUpsertDto);
        if (errors.Any())
        {
            errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
            return BadRequest(ModelState);
        }

        var meterReadingDto = await MeterReadings.CreateAsync(_dbContext, geographyID, wellID, meterID, meterReadingUpsertDto);
        return Ok(meterReadingDto);
    }

    [HttpGet("{meterReadingID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Well), "wellID")]
    [EntityNotFound(typeof(Meter), "meterID")]
    [EntityNotFound(typeof(MeterReading), "meterReadingID")]
    [WithWaterAccountRolePermission(PermissionEnum.WellMeterReadingRights, RightsEnum.Read)]
    public async Task<ActionResult<MeterReadingDto>> GetMeterReadingByID([FromRoute] int geographyID, [FromRoute] int wellID, [FromRoute] int meterID, [FromRoute] int meterReadingID)
    {
        var meterReadingDto = await MeterReadings.GetByIDAsDtoAsync(_dbContext, geographyID, wellID, meterID, meterReadingID);
        return Ok(meterReadingDto);
    }

    [HttpGet("last-reading/{dateAsString}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Well), "wellID")]
    [EntityNotFound(typeof(Meter), "meterID")]
    [WithWaterAccountRolePermission(PermissionEnum.WellMeterReadingRights, RightsEnum.Read)]
    public async Task<ActionResult<MeterReadingDto>> GetLastReadingFromDate([FromRoute] int geographyID, [FromRoute] int wellID, [FromRoute] int meterID, [FromRoute] string dateAsString)
    {
        const string dateFormat = "yyyy-MM-dd";
        var dateParsedSuccessfully = DateTime.TryParseExact(dateAsString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);

        if (!dateParsedSuccessfully)
        {
            return BadRequest($"Invalid date format. Expected {dateFormat}.");
        }

        if(date <= (DateTime)SqlDateTime.MinValue || date >= (DateTime)SqlDateTime.MaxValue)
        {
            ModelState.AddModelError("Date", $"Date needs to be after {(DateTime) SqlDateTime.MinValue:d} and before {(DateTime)SqlDateTime.MaxValue:d}");
            return BadRequest(ModelState);
        }

        var meterReadingDto = await MeterReadings.GetLastReadingFromDateAsync(_dbContext, geographyID, wellID, meterID, date);
        return Ok(meterReadingDto);
    }

    [HttpPut("{meterReadingID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Well), "wellID")]
    [EntityNotFound(typeof(Meter), "meterID")]
    [EntityNotFound(typeof(MeterReading), "meterReadingID")]
    [WithWaterAccountRolePermission(PermissionEnum.WellMeterReadingRights, RightsEnum.Update)]
    public async Task<ActionResult<MeterReadingDto>> UpdateWellMeterReading([FromRoute] int geographyID, [FromRoute] int wellID, [FromRoute] int meterID, [FromRoute] int meterReadingID, [FromBody] MeterReadingUpsertDto wellMeterReadingUpsertDto)
    {
        var errors = MeterReadings.ValidateMeterReadingUpsertAsync(_dbContext, geographyID, wellID, meterID, wellMeterReadingUpsertDto, meterReadingID);
        if (errors.Any())
        {
            errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
            return BadRequest(ModelState);
        }

        var wellMeterReadingSimpleDto = await MeterReadings.UpdateAsync(_dbContext, geographyID, wellID, meterID, meterReadingID, wellMeterReadingUpsertDto);
        return Ok(wellMeterReadingSimpleDto);
    }
}