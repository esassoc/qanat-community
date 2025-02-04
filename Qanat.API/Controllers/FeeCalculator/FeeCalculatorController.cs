using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Helpers.FeeCalculator;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("fee-calculator/{geographyName}")]
public class FeeCalculatorController(QanatDbContext dbContext, ILogger<FeeCalculatorController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser) : SitkaController<FeeCalculatorController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet("inputs")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public async Task<ActionResult<FeeCalculatorInputOptionsDto>> GetFeeCalculatorInputs([FromRoute] string geographyName)
    {
        var geography = Geographies.GetByNameAsPublicDto(_dbContext, geographyName);
        if (geography == null)
        {
            return NotFound();
        }

        if (!geography.FeeCalculatorEnabled)
        {
            return BadRequest($"Fee calculator is not enabled for {geography.FeeCalculatorEnabled}.");
        }

        var waterAccounts = WaterAccounts.ListByUserAsMinimalDtos(_dbContext, callingUser);
        var geographyWaterAccounts = waterAccounts.Where(x => x.GeographyID == geography.GeographyID).ToList();

        if (geographyWaterAccounts.Count == 0)
        {
            return BadRequest($"You have no water accounts in {geography.GeographyDisplayName}.");
        }

        //TODO: turn this into user entered configuration, either a Bit on Reporting Period or a list of Reporting PeriodIDs for a geography.
        var reportingPeriodYears = FeeCalculatorYearDtos.ETSGSA_ReportingPeriods;

        //MK 10/23/2024 -- Only supporting ETSGSA for now. We can generalize this later.
        var feeStructures = FeeStructuresDtos.ETSGSA_FeeStructures; 
        var mlrpIncentives = MLRPIncentiveDtos.ETSGSA_MLRPIncentives.OrderBy(x => x.DisplayOrder).ToList();

        var initialInputs = new FeeCalculatorInputDto()
        {
            ReportingYear = reportingPeriodYears.First(x => x.Year == 2023).Year, //TODO: This should be the default reporting period on the geography.
            WaterAccountID = geographyWaterAccounts.First().WaterAccountID,
            FeeStructureID = feeStructures.First().FeeStructureID,
            SurfaceWaterDelivered = null,
            SurfaceWaterIrrigationEfficiency = null,
            MLRPIncentives = mlrpIncentives
        };

        var initialOutput = await FeeCalculatorHelper.CalculateFee(_dbContext, geography, initialInputs);
        var inputOptions = new FeeCalculatorInputOptionsDto()
        {
            Geography = geography,
            WaterAccounts = geographyWaterAccounts,
            Years = reportingPeriodYears,
            FeeStructures = feeStructures,
            MLRPIncentives = mlrpIncentives,

            InitialInputs = initialInputs,
            InitialOutput = initialOutput
        };

        return Ok(inputOptions);
    }

    [HttpPost("calculate")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public async Task<ActionResult<FeeCalculatorOutputDto>> CalculateFee([FromRoute] string geographyName, [FromBody] FeeCalculatorInputDto feeCalculatorInput)
    {
        var geography = Geographies.GetByNameAsPublicDto(_dbContext, geographyName);
        if (geography == null)
        {
            return NotFound();
        }

        if (!geography.FeeCalculatorEnabled)
        {
            return BadRequest($"Fee calculator is not enabled for {geography.GeographyDisplayName}.");
        }
        
        var output = await FeeCalculatorHelper.CalculateFee(_dbContext, geography, feeCalculatorInput);
        return  Ok(output);
    }
}