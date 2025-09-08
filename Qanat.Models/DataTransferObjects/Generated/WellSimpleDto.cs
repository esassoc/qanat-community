//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Well]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WellSimpleDto
    {
        public int WellID { get; set; }
        public int GeographyID { get; set; }
        public int? ParcelID { get; set; }
        public string WellName { get; set; }
        public string StateWCRNumber { get; set; }
        public string CountyWellPermitNumber { get; set; }
        public DateOnly? DateDrilled { get; set; }
        public int? WellDepth { get; set; }
        public DateTime? CreateDate { get; set; }
        public int WellStatusID { get; set; }
        public string Notes { get; set; }
    }
}