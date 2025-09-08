using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public partial class CustomRichText
    {
        public static CustomRichTextDto GetByCustomRichTextTypeID(QanatDbContext dbContext, int customRichTextTypeID, int? geographyID)
        {
            var customRichText = dbContext.CustomRichTexts
                .SingleOrDefault(x => x.CustomRichTextTypeID == customRichTextTypeID && x.GeographyID == geographyID);

            return customRichText?.AsCustomRichTextDto() ?? CreateIfNotExists(dbContext, customRichTextTypeID, geographyID).AsCustomRichTextDto();
        }

        public static CustomRichText CreateIfNotExists(QanatDbContext dbContext, int customRichTextTypeID, int? geographyID)
        {
            var newCustomRichText = new CustomRichText()
            {
                CustomRichTextTypeID = customRichTextTypeID,
                GeographyID = geographyID,
                CustomRichTextContent = $"<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</p>"
            };
            
            dbContext.CustomRichTexts.Add(newCustomRichText);
            dbContext.SaveChanges();
            dbContext.Entry(newCustomRichText).Reload();
            return newCustomRichText;
        }

        public static List<CustomRichTextDto> ListFieldDefinitions(QanatDbContext dbContext)
        {
            return dbContext.CustomRichTexts.ToList()
                .Where(x => x.CustomRichTextType.ContentTypeID == (int)ContentTypeEnum.FieldDefinition)
                .Select(x => x.AsCustomRichTextDto()).ToList();
        }

        public static CustomRichTextDto UpdateCustomRichText(QanatDbContext dbContext, int customRichTextTypeID,
            CustomRichTextSimpleDto customRichTextUpdateDto, int? geographyID)
        {
            var customRichText = dbContext.CustomRichTexts
                .SingleOrDefault(x => x.CustomRichTextTypeID == customRichTextTypeID && x.GeographyID == geographyID);

            // null check occurs in calling endpoint method.
            customRichText.CustomRichTextTitle = customRichTextUpdateDto.CustomRichTextTitle;
            customRichText.CustomRichTextContent = customRichTextUpdateDto.CustomRichTextContent;

            dbContext.SaveChanges();
            dbContext.Entry(customRichText).Reload();
            return customRichText.AsCustomRichTextDto();
        }
    }
}