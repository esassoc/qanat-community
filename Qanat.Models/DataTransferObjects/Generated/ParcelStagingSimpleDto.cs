//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelStaging]

namespace Qanat.Models.DataTransferObjects
{
    public partial class ParcelStagingSimpleDto
    {
        public int ParcelStagingID { get; set; }
        public int GeographyID { get; set; }
        public string ParcelNumber { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAddress { get; set; }
        public bool HasConflict { get; set; }
    }
}