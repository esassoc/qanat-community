using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects
{
    public class CustomPageWithRolesDto
    {
        [Required]
        public int CustomPageID { get; set; }
        [Required]
        public string CustomPageDisplayName { get; set; }
        [Required]
        public string CustomPageVanityUrl { get; set; }
        public string CustomPageContent { get; set; }
        [Required]
        public MenuItemSimpleDto MenuItem { get; set; }
        public int? SortOrder { get; set; }
        [Required]
        public List<RoleSimpleDto> ViewableRoles { get; set; }
        public bool IsEmptyContent => string.IsNullOrEmpty(CustomPageContent);
    }
}