//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETDataType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class OpenETDataTypeExtensionMethods
    {
        public static OpenETDataTypeSimpleDto AsSimpleDto(this OpenETDataType openETDataType)
        {
            var dto = new OpenETDataTypeSimpleDto()
            {
                OpenETDataTypeID = openETDataType.OpenETDataTypeID,
                OpenETDataTypeName = openETDataType.OpenETDataTypeName,
                OpenETDataTypeDisplayName = openETDataType.OpenETDataTypeDisplayName,
                OpenETDataTypeVariableName = openETDataType.OpenETDataTypeVariableName
            };
            return dto;
        }
    }
}