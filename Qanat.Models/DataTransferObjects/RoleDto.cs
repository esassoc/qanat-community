namespace Qanat.Models.DataTransferObjects;

public partial class RoleDto
{
    public int RoleID { get; set; }
    public string RoleName { get; set; }
    public string RoleDisplayName { get; set; }
    public string RoleDescription { get; set; }
    public int SortOrder { get; set; }
    public string Rights { get; set; }
    public string Flags { get; set; }
}