namespace Qanat.Models.DataTransferObjects;

public class ExternalMapLayerDto
{
    public int ExternalMapLayerID { get; set; }
    public int GeographyID { get; set; }
    public string ExternalMapLayerDisplayName { get; set; }
    public ExternalMapLayerTypeSimpleDto ExternalMapLayerType { get; set; }
    public string ExternalMapLayerURL { get; set; }
    public bool LayerIsOnByDefault { get; set; }
    public bool IsActive { get; set; }
    public string ExternalMapLayerDescription { get; set; }
    public string PopUpField { get; set; }
    public int? MinZoom { get; set; }
}