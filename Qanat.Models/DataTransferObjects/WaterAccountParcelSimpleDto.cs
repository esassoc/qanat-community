namespace Qanat.Models.DataTransferObjects
{
    public class WaterAccountParcelSimpleDto
    {
        public int WaterAccountParcelID { get; set; }
        public int GeographyID { get; set; }
        public int WaterAccountID { get; set; }
        public int ParcelID { get; set; }
        public int ReportingPeriodID { get; set; }
    }
}