namespace Qanat.Models.DataTransferObjects.Geography;

public class GeographyUserDto
{
    public int GeographyUserID { get; set; }
    public GeographySimpleDto Geography { get; set; }
    public UserDto User { get; set; }
    public GeographyRoleSimpleDto GeographyRole { get; set; }
    public int WaterAccountCount => WaterAccounts?.Count ?? 0;
    public List<WaterAccountSimpleDto> WaterAccounts { get; set; }
    public int WellRegistrationCount { get; set; }
}