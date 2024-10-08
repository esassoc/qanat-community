using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects
{
    public class CustomPageUpsertDto
    {
        [Required]
        [RegularExpression(@"^[_a-zA-Z0-9\-\s]{1,100}$", ErrorMessage = "Only letters, numbers, spaces, and hyphens allowed. 100 character maximum")]
        public string CustomPageDisplayName { get; set; }
        [Required]
        [RegularExpression(@"^[_a-zA-Z0-9\-]{1,100}$", ErrorMessage = "Only letters, numbers, and hyphens allowed. 100 character maximum")]
        public string CustomPageVanityUrl { get; set; }
        public string CustomPageContent { get; set; }
        [Required]
        public int MenuItemID { get; set; }
        public int? SortOrder { get; set; }
        [Required]
        public List<int> ViewableRoleIDs { get; set; }
    }
}