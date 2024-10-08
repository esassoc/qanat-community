//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UploadedWellGdb]

namespace Qanat.Models.DataTransferObjects
{
    public partial class UploadedWellGdbSimpleDto
    {
        public int UploadedWellGdbID { get; set; }
        public int UserID { get; set; }
        public int GeographyID { get; set; }
        public string CanonicalName { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool Finalized { get; set; }
        public int SRID { get; set; }
    }
}