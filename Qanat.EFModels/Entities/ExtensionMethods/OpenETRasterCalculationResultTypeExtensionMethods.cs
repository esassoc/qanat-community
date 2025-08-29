using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class OpenETRasterCalculationResultTypeExtensionMethods
    {
        public static OpenETRasterCalculationResultTypeSimpleDto AsSimpleDto(this OpenETRasterCalculationResultType openETRasterCalculationResultType)
        {
            var dto = new OpenETRasterCalculationResultTypeSimpleDto()
            {
                OpenETRasterCalculationResultTypeID = openETRasterCalculationResultType.OpenETRasterCalculationResultTypeID,
                OpenETRasterCalculationResultTypeName = openETRasterCalculationResultType.OpenETRasterCalculationResultTypeName,
                OpenETRasterCalculationResultTypeDisplayName = openETRasterCalculationResultType.OpenETRasterCalculationResultTypeDisplayName
            };
            return dto;
        }
    }
}