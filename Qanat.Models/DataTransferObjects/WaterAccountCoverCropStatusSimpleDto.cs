namespace Qanat.Models.DataTransferObjects
{
    public class WaterAccountCoverCropStatusSimpleDto
    {
        public int WaterAccountCoverCropStatusID { get; set; }
        public int GeographyID { get; set; }
        public int WaterAccountID { get; set; }
        public int ReportingPeriodID { get; set; }
        public int SelfReportStatusID { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public int? SubmittedByUserID { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? ApprovedByUserID { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public int? ReturnedByUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserID { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUserID { get; set; }
    }
}