//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[IrrigationMethod]

namespace Qanat.Models.DataTransferObjects
{
    public partial class IrrigationMethodSimpleDto
    {
        public int IrrigationMethodID { get; set; }
        public int GeographyID { get; set; }
        public string Name { get; set; }
        public string SystemType { get; set; }
        public int EfficiencyAsPercentage { get; set; }
        public int DisplayOrder { get; set; }
    }
}