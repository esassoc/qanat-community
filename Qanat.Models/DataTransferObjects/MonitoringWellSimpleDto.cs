namespace Qanat.Models.DataTransferObjects
{
    public class MonitoringWellSimpleDto
    {
        public int MonitoringWellID { get; set; }
        public int GeographyID { get; set; }
        public string SiteCode { get; set; }
        public string MonitoringWellName { get; set; }
        public int MonitoringWellSourceTypeID { get; set; }
    }
}