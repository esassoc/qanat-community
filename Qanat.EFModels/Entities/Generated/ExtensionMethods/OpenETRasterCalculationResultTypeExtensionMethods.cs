//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETRasterCalculationResultType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class OpenETRasterCalculationResultTypeExtensionMethods
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