namespace Qanat.Models.DataTransferObjects
{
    public class IrrigationMethodSimpleDto
    {
        public int IrrigationMethodID { get; set; }
        public int GeographyID { get; set; }
        public string Name { get; set; }
        public string SystemType { get; set; }
        public int EfficiencyAsPercentage { get; set; }
        public int DisplayOrder { get; set; }
    }
}