//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GETActionStatus]

namespace Qanat.Models.DataTransferObjects
{
    public partial class GETActionStatusSimpleDto
    {
        public int GETActionStatusID { get; set; }
        public string GETActionStatusName { get; set; }
        public string GETActionStatusDisplayName { get; set; }
        public int? GETRunStatusID { get; set; }
        public bool IsTerminal { get; set; }
    }
}