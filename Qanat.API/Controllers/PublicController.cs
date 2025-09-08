using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;
using Qanat.Models.DataTransferObjects.SupportTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[Route("public")]
public class PublicController(
    QanatDbContext dbContext,
    ILogger<PublicController> logger,
    IOptions<QanatConfiguration> qanatConfiguration,
    SitkaSmtpClientService sitkaSmtpClientService)
    : SitkaController<PublicController>(dbContext, logger, qanatConfiguration)
{
    protected readonly SitkaSmtpClientService _sitkaSmtpClientService = sitkaSmtpClientService;

    [HttpGet("system-info")]
    [AllowAnonymous]
    [LogIgnore]
    public ActionResult<SystemInfoDto> GetSystemInfo([FromServices] IWebHostEnvironment environment)
    {
        var systemInfo = new SystemInfoDto
        {
            Environment = environment.EnvironmentName,
            CurrentTimeUTC = DateTime.UtcNow.ToString("o"),
            PodName = _qanatConfiguration.HostName
        };

        return Ok(systemInfo);
    }

    [HttpGet("custom-rich-texts/{customRichTextTypeID}")]
    [AllowAnonymous]
    public ActionResult<CustomRichTextDto> GetCustomRichText([FromRoute] int customRichTextTypeID, [FromQuery] int? geographyID)
    {
        var customRichTextDto = CustomRichText.GetByCustomRichTextTypeID(_dbContext, customRichTextTypeID, geographyID);
        return RequireNotNullLogIfNotFound(customRichTextDto, "CustomRichText", customRichTextTypeID);
    }

    [HttpGet("faqs")]
    [AllowAnonymous]
    public ActionResult<List<FrequentlyAskedQuestionGridDto>> ListFrequentlyAskedQuestions()
    {
        var faqs = FrequentlyAskedQuestions.GetAllFaqAsGridDto(_dbContext);
        return Ok(faqs);
    }

    [HttpGet("faqs/{frequentlyAskedQuestionID}")]
    [AllowAnonymous]
    public ActionResult<FrequentlyAskedQuestionGridDto> GetFrequentlyAskedQuestionByID([FromRoute] int frequentlyAskedQuestionID)
    {
        var returnDto = FrequentlyAskedQuestions.GetFaqByIDAsGridDto(_dbContext, frequentlyAskedQuestionID);
        return Ok(returnDto);
    }

    [HttpGet("faqs/location/{faqDisplayQuestionLocationTypeID}")]
    [AllowAnonymous]
    public ActionResult<List<FrequentlyAskedQuestionLocationDisplayDto>> ListFrequentlyAskedQuestionsByLocationTypeID([FromRoute] int faqDisplayQuestionLocationTypeID)
    {
        var frequentlyAskedQuestions = FrequentlyAskedQuestions.GetByLocationID(_dbContext, faqDisplayQuestionLocationTypeID);
        return Ok(frequentlyAskedQuestions);
    }

    [HttpGet("states")]
    [AllowAnonymous]
    public ActionResult<List<StateSimpleDto>> StatesList()
    {
        var stateList = State.All.Select(x => x.AsSimpleDto());
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
        var geography = _dbContext.Geographies.AsNoTracking()
            .Include(x => x.GeographyBoundary)
            .Include(x => x.GeographyAllocationPlanConfiguration)
            .SingleOrDefault(x => x.GeographyName == geographyName);

        if (geography == null)
        {
            return NotFound(geographyName);
        }

        var geographyWithBoundingBoxDto = new GeographyWithBoundingBoxDto()
        {
            GeographyID = geography.GeographyID,
            GeographyName = geography.GeographyName,
            GeographyDisplayName = geography.GeographyDisplayName,
            BoundingBox = new BoundingBoxDto(new List<Geometry>() { geography.GeographyBoundary.BoundingBox }),
            AllocationPlansVisibleToLandowners = geography.GeographyAllocationPlanConfiguration?.IsVisibleToLandowners ?? false,
            AllocationPlansVisibleToPublic = geography.GeographyAllocationPlanConfiguration?.IsVisibleToPublic ?? false,
        };

        return Ok(geographyWithBoundingBoxDto);
    }

    [HttpGet("geography-boundaries")]
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
    public ActionResult<ZoneGroupMinimalDto> GetZoneGroupBySlug([FromRoute] int geographyID, string zoneGroupSlug)
    {
        return ZoneGroups.GetByZoneGroupSlugAsMinimalDto(_dbContext, geographyID, zoneGroupSlug, false);
    }

    [HttpPost("support-tickets")]
    [AllowAnonymous]
    public async Task<ActionResult> CreateSupportTicket([FromBody] SupportTicketUpsertDto supportTicketUpsertDto)
    {
        if (await RecaptchaValidator.IsValidResponseAsync(supportTicketUpsertDto.RecaptchaToken, _qanatConfiguration.RecaptchaSecretKey, _qanatConfiguration.RecaptchaVerifyUrl, _qanatConfiguration.RecaptchaScoreThreshold) == false)
        {
            ModelState.AddModelError("Recaptcha", "Recaptcha validation failed. Please try again.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext)?.UserID;
        var supportTicket = await SupportTickets.CreateSupportTicket(_dbContext, supportTicketUpsertDto, userID);

        var geographyManagerEmails = GeographyUsers.ListEmailAddressesForGeographyManagersWhoReceiveNotifications(_dbContext, supportTicketUpsertDto.GeographyID);
        if (geographyManagerEmails.Any())
        {
            await _sitkaSmtpClientService.SendSupportTicketCreatedEmail(supportTicket, geographyManagerEmails);
        }

        return Ok();
    }
}