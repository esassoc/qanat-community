//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountParcel]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WaterAccountParcelSimpleDto
    {
        public int WaterAccountParcelID { get; set; }
        public int GeographyID { get; set; }
        public int WaterAccountID { get; set; }
        public int ParcelID { get; set; }
        public int EffectiveYear { get; set; }
        public int? EndYear { get; set; }
    }
}