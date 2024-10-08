//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementType]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WaterMeasurementTypeSimpleDto
    {
        public int WaterMeasurementTypeID { get; set; }
        public int GeographyID { get; set; }
        public int WaterMeasurementCategoryTypeID { get; set; }
        public bool IsActive { get; set; }
        public string WaterMeasurementTypeName { get; set; }
        public int SortOrder { get; set; }
        public bool IsUserEditable { get; set; }
        public bool ShowToLandowner { get; set; }
        public int? WaterMeasurementCalculationTypeID { get; set; }
        public string CalculationJSON { get; set; }
    }
}