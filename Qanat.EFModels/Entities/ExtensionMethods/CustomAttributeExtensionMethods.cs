using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class CustomAttributeExtensionMethods
    {
        public static CustomAttributeSimpleDto AsSimpleDto(this CustomAttribute customAttribute)
        {
            var dto = new CustomAttributeSimpleDto()
            {
                CustomAttributeID = customAttribute.CustomAttributeID,
                GeographyID = customAttribute.GeographyID,
                CustomAttributeTypeID = customAttribute.CustomAttributeTypeID,
                CustomAttributeName = customAttribute.CustomAttributeName
            };
            return dto;
        }
    }
}