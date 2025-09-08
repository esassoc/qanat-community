using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.SupportTicket;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qanat.API.Services.Attributes;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("support-tickets")]
public class SupportTicketController(
    QanatDbContext dbContext,
    ILogger<SupportTicketController> logger,
    IOptions<QanatConfiguration> qanatConfiguration,
    SitkaSmtpClientService sitkaSmtpClientService)
    : SitkaController<SupportTicketController>(dbContext, logger, qanatConfiguration)
{
    protected readonly SitkaSmtpClientService SitkaSmtpClientService = sitkaSmtpClientService;

    [HttpPost]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public async Task CreateSupportTicket([FromBody] SupportTicketUpsertDto supportTicketUpsertDto)
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext)?.UserID;
        var supportTicket = await SupportTickets.CreateSupportTicket(_dbContext, supportTicketUpsertDto, userID);

        var geographyManagerEmails = GeographyUsers.ListEmailAddressesForGeographyManagersWhoReceiveNotifications(_dbContext, supportTicketUpsertDto.GeographyID);
        if (geographyManagerEmails.Any())
        {
            await SitkaSmtpClientService.SendSupportTicketCreatedEmail(supportTicket, geographyManagerEmails);
        }

        if (supportTicketUpsertDto.AssignedUserID.HasValue)
        {
            await SendSupportTicketAssignedEmail(supportTicket);
        }
    }

    [HttpGet]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public ActionResult<List<SupportTicketGridDto>> ListAllSupportTickets()
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        var supportTickets = SupportTickets.GetSupportTicketsAsSimpleDto(_dbContext, userID);
        return supportTickets;
    }

    [HttpGet("{supportTicketID}")]
    [EntityNotFound(typeof(SupportTicket), "supportTicketID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public ActionResult<SupportTicketGridDto> GetSupportTicketByID([FromRoute] int supportTicketID)
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        var supportTickets = SupportTickets.GetSupportTicketByID(_dbContext, supportTicketID, userID);
        return supportTickets;
    }

    [HttpPut("{supportTicketID}")]
    [EntityNotFound(typeof(SupportTicket), "supportTicketID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public async Task<ActionResult> EditSupportTicket([FromRoute] int supportTicketID, [FromBody] SupportTicketUpsertDto supportTicketUpsertDto)
    {
        var supportTicket = _dbContext.SupportTickets.Single(x => x.SupportTicketID == supportTicketID);
        var newAssignedUser = supportTicketUpsertDto.AssignedUserID != supportTicket.AssignedUserID;
        await SupportTickets.UpdateSupportTicket(_dbContext, supportTicket, supportTicketUpsertDto);

        if (supportTicketUpsertDto.AssignedUserID.HasValue && newAssignedUser)
        {
            await SendSupportTicketAssignedEmail(supportTicket);
        }

        return Ok();
    }
    
    [HttpPut("{supportTicketID}/status")]
    [EntityNotFound(typeof(SupportTicket), "supportTicketID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public async Task<ActionResult> UpdateSupportTicketStatus([FromRoute] int supportTicketID, [FromBody] SupportTicketStatusUpdateDto supportTicketStatusUpdateDto)
    {
        var errors = SupportTickets.ValidateStatusUpdate(supportTicketStatusUpdateDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var supportTicket = _dbContext.SupportTickets.SingleOrDefault(x => x.SupportTicketID == supportTicketID);
        if (supportTicket == null)
        {
            return NotFound();
        }

        var newAssignedUser = supportTicketStatusUpdateDto.AssignedUserID.HasValue && supportTicketStatusUpdateDto.AssignedUserID != supportTicket.AssignedUserID;

        await SupportTickets.UpdateStatus(_dbContext, supportTicket, supportTicketStatusUpdateDto);

        if (supportTicketStatusUpdateDto.AssignedUserID.HasValue && newAssignedUser)
        {
            await SendSupportTicketAssignedEmail(supportTicket);
        }

        return Ok();
    }

    [HttpPost("{supportTicketID}/notes")]
    [EntityNotFound(typeof(SupportTicket), "supportTicketID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public async Task CreateSupportTicketNote([FromRoute] int supportTicketID,
        [FromBody] SupportTicketNoteSimpleDto supportTicketNoteSimpleDto)
    {
        supportTicketNoteSimpleDto.CreateUserID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        await SupportTicketNotes.CreateSupportTicketNote(_dbContext, supportTicketNoteSimpleDto);
        if (!supportTicketNoteSimpleDto.InternalNote)
        {
            await SendSupportTicketResponseEmail(supportTicketID, supportTicketNoteSimpleDto);
        }
    }

    [HttpGet("{supportTicketID}/notes")]
    [EntityNotFound(typeof(SupportTicket), "supportTicketID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard, true, true)]
    public ActionResult<List<SupportTicketNoteFeedDto>> GetSupportTicketNotesBySupportTicketID(
        [FromRoute] int supportTicketID)
    {
        var supportTickets = SupportTicketNotes.GetNotesForSupportTicket(_dbContext, supportTicketID);
        return Ok(supportTickets);
    }

    private async Task SendSupportTicketAssignedEmail(SupportTicket supportTicket)
    {
        var message = new SendGridMessage();

        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var assignedUser = _dbContext.Users.AsNoTracking()
            .Single(x => x.UserID == supportTicket.AssignedUserID);

        message.AddTo(new EmailAddress(assignedUser.Email, assignedUser.FullName));
        if (currentUser.UserID != assignedUser.UserID)
        {
            message.AddCc(new EmailAddress(currentUser.Email, currentUser.FullName));
        }

        var templateData = new SendGridBasicTextWithLinkTemplateData()
        {
            Subject = $"Help request #{supportTicket.SupportTicketID} has been assigned to you",
            Header = $"Help request #{supportTicket.SupportTicketID} has been assigned to you",
            Text = "A new help request has been assigned to you. View the request here: ",
            LinkUrl = $"{_qanatConfiguration.WEB_URL}/support-tickets/{supportTicket.SupportTicketID}",
            LinkText = $"Support Ticket #{supportTicket.SupportTicketID}"
        };

        await SitkaSmtpClientService.SendBasicTextWithLinkEmail(message, templateData);
    }

    private async Task SendSupportTicketResponseEmail(int supportTicketID, SupportTicketNoteSimpleDto supportTicketNote)
    {
        var supportTicket = _dbContext.SupportTickets.AsNoTracking()
            .Include(x => x.CreateUser)
            .Include(x => x.AssignedUser)
            .Include(x => x.Geography)
            .Single(x => x.SupportTicketID == supportTicketID);

        var message = new SendGridMessage();

        //MK 6/20/2025: ContactEmail doesn't appear to be required. If we have that use that, otherwise fall back to the CreateUser's email. 
        message.AddTo(!string.IsNullOrEmpty(supportTicket.ContactEmail) 
            ? new EmailAddress(supportTicket.ContactEmail, $"{supportTicket.ContactFirstName} {supportTicket.ContactLastName}") 
            : new EmailAddress(supportTicket.CreateUser.Email, supportTicket.CreateUser.FullName));

        var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        message.AddCc(new EmailAddress(currentUser.Email, currentUser.FullName));
        message.SetReplyTo(new EmailAddress(currentUser.Email, currentUser.FullName));

        if (supportTicket.AssignedUserID.HasValue && supportTicket.AssignedUserID != currentUser.UserID)
        {
            message.AddCc(new EmailAddress(supportTicket.AssignedUser.Email, supportTicket.AssignedUser.FullName));
        }

        var templateData = new SendGridSupportTicketResponseTemplateData()
        {
            Subject = $"Response to help request #{supportTicket.SupportTicketID}",
            RecipientFullName = $"{supportTicket.ContactFirstName}{(!string.IsNullOrEmpty(supportTicket.ContactLastName) ? $" {supportTicket.ContactLastName}" : "")}",
            QuestionTypeDisplayName = supportTicket.SupportTicketQuestionType?.SupportTicketQuestionTypeDisplayName,
            ResponseBody = supportTicketNote.Message,
            GeographyLongName = supportTicket.Geography.GeographyName
        };

        await SitkaSmtpClientService.SendSupportTicketResponseEmail(message, templateData);
    }
}