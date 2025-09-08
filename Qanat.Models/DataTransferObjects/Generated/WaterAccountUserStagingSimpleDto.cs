//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountUserStaging]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WaterAccountUserStagingSimpleDto
    {
        public int WaterAccountUserStagingID { get; set; }
        public int UserID { get; set; }
        public int WaterAccountID { get; set; }
        public DateTime ClaimDate { get; set; }
    }
}