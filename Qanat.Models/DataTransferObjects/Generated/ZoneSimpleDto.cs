//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Zone]

namespace Qanat.Models.DataTransferObjects
{
    public partial class ZoneSimpleDto
    {
        public int ZoneID { get; set; }
        public int ZoneGroupID { get; set; }
        public string ZoneName { get; set; }
        public string ZoneSlug { get; set; }
        public string ZoneDescription { get; set; }
        public string ZoneColor { get; set; }
        public string ZoneAccentColor { get; set; }
        public decimal? PrecipMultiplier { get; set; }
        public int SortOrder { get; set; }
    }
}