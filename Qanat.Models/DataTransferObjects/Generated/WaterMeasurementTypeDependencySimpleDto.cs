//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementTypeDependency]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WaterMeasurementTypeDependencySimpleDto
    {
        public int WaterMeasurementTypeDependencyID { get; set; }
        public int GeographyID { get; set; }
        public int WaterMeasurementTypeID { get; set; }
        public int DependsOnWaterMeasurementTypeID { get; set; }
    }
}