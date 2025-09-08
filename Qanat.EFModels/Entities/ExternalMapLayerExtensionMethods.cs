using Qanat.Models.DataTransferObjects;
using System.Linq;

namespace Qanat.EFModels.Entities;


public partial class ExternalMapLayerExtensionMethods
{
    public static ExternalMapLayerDto AsExternalMapLayerDto(this ExternalMapLayer externalMapLayer)
    {
        var externalMapLayerDto = new ExternalMapLayerDto()
        {
            ExternalMapLayerID = externalMapLayer.ExternalMapLayerID,
            GeographyID = externalMapLayer.GeographyID,
            ExternalMapLayerDisplayName = externalMapLayer.ExternalMapLayerDisplayName,
            ExternalMapLayerType = externalMapLayer.ExternalMapLayerType.AsSimpleDto(),
            ExternalMapLayerURL = externalMapLayer.ExternalMapLayerURL,
            LayerIsOnByDefault = externalMapLayer.LayerIsOnByDefault,
            IsActive = externalMapLayer.IsActive,
            ExternalMapLayerDescription = externalMapLayer.ExternalMapLayerDescription,
            PopUpField = externalMapLayer.PopUpField,
            MinZoom = externalMapLayer.MinZoom
        };
        return externalMapLayerDto;
    }
}