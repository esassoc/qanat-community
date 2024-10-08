//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ReportingPeriod]

namespace Qanat.Models.DataTransferObjects
{
    public partial class ReportingPeriodSimpleDto
    {
        public int ReportingPeriodID { get; set; }
        public int GeographyID { get; set; }
        public string ReportingPeriodName { get; set; }
        public int StartMonth { get; set; }
        public string Interval { get; set; }
    }
}