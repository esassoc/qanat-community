namespace Qanat.Models.DataTransferObjects
{
    public class WellSimpleDto
    {
        public int WellID { get; set; }
        public int GeographyID { get; set; }
        public int? ParcelID { get; set; }
        public bool ParcelIsManualOverride { get; set; }
        public string WellName { get; set; }
        public string StateWCRNumber { get; set; }
        public string CountyWellPermitNumber { get; set; }
        public DateOnly? DateDrilled { get; set; }
        public DateTime? CreateDate { get; set; }
        public int WellStatusID { get; set; }
        public string Notes { get; set; }
        public int? WellDepth { get; set; }
        public int? CasingDiameter { get; set; }
        public int? TopOfPerforations { get; set; }
        public int? BottomOfPerforations { get; set; }
        public string ElectricMeterNumber { get; set; }
    }
}