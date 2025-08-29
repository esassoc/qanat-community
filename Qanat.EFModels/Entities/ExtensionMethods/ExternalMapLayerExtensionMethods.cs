using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ExternalMapLayerExtensionMethods
{
    public static ExternalMapLayerSimpleDto AsSimpleDto(this ExternalMapLayer externalMapLayer)
    {
        var dto = new ExternalMapLayerSimpleDto()
        {
            ExternalMapLayerID = externalMapLayer.ExternalMapLayerID,
            GeographyID = externalMapLayer.GeographyID,
            ExternalMapLayerDisplayName = externalMapLayer.ExternalMapLayerDisplayName,
            ExternalMapLayerTypeID = externalMapLayer.ExternalMapLayerTypeID,
            ExternalMapLayerTypeDisplayName = externalMapLayer.ExternalMapLayerType.ExternalMapLayerTypeDisplayName,
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