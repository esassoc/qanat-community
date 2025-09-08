//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelSupply]

namespace Qanat.Models.DataTransferObjects
{
    public partial class ParcelSupplySimpleDto
    {
        public int ParcelSupplyID { get; set; }
        public int GeographyID { get; set; }
        public int ParcelID { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal TransactionAmount { get; set; }
        public int? WaterTypeID { get; set; }
        public int? UserID { get; set; }
        public string UserComment { get; set; }
        public string UploadedFileName { get; set; }
    }
}