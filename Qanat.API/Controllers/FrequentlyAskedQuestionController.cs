using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("faqs")]
public class FrequentlyAskedQuestionController(
    QanatDbContext dbContext,
    ILogger<FrequentlyAskedQuestionController> logger,
    IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<FrequentlyAskedQuestionController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost()]
    [WithRolePermission(PermissionEnum.FrequentlyAskedQuestionRights, RightsEnum.Create)]
    public ActionResult<FrequentlyAskedQuestionSimpleDto> Create([FromBody] FrequentlyAskedQuestionAdminFormDto faq)
    {
        var frequentlyAskedQuestionSimpleDto = FrequentlyAskedQuestions.CreateFaq(_dbContext, faq);
        return Ok(frequentlyAskedQuestionSimpleDto);
    }

    [HttpPut]
    [WithRolePermission(PermissionEnum.FrequentlyAskedQuestionRights, RightsEnum.Update)]
    public ActionResult<FrequentlyAskedQuestionSimpleDto> Update([FromBody] FrequentlyAskedQuestionAdminFormDto faq)
    {
        var frequentlyAskedQuestionSimpleDto = FrequentlyAskedQuestions.UpdateFaq(_dbContext, faq);
        return Ok(frequentlyAskedQuestionSimpleDto);
    }

    [HttpPost("{faqDisplayLocationTypeID}")]
    [WithRolePermission(PermissionEnum.FrequentlyAskedQuestionRights, RightsEnum.Update)]
    public ActionResult<List<FrequentlyAskedQuestionLocationDisplayDto>> UpdateFaqDisplayLocationType([FromRoute] int faqDisplayLocationTypeID, [FromBody] List<FrequentlyAskedQuestionGridDto> faqSimpleDtos)
    {
        var frequentlyAskedQuestionLocationDisplayDtos = FrequentlyAskedQuestions.UpdateFaqForLocationType(_dbContext, faqDisplayLocationTypeID, faqSimpleDtos);
        return Ok(frequentlyAskedQuestionLocationDisplayDtos);
    }

    [HttpDelete("{frequentlyAskedQuestionID}")]
    [EntityNotFound(typeof(FrequentlyAskedQuestion), "frequentlyAskedQuestionID")]
    [WithRolePermission(PermissionEnum.FrequentlyAskedQuestionRights, RightsEnum.Delete)]
    public ActionResult Delete([FromRoute] int frequentlyAskedQuestionID)
    {
        FrequentlyAskedQuestions.DeleteFaq(_dbContext, frequentlyAskedQuestionID);
        return Ok();
    }
}