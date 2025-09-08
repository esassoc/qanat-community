//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UploadedGdb]

namespace Qanat.Models.DataTransferObjects
{
    public partial class UploadedGdbSimpleDto
    {
        public int UploadedGdbID { get; set; }
        public int UserID { get; set; }
        public int GeographyID { get; set; }
        public string CanonicalName { get; set; }
        public DateTime UploadDate { get; set; }
        public int? EffectiveYear { get; set; }
        public bool Finalized { get; set; }
        public int SRID { get; set; }
    }
}