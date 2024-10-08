//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccount]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WaterAccountSimpleDto
    {
        public int WaterAccountID { get; set; }
        public int GeographyID { get; set; }
        public int WaterAccountNumber { get; set; }
        public string WaterAccountName { get; set; }
        public string Notes { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string WaterAccountPIN { get; set; }
        public DateTime CreateDate { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
    }
}