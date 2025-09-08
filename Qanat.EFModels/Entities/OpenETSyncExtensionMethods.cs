using System.Linq;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class OpenETSyncExtensionMethods
{
    public static OpenETSyncDto AsOpenETSyncDto(this OpenETSync openETSync)
    {
        var lastSync = openETSync.OpenETSyncHistories.MaxBy(x => x.CreateDate);
        var openETSyncDto = new OpenETSyncDto()
        {
            OpenETSyncID = openETSync.OpenETSyncID,
            Geography = openETSync.Geography.AsDto(),
            OpenETDataType = openETSync.OpenETDataType.AsSimpleDto(),
            Year = openETSync.Year,
            Month = openETSync.Month,

            LastSyncDate = lastSync?.UpdateDate,
            LastSyncStatus = lastSync?.OpenETSyncResultType?.AsSimpleDto(),
            LastSuccessfulSyncDate = openETSync.OpenETSyncHistories
                .Where(x => x.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.Succeeded).MaxBy(x => x.CreateDate)
                ?.UpdateDate,
            LastSyncMessage = lastSync?.ErrorMessage,
            HasInProgressSync = openETSync.OpenETSyncHistories.Any(x => x.OpenETSyncResultTypeID == (int)OpenETSyncResultTypeEnum.InProgress),
            
            LastRasterCalculationDate = lastSync?.LastCalculationDate,
            LastRasterCalculationStatus = lastSync?.OpenETRasterCalculationResultType?.AsSimpleDto(),
            LastSuccessfulCalculationDate = lastSync?.LastSuccessfulCalculationDate,
            LastRasterCalculationMessage = lastSync?.LastCalculationErrorMessage,
            FileResourceGUID = lastSync?.RasterFileResource?.FileResourceCanonicalName,
            FileResourceOriginalName = lastSync?.RasterFileResource?.OriginalBaseFilename,
            FileResourceOriginalFileExtension = lastSync?.RasterFileResource?.OriginalFileExtension,

            FinalizeDate = openETSync.FinalizeDate,
        };

        return openETSyncDto;
    }
}