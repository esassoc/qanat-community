using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class OpenETDataTypeExtensionMethods
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