using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class FrequentlyAskedQuestionController : SitkaController<FrequentlyAskedQuestionController>
{
    public FrequentlyAskedQuestionController(QanatDbContext dbContext, ILogger<FrequentlyAskedQuestionController> logger, IOptions<QanatConfiguration> qanatConfiguration)
        : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpPost("faq")]
    [WithRolePermission(PermissionEnum.FrequentlyAskedQuestionRights, RightsEnum.Create)]
    public ActionResult<FrequentlyAskedQuestionSimpleDto> Create([FromBody] FrequentlyAskedQuestionAdminFormDto faq)
    {
        var frequentlyAskedQuestionSimpleDto = FrequentlyAskedQuestions.CreateFaq(_dbContext, faq);
        return Ok(frequentlyAskedQuestionSimpleDto);
    }

    [HttpPut("faq")]
    [WithRolePermission(PermissionEnum.FrequentlyAskedQuestionRights, RightsEnum.Update)]
    public ActionResult<FrequentlyAskedQuestionSimpleDto> Update([FromBody] FrequentlyAskedQuestionAdminFormDto faq)
    {
        var frequentlyAskedQuestionSimpleDto = FrequentlyAskedQuestions.UpdateFaq(_dbContext, faq);
        return Ok(frequentlyAskedQuestionSimpleDto);
    }

    [HttpPost("faqs/{faqDisplayLocationTypeID}")]
    [WithRolePermission(PermissionEnum.FrequentlyAskedQuestionRights, RightsEnum.Update)]
    public ActionResult<List<FrequentlyAskedQuestionLocationDisplayDto>> UpdateFaqDisplayLocationType([FromRoute] int faqDisplayLocationTypeID, [FromBody] List<FrequentlyAskedQuestionGridDto> faqSimpleDtos)
    {
        var frequentlyAskedQuestionLocationDisplayDtos = FrequentlyAskedQuestions.UpdateFaqForLocationType(_dbContext, faqDisplayLocationTypeID, faqSimpleDtos);
        return Ok(frequentlyAskedQuestionLocationDisplayDtos);
    }

    [HttpDelete("faq/{frequentlyAskedQuestionID}")]
    [WithRolePermission(PermissionEnum.FrequentlyAskedQuestionRights, RightsEnum.Delete)]
    public ActionResult Delete([FromRoute] int frequentlyAskedQuestionID)
    {
        FrequentlyAskedQuestions.DeleteFaq(_dbContext, frequentlyAskedQuestionID);
        return Ok();
    }

    [HttpGet("faq/display-locations")]
    [WithRolePermission(PermissionEnum.FrequentlyAskedQuestionRights, RightsEnum.Create)]
    public ActionResult<List<FaqDisplayLocationTypeSimpleDto>> GetDisplayLocationTypeEnum()
    {
        var faqDisplayLocationTypeSimpleDtos = FaqDisplayLocationType.AllAsSimpleDto;
        return Ok(faqDisplayLocationTypeSimpleDtos);
    }
}