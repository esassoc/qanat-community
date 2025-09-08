//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETSyncHistory]
namespace Qanat.EFModels.Entities
{
    public partial class OpenETSyncHistory
    {
        public int PrimaryKey => OpenETSyncHistoryID;
        public OpenETSyncResultType OpenETSyncResultType => OpenETSyncResultType.AllLookupDictionary[OpenETSyncResultTypeID];
        public OpenETRasterCalculationResultType OpenETRasterCalculationResultType => OpenETRasterCalculationResultType.AllLookupDictionary[OpenETRasterCalculationResultTypeID];

        public static class FieldLengths
        {
            public const int GoogleDriveRasterFileID = 33;
        }
    }
}