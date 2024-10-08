//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ExternalMapLayer]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ExternalMapLayerExtensionMethods
    {
        public static ExternalMapLayerSimpleDto AsSimpleDto(this ExternalMapLayer externalMapLayer)
        {
            var dto = new ExternalMapLayerSimpleDto()
            {
                ExternalMapLayerID = externalMapLayer.ExternalMapLayerID,
                GeographyID = externalMapLayer.GeographyID,
                ExternalMapLayerDisplayName = externalMapLayer.ExternalMapLayerDisplayName,
                ExternalMapLayerTypeID = externalMapLayer.ExternalMapLayerTypeID,
                ExternalMapLayerURL = externalMapLayer.ExternalMapLayerURL,
                LayerIsOnByDefault = externalMapLayer.LayerIsOnByDefault,
                IsActive = externalMapLayer.IsActive,
                ExternalMapLayerDescription = externalMapLayer.ExternalMapLayerDescription,
                PopUpField = externalMapLayer.PopUpField,
                MinZoom = externalMapLayer.MinZoom
            };
            return dto;
        }
    }
}