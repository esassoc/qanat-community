namespace Qanat.Models.DataTransferObjects
{
    public class UsageLocationSimpleDto
    {
        public int UsageLocationID { get; set; }
        public int GeographyID { get; set; }
        public int ParcelID { get; set; }
        public int ReportingPeriodID { get; set; }
        public string Name { get; set; }
        public double Area { get; set; }
        public int FallowTypeID { get; set; }
        public DateOnly? FallowStartDate { get; set; }
        public DateOnly? FallowEndDate { get; set; }
        public DateOnly? CoverCropStartDate { get; set; }
        public DateOnly? CoverCropEndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserID { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUserID { get; set; }
    }
}