//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ExternalMapLayerType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ExternalMapLayerTypeExtensionMethods
    {
        public static ExternalMapLayerTypeSimpleDto AsSimpleDto(this ExternalMapLayerType externalMapLayerType)
        {
            var dto = new ExternalMapLayerTypeSimpleDto()
            {
                ExternalMapLayerTypeID = externalMapLayerType.ExternalMapLayerTypeID,
                ExternalMapLayerTypeName = externalMapLayerType.ExternalMapLayerTypeName,
                ExternalMapLayerTypeDisplayName = externalMapLayerType.ExternalMapLayerTypeDisplayName
            };
            return dto;
        }
    }
}