using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.SupportTicket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("support-tickets")]
public class SupportTicketController : SitkaController<SupportTicketController>
{
    protected readonly SitkaSmtpClientService _sitkaSmtpClientService;
    public SupportTicketController(QanatDbContext dbContext, ILogger<SupportTicketController> logger, IOptions<QanatConfiguration> qanatConfiguration, SitkaSmtpClientService sitkaSmtpClientService) : base(dbContext, logger, qanatConfiguration)
    {
        _sitkaSmtpClientService=sitkaSmtpClientService;
    }

    [HttpGet()]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public ActionResult<List<SupportTicketGridDto>> ListAllSupportTickets()
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        var supportTickets = SupportTickets.GetSupportTicketsAsSimpleDto(_dbContext, userID);
        return supportTickets;
    }

    [HttpGet("{supportTicketID}")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public ActionResult<SupportTicketGridDto> GetSupportTicketByID([FromRoute] int supportTicketID)
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        var supportTickets = SupportTickets.GetSupportTicketByID(_dbContext, supportTicketID, userID);
        return supportTickets;
    }

    [HttpGet("{supportTicketID}/notes")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public ActionResult<List<SupportTicketNoteFeedDto>> GetSupportTicketNotesBySupportTicketID([FromRoute] int supportTicketID)
    {
        var supportTickets = SupportTicketNotes.GetNotesForSupportTicket(_dbContext, supportTicketID);
        return Ok(supportTickets);
    }

    [HttpPost("{supportTicketID}/notes")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public async Task CreateSupportTicketNote([FromRoute] int supportTicketID, [FromBody] SupportTicketNoteSimpleDto supportTicketNoteSimpleDto)
    {
        supportTicketNoteSimpleDto.CreateUserID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        await SupportTicketNotes.CreateSupportTicketNote(_dbContext, supportTicketNoteSimpleDto);
        if (!supportTicketNoteSimpleDto.InternalNote)
        {
            var mailMessageActiveUser = SupportTicketNotes.SendResponseForSupportTicket(_dbContext, supportTicketNoteSimpleDto);
            await _sitkaSmtpClientService.Send(mailMessageActiveUser);
        }
    }

    [HttpPost("create")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public async Task CreateSupportTicket([FromBody] SupportTicketUpsertDto supportTicketUpsertDto)
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext)?.UserID;
        await SupportTickets.CreateSupportTicket(_dbContext, supportTicketUpsertDto, userID);
    }

    [HttpPut("edit")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public async Task EditSupportTicket([FromBody] SupportTicketUpsertDto supportTicketUpsertDto)
    {
        await SupportTickets.UpdateSupportTicket(_dbContext, supportTicketUpsertDto);
    }

    [HttpPut("{supportTicketID}/reopen")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public async Task ReopenSupportTicket([FromRoute] int supportTicketID)
    {
        await SupportTickets.ReopenSupportTicket(_dbContext, supportTicketID);
    }

    [HttpPut("{supportTicketID}/close")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public async Task CloseSupportTicket([FromRoute] int supportTicketID)
    {
        await SupportTickets.CloseSupportTicket(_dbContext, supportTicketID);
    }
}