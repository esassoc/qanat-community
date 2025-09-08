//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ReferenceWell]

namespace Qanat.Models.DataTransferObjects
{
    public partial class ReferenceWellSimpleDto
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