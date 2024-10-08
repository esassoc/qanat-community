//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FaqDisplayLocationType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class FaqDisplayLocationTypeExtensionMethods
    {
        public static FaqDisplayLocationTypeSimpleDto AsSimpleDto(this FaqDisplayLocationType faqDisplayLocationType)
        {
            var dto = new FaqDisplayLocationTypeSimpleDto()
            {
                FaqDisplayLocationTypeID = faqDisplayLocationType.FaqDisplayLocationTypeID,
                FaqDisplayLocationTypeName = faqDisplayLocationType.FaqDisplayLocationTypeName,
                FaqDisplayLocationTypeDisplayName = faqDisplayLocationType.FaqDisplayLocationTypeDisplayName
            };
            return dto;
        }
    }
}