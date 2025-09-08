namespace Qanat.Models.DataTransferObjects;

public class OpenETConfigurationDto
{
    public int GeographyID { get; set; }
    public bool IsOpenETActive { get; set; }
    public string OpenETShapefilePath { get; set; }
}