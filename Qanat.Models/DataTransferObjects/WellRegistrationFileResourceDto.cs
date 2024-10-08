namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationFileResourceDto
{
    public int WellRegistrationFileResourceID { get; set; }
    public int WellRegistrationID { get; set; }
    public int FileResourceID { get; set; }
    public string FileDescription { get; set; }
    public FileResourceSimpleDto FileResource { get; set; }
}