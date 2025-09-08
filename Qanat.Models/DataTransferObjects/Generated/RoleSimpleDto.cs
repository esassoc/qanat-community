//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Role]

namespace Qanat.Models.DataTransferObjects
{
    public partial class RoleSimpleDto
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public string RoleDescription { get; set; }
        public int SortOrder { get; set; }
        public string Rights { get; set; }
        public string Flags { get; set; }
    }
}