//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETSyncHistory]

namespace Qanat.Models.DataTransferObjects
{
    public partial class OpenETSyncHistorySimpleDto
    {
        public int OpenETSyncHistoryID { get; set; }
        public int OpenETSyncID { get; set; }
        public int OpenETSyncResultTypeID { get; set; }
        public int OpenETRasterCalculationResultTypeID { get; set; }
        public DateTime? LastCalculationDate { get; set; }
        public DateTime? LastSuccessfulCalculationDate { get; set; }
        public string LastCalculationErrorMessage { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string ErrorMessage { get; set; }
        public string GoogleDriveRasterFileID { get; set; }
        public int? RasterFileResourceID { get; set; }
    }
}