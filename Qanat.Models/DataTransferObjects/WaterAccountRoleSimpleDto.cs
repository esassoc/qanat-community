namespace Qanat.Models.DataTransferObjects
{
    public class WaterAccountRoleSimpleDto
    {
        public int WaterAccountRoleID { get; set; }
        public string WaterAccountRoleName { get; set; }
        public string WaterAccountRoleDisplayName { get; set; }
        public string WaterAccountRoleDescription { get; set; }
        public int SortOrder { get; set; }
        public string Rights { get; set; }
        public string Flags { get; set; }
    }
}