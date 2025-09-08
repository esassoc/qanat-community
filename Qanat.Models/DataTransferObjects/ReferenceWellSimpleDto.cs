namespace Qanat.Models.DataTransferObjects
{
    public class ReferenceWellSimpleDto
    {
        public int ReferenceWellID { get; set; }
        public int GeographyID { get; set; }
        public string WellName { get; set; }
        public string CountyWellPermitNo { get; set; }
        public int? WellDepth { get; set; }
        public string StateWCRNumber { get; set; }
        public DateOnly? DateDrilled { get; set; }
    }
}