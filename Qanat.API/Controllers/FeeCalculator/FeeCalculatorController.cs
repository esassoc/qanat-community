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

        var reportingPeriods = await ReportingPeriods.ListByGeographyIDAsync(_dbContext, geography.GeographyID, callingUser);
        var defaultReportingPeriod = reportingPeriods.FirstOrDefault(x => x.IsDefault) ?? reportingPeriods.First();

        //MK 10/23/2024 -- Only supporting ETSGSA for now. We can generalize this later.
        var feeStructures = FeeStructuresDtos.ETSGSA_FeeStructures; 
        var mlrpIncentives = MLRPIncentiveDtos.ETSGSA_MLRPIncentives.OrderBy(x => x.DisplayOrder).ToList();

        var initialInputs = new FeeCalculatorInputDto()
        {
            WaterAccountID = geographyWaterAccounts.First().WaterAccountID,
            ReportingPeriodID = defaultReportingPeriod.ReportingPeriodID,
            FeeStructureID = feeStructures.First().FeeStructureID,
            SurfaceWaterDelivered = null,
            SurfaceWaterIrrigationEfficiency = null,
            MLRPIncentives = mlrpIncentives
        };

        reportingPeriods.ForEach(x => x.Name = $"{x.Name} ({x.StartDate:MMM d, yyyy} - {x.EndDate:MMM d, yyyy})");

        var initialOutput = await FeeCalculatorHelper.CalculateFee(_dbContext, geography, initialInputs);
        var inputOptions = new FeeCalculatorInputOptionsDto()
        {
            Geography = geography,
            WaterAccounts = geographyWaterAccounts,
            ReportingPeriods = reportingPeriods,
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