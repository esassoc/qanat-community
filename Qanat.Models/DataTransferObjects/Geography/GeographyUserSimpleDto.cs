namespace Qanat.Models.DataTransferObjects
{
    public class GeographyUserSimpleDto
    {
        public int GeographyUserID { get; set; }
        public int GeographyID { get; set; }
        public int UserID { get; set; }
        public int GeographyRoleID { get; set; }
        public bool ReceivesNotifications { get; set; }
    }
}