namespace Qanat.Models.DataTransferObjects;

public class WellFileResourceDto
{
    public int WellFileResourceID { get; set; }
    public int WellID { get; set; }
    public int FileResourceID { get; set; }
    public string FileDescription { get; set; }
    public FileResourceSimpleDto FileResource { get; set; }
}