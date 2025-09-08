//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETSyncHistory]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class OpenETSyncHistoryExtensionMethods
    {
        public static OpenETSyncHistorySimpleDto AsSimpleDto(this OpenETSyncHistory openETSyncHistory)
        {
            var dto = new OpenETSyncHistorySimpleDto()
            {
                OpenETSyncHistoryID = openETSyncHistory.OpenETSyncHistoryID,
                OpenETSyncID = openETSyncHistory.OpenETSyncID,
                OpenETSyncResultTypeID = openETSyncHistory.OpenETSyncResultTypeID,
                OpenETRasterCalculationResultTypeID = openETSyncHistory.OpenETRasterCalculationResultTypeID,
                LastCalculationDate = openETSyncHistory.LastCalculationDate,
                LastSuccessfulCalculationDate = openETSyncHistory.LastSuccessfulCalculationDate,
                LastCalculationErrorMessage = openETSyncHistory.LastCalculationErrorMessage,
                CreateDate = openETSyncHistory.CreateDate,
                UpdateDate = openETSyncHistory.UpdateDate,
                ErrorMessage = openETSyncHistory.ErrorMessage,
                GoogleDriveRasterFileID = openETSyncHistory.GoogleDriveRasterFileID,
                RasterFileResourceID = openETSyncHistory.RasterFileResourceID
            };
            return dto;
        }
    }
}