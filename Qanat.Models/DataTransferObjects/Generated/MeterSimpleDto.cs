//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Meter]

namespace Qanat.Models.DataTransferObjects
{
    public partial class MeterSimpleDto
    {
        public int MeterID { get; set; }
        public string SerialNumber { get; set; }
        public string DeviceName { get; set; }
        public string Make { get; set; }
        public string ModelNumber { get; set; }
        public int GeographyID { get; set; }
        public int MeterStatusID { get; set; }
    }
}