//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETSync]

namespace Qanat.Models.DataTransferObjects
{
    public partial class OpenETSyncSimpleDto
    {
        public int OpenETSyncID { get; set; }
        public int GeographyID { get; set; }
        public int OpenETDataTypeID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime? FinalizeDate { get; set; }
    }
}