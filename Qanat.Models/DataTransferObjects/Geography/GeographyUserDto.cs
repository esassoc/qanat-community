namespace Qanat.Models.DataTransferObjects.Geography;

public class GeographyUserDto
{
    public int GeographyUserID { get; set; }
    public GeographySimpleDto Geography { get; set; }
    public UserDto User { get; set; }
    public GeographyRoleSimpleDto GeographyRole { get; set; }
}