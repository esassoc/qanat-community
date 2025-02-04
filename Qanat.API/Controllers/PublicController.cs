using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Hosting;
using Qanat.Models.DataTransferObjects.SupportTicket;
using Qanat.Models.DataTransferObjects.Geography;
using Qanat.API.Services.Authorization;

namespace Qanat.API.Controllers;

[ApiController]
[Route("public")]
public class PublicController : SitkaController<PublicController>
{
    public PublicController(QanatDbContext dbContext, ILogger<PublicController> logger,
        IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("system-info")]
    [AllowAnonymous]
    public ActionResult<SystemInfoDto> GetSystemInfo([FromServices] IWebHostEnvironment environment)
    {
        SystemInfoDto systemInfo = new SystemInfoDto
        {
            Environment = environment.EnvironmentName,
            CurrentTimeUTC = DateTime.UtcNow.ToString("o"),
            PodName = _qanatConfiguration.HostName
        };
        return Ok(systemInfo);
    }


    [HttpGet("customRichTexts/{customRichTextTypeID}")]
    [AllowAnonymous]
    public ActionResult<CustomRichTextDto> GetCustomRichText([FromRoute] int customRichTextTypeID, [FromQuery] int? geographyID)
    {
        var customRichTextDto = CustomRichText.GetByCustomRichTextTypeID(_dbContext, customRichTextTypeID, geographyID);
        return RequireNotNullLogIfNotFound(customRichTextDto, "CustomRichText", customRichTextTypeID);
    }

    [HttpGet("faq")]
    [AllowAnonymous]
    public ActionResult<List<FrequentlyAskedQuestionGridDto>> Get()
    {
        var faqs = FrequentlyAskedQuestions.GetAllFaqAsGridDto(_dbContext);
        return Ok(faqs);
    }

    [HttpGet("faq/{frequentlyAskedQuestionID}")]
    [AllowAnonymous]
    public ActionResult<FrequentlyAskedQuestionGridDto> GetByID([FromRoute] int frequentlyAskedQuestionID)
    {
        var returnDto = FrequentlyAskedQuestions.GetFaqByIDAsGridDto(_dbContext, frequentlyAskedQuestionID);
        return Ok(returnDto);
    }

    [HttpGet("faq/location/{faqDisplayQuestionLocationTypeID}")]
    [AllowAnonymous]
    public ActionResult<List<FrequentlyAskedQuestionLocationDisplayDto>> GetByLocationID([FromRoute] int faqDisplayQuestionLocationTypeID)
    {
        var frequentlyAskedQuestions = FrequentlyAskedQuestions.GetByLocationID(_dbContext, faqDisplayQuestionLocationTypeID);
        return Ok(frequentlyAskedQuestions);
    }

    [HttpGet("states")]
    [AllowAnonymous]
    public ActionResult<List<StateSimpleDto>> StatesList()
    {
        var stateList = State.AllAsSimpleDto;
        return Ok(stateList);
    }

    [HttpGet("geographies")]
    [AllowAnonymous]
    public ActionResult<List<GeographyPublicDto>> GeographiesList()
    {
        var geographyList = Geographies.ListAsPublicDto(_dbContext);
        return Ok(geographyList);
    }

    [HttpGet("geographies/name/{geographyName}")]
    [AllowAnonymous] // this is used on public geography dashboard
    public ActionResult<GeographyPublicDto> GetGeographyByName([FromRoute] string geographyName)
    {
        var geographyDto = Geographies.GetByNameAsPublicDto(_dbContext, geographyName);
        if (geographyDto == null)
        {
            return NotFound();
        }

        return geographyDto;
    }

    [HttpGet("geographies/boundingBox/{geographyName}")]
    [AllowAnonymous] // this is used on public geography dashboard
    public ActionResult<GeographyWithBoundingBoxDto> GetGeographyByNameWithBoundingBox([FromRoute] string geographyName)
    {
        var geography = _dbContext.Geographies.Include(x => x.GeographyBoundary).AsNoTracking()
            .SingleOrDefault(x => x.GeographyName == geographyName);
        if (geography == null) return NotFound(geographyName);
        var geographyWithBoundingBoxDto = new GeographyWithBoundingBoxDto()
        {
            GeographyID = geography.GeographyID,
            GeographyName = geography.GeographyName,
            GeographyDisplayName = geography.GeographyDisplayName,
            BoundingBox = new BoundingBoxDto(new List<Geometry>() { geography.GeographyBoundary.BoundingBox })
        };
        return Ok(geographyWithBoundingBoxDto);
    }

    [HttpGet("geographyBoundaries")]
    [AllowAnonymous]
    public ActionResult<List<GeographyBoundarySimpleDto>> ListBoundaries()
    {
        var geographyBoundarySimpleDtos =
            _dbContext.GeographyBoundaries.AsNoTracking().Select(x => x.AsSimpleDto()).ToList();
        return Ok(geographyBoundarySimpleDtos);
    }

    [HttpGet("geographies/{geographyID}/allocation-plans")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [AllowAnonymous]
    public ActionResult<List<AllocationPlanMinimalDto>> ListAllocationPlansByGeographyID([FromRoute] int geographyID)
    {
        var allocationPlanMinimalDtos = AllocationPlans.ListByGeographyIDAsMinimalDto(_dbContext, geographyID);
        return allocationPlanMinimalDtos;
    }

    [HttpGet("geographies/{geographyID}/allocation-plan-configuration/description")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [AllowAnonymous]
    public ActionResult<string> GetAllocationPlanConfigurationDescriptionByGeographyID([FromRoute] int geographyID)
    {
        var geographyAllocationPlanConfiguration = GeographyAllocationPlanConfigurations.GetByGeographyID(_dbContext, geographyID);
        return Ok(geographyAllocationPlanConfiguration.AllocationPlansDescription);
    }

    [HttpGet("geographies/{geographyID}/allocation-plans/{waterTypeSlug}/{zoneSlug}")]
    [AllowAnonymous]
    public ActionResult<AllocationPlanManageDto> GetAllocationPlanByWaterTypeSlugAndZoneSlug([FromRoute] int geographyID, [FromRoute] string waterTypeSlug, [FromRoute] string zoneSlug)
    {
        var allocationPlan = AllocationPlans.GetAllocationPlanManageDto(_dbContext, geographyID, waterTypeSlug, zoneSlug);
        if (allocationPlan == null)
        {
            return NotFound(
                $"Could not find an Allocation Plan with the waterTypeSlug of \"{waterTypeSlug}\" and the zoneSlug of \"{zoneSlug}\"");
        }

        return Ok(allocationPlan);
    }

    [HttpGet("geographies/{geographyID}/monitoring-wells")]
    [AllowAnonymous]
    public ActionResult<List<MonitoringWellDataDto>> GetAllMonitoringWellsForGeographyForGrid([FromRoute] int geographyID)
    {
        var monitoringWells = MonitoringWells.GetMonitoringWellsFromGeographyForGrid(_dbContext, geographyID);
        return Ok(monitoringWells.Select(x => x.AsMonitoringWellDataDto()));
    }

    [HttpGet("geographies/{geographyID}/monitoring-well/{siteCode}")]
    [AllowAnonymous]
    public ActionResult<List<MonitoringWellMeasurementDto>> GetMonitoringWellByGeographyAndSiteCode([FromRoute] int geographyID, string siteCode)
    {
        var monitoringWellMeasurements = MonitoringWells.GetMonitoringWell(_dbContext, geographyID, siteCode);
        return Ok(monitoringWellMeasurements);
    }

    [HttpGet("geographies/{geographyID}/zone-group/{zoneGroupSlug}")]
    [AllowAnonymous]
    public ActionResult<ZoneGroupMinimalDto> GetByID([FromRoute] int geographyID, string zoneGroupSlug)
    {
        return ZoneGroups.GetByZoneGroupSlugAsMinimalDto(_dbContext, zoneGroupSlug, geographyID);
    }

    [HttpPost("support-tickets/create")]
    [AllowAnonymous]
    public async Task CreateSupportTicket([FromForm] SupportTicketUpsertDto supportTicketUpsertDto, [FromForm] string token)
    {
        if (await RecaptchaValidator.IsValidResponseAsync(token, _qanatConfiguration.RecaptchaSecretKey, _qanatConfiguration.RecaptchaVerifyUrl, _qanatConfiguration.RecaptchaScoreThreshold) == false)
        {
            throw new Exception("Recaptcha validation failed. Please try again.");
        }
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext)?.UserID;
        await SupportTickets.CreateSupportTicket(_dbContext, supportTicketUpsertDto, userID);
    }
}