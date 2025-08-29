//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterType]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WaterTypeSimpleDto
    {
        public int WaterTypeID { get; set; }
        public int GeographyID { get; set; }
        public bool IsActive { get; set; }
        public string WaterTypeName { get; set; }
        public bool IsAppliedProportionally { get; set; }
        public string WaterTypeDefinition { get; set; }
        public bool IsSourcedFromApi { get; set; }
        public int SortOrder { get; set; }
        public string WaterTypeSlug { get; set; }
        public string WaterTypeColor { get; set; }
    }
}