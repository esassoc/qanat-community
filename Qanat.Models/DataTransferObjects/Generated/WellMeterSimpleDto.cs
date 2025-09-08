//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WellMeter]

namespace Qanat.Models.DataTransferObjects
{
    public partial class WellMeterSimpleDto
    {
        public int WellMeterID { get; set; }
        public int WellID { get; set; }
        public int MeterID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}