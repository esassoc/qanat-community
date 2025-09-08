//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MonitoringWellMeasurement]

namespace Qanat.Models.DataTransferObjects
{
    public partial class MonitoringWellMeasurementSimpleDto
    {
        public int MonitoringWellMeasurementID { get; set; }
        public int MonitoringWellID { get; set; }
        public int GeographyID { get; set; }
        public int ExtenalUniqueID { get; set; }
        public decimal Measurement { get; set; }
        public DateTime MeasurementDate { get; set; }
    }
}