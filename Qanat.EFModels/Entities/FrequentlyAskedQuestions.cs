using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class FrequentlyAskedQuestions
{
    public static List<FrequentlyAskedQuestionGridDto> GetAllFaqAsGridDto(QanatDbContext dbContext)
    {
        return dbContext.FrequentlyAskedQuestions.AsNoTracking()
            .Include(x => x.FrequentlyAskedQuestionFaqDisplayLocationTypes)
            .Select(x => x.AsGridDto()).ToList();
    }

    public static FrequentlyAskedQuestionGridDto GetFaqByIDAsGridDto(QanatDbContext dbContext, int frequentlyAskedQuestionID)
    {
        var frequentlyAskedQuestion = dbContext.FrequentlyAskedQuestions.AsNoTracking()
            .Include(x => x.FrequentlyAskedQuestionFaqDisplayLocationTypes)
            .Single(x => x.FrequentlyAskedQuestionID == frequentlyAskedQuestionID);
        var returnDto = frequentlyAskedQuestion.AsGridDto();
        return returnDto;
    }

    public static List<FrequentlyAskedQuestionLocationDisplayDto> GetByLocationID(QanatDbContext dbContext, int faqDisplayQuestionLocationTypeID)
    {
        var frequentlyAskedQuestions = dbContext.FrequentlyAskedQuestions.AsNoTracking()
            .Include(x => x.FrequentlyAskedQuestionFaqDisplayLocationTypes)
            .Where(x => x.FrequentlyAskedQuestionFaqDisplayLocationTypes.Select(y => y.FaqDisplayLocationTypeID)
                .Contains(faqDisplayQuestionLocationTypeID)).Select(x => x.AsLocationDisplayDto(faqDisplayQuestionLocationTypeID)).ToList();

        return frequentlyAskedQuestions.OrderBy(x => x.SortOrder).ToList();
    }

    public static FrequentlyAskedQuestionSimpleDto CreateFaq(QanatDbContext dbContext, FrequentlyAskedQuestionAdminFormDto faq)
    {
        faq.FixNullFaqDisplayLocationTypeIDs();

        var askedQuestionFaqDisplayLocationTypes = dbContext.FrequentlyAskedQuestionFaqDisplayLocationTypes
            .Where(x => faq.FaqDisplayLocationTypeIDs.Contains(x.FaqDisplayLocationTypeID))
            .ToList();

        var question = new FrequentlyAskedQuestion()
        {
            QuestionText = faq.QuestionText,
            AnswerText = faq.AnswerText,
            FrequentlyAskedQuestionFaqDisplayLocationTypes = faq.FaqDisplayLocationTypeIDs.Select(x => new FrequentlyAskedQuestionFaqDisplayLocationType()
            {
                FaqDisplayLocationTypeID = x,
                SortOrder = askedQuestionFaqDisplayLocationTypes.Where(y => y.FaqDisplayLocationTypeID == x).ToList().Count == 0 
                ? 0
                : askedQuestionFaqDisplayLocationTypes.Where(y => y.FaqDisplayLocationTypeID == x).OrderByDescending(y => y.SortOrder).First().SortOrder + 1
            }).ToList(),
        };

        dbContext.FrequentlyAskedQuestions.Add(question);
        dbContext.SaveChanges();
        dbContext.Entry(question).Reload();

        var frequentlyAskedQuestionSimpleDto = dbContext.FrequentlyAskedQuestions.Single(x => x.FrequentlyAskedQuestionID == question.FrequentlyAskedQuestionID).AsSimpleDto();
        return frequentlyAskedQuestionSimpleDto;
    }

    public static FrequentlyAskedQuestionSimpleDto UpdateFaq(QanatDbContext dbContext, FrequentlyAskedQuestionAdminFormDto faq)
    {
        faq.FixNullFaqDisplayLocationTypeIDs();

        var question = dbContext.FrequentlyAskedQuestions
            .Include(x => x.FrequentlyAskedQuestionFaqDisplayLocationTypes)
            .Single(x => x.FrequentlyAskedQuestionID == faq.FrequentlyAskedQuestionID);

        question.QuestionText = faq.QuestionText;
        question.AnswerText = faq.AnswerText;

        if (question.FrequentlyAskedQuestionFaqDisplayLocationTypes.Count != faq.FaqDisplayLocationTypeIDs.Count)
        {
            var existingFaqLocations =
                dbContext.FrequentlyAskedQuestionFaqDisplayLocationTypes.AsNoTracking()
                    .Where(x =>
                        faq.FaqDisplayLocationTypeIDs.Contains(x.FaqDisplayLocationTypeID)).ToList();

            var updated = faq.FaqDisplayLocationTypeIDs
                .Select(x =>
                {
                    var frequentlyAskedQuestionFaqDisplayLocationType = new FrequentlyAskedQuestionFaqDisplayLocationType()
                    {
                        FrequentlyAskedQuestionID = faq.FrequentlyAskedQuestionID!.Value,
                        FaqDisplayLocationTypeID = x
                    };
                    if (existingFaqLocations.Count == 0)
                    {
                        frequentlyAskedQuestionFaqDisplayLocationType.SortOrder = 0;
                    }
                    else
                    {
                        var sortOrder = existingFaqLocations.SingleOrDefault(
                            y =>
                                y.FaqDisplayLocationTypeID == x && y.FrequentlyAskedQuestionID ==
                                (int)faq.FrequentlyAskedQuestionID)?.SortOrder;
                        var frequentlyAskedQuestionFaqDisplayLocationTypes = existingFaqLocations.Where(y => y.FaqDisplayLocationTypeID == x).ToList();
                        var sortOrder2 = frequentlyAskedQuestionFaqDisplayLocationTypes.Any() ? frequentlyAskedQuestionFaqDisplayLocationTypes
                            .Max(y => y.SortOrder) + 1 : 0;
                        frequentlyAskedQuestionFaqDisplayLocationType.SortOrder = sortOrder ?? sortOrder2;
                    }

                    return frequentlyAskedQuestionFaqDisplayLocationType;
                }).ToList();


            question.FrequentlyAskedQuestionFaqDisplayLocationTypes
                .Merge(updated, dbContext.FrequentlyAskedQuestionFaqDisplayLocationTypes,
                (a, b) => a.FaqDisplayLocationTypeID == b.FaqDisplayLocationTypeID &&
                          a.FrequentlyAskedQuestionID == b.FrequentlyAskedQuestionID);
        }

        dbContext.SaveChanges();
        dbContext.Entry(question).Reload();

        var frequentlyAskedQuestionSimpleDto = dbContext.FrequentlyAskedQuestions.Single(x => x.FrequentlyAskedQuestionID == question.FrequentlyAskedQuestionID).AsSimpleDto();
        return frequentlyAskedQuestionSimpleDto;
    }

    public static List<FrequentlyAskedQuestionLocationDisplayDto> UpdateFaqForLocationType(QanatDbContext dbContext, int faqDisplayLocationTypeID, List<FrequentlyAskedQuestionGridDto> faqSimpleDtos)
    {
        var faqs = new List<FrequentlyAskedQuestionFaqDisplayLocationType>();
        var frequentlyAskedQuestionFaqDisplayLocationTypes = dbContext.FrequentlyAskedQuestionFaqDisplayLocationTypes;
        var selectedFaqIDs = faqSimpleDtos.Select(x => x.FrequentlyAskedQuestionID).ToList();
        var sortOrder = 1;
        foreach (var faqDto in faqSimpleDtos)
        {
            var containsFaq = frequentlyAskedQuestionFaqDisplayLocationTypes
                .Where(x => x.FaqDisplayLocationTypeID == faqDisplayLocationTypeID)
                .Select(x => x.FrequentlyAskedQuestionID)
                .Contains(faqDto.FrequentlyAskedQuestionID);
            if (containsFaq)
            {
                var faq = frequentlyAskedQuestionFaqDisplayLocationTypes.Single(x =>
                    x.FaqDisplayLocationTypeID == faqDisplayLocationTypeID &&
                    x.FrequentlyAskedQuestionID == faqDto.FrequentlyAskedQuestionID);
                faq.SortOrder = sortOrder;
            }
            else
            {
                faqs.Add(new FrequentlyAskedQuestionFaqDisplayLocationType()
                {
                    FaqDisplayLocationTypeID = faqDisplayLocationTypeID,
                    FrequentlyAskedQuestionID = faqDto.FrequentlyAskedQuestionID,
                    SortOrder = sortOrder
                });
            }

            sortOrder++;
        }

        var frequentlyAskedQuestionIDs = frequentlyAskedQuestionFaqDisplayLocationTypes
            .Where(x => x.FaqDisplayLocationTypeID == faqDisplayLocationTypeID)
            .Select(x => x.FrequentlyAskedQuestionID).ToList();
        foreach (var faqID in frequentlyAskedQuestionIDs.Where(faqID => !selectedFaqIDs.Contains(faqID)))
        {
            frequentlyAskedQuestionFaqDisplayLocationTypes.Remove(
                frequentlyAskedQuestionFaqDisplayLocationTypes.Single(x =>
                    x.FrequentlyAskedQuestionID == faqID &&
                    x.FaqDisplayLocationTypeID == faqDisplayLocationTypeID));
        }
        dbContext.FrequentlyAskedQuestionFaqDisplayLocationTypes.AddRange(faqs);
        dbContext.SaveChanges();
        return GetByLocationID(dbContext, faqDisplayLocationTypeID);
    }


    public static void DeleteFaq(QanatDbContext dbContext, int frequentlyAskedQuestionID)
    {
        var question =
            dbContext.FrequentlyAskedQuestions.Single(x => x.FrequentlyAskedQuestionID == frequentlyAskedQuestionID);
        dbContext.Remove(question);
        dbContext.SaveChanges();
    }
}