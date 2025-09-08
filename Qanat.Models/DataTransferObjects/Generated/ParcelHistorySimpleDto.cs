//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelHistory]

namespace Qanat.Models.DataTransferObjects
{
    public partial class ParcelHistorySimpleDto
    {
        public int ParcelHistoryID { get; set; }
        public int GeographyID { get; set; }
        public int ParcelID { get; set; }
        public int EffectiveYear { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserID { get; set; }
        public decimal ParcelArea { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAddress { get; set; }
        public int ParcelStatusID { get; set; }
        public bool IsReviewed { get; set; }
        public bool IsManualOverride { get; set; }
        public DateTime? ReviewDate { get; set; }
        public int? WaterAccountID { get; set; }
    }
}