using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects.User
{
    public class UserUpsertDto
    {
        [Required]
        public int? RoleID { get; set; }
        [Required]
        public bool ReceiveSupportEmails { get; set; }

        public int? GETRunCustomerID { get; set; }
        public int? GETRunUserID { get; set; }
    }
}