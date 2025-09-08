namespace Qanat.Models.DataTransferObjects
{
    public class GeographyRoleSimpleDto
    {
        public int GeographyRoleID { get; set; }
        public string GeographyRoleName { get; set; }
        public string GeographyRoleDisplayName { get; set; }
        public string GeographyRoleDescription { get; set; }
        public int SortOrder { get; set; }
        public string Rights { get; set; }
        public string Flags { get; set; }
    }
}