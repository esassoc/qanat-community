using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class OpenETSyncResultTypeExtensionMethods
    {
        public static OpenETSyncResultTypeSimpleDto AsSimpleDto(this OpenETSyncResultType openETSyncResultType)
        {
            var dto = new OpenETSyncResultTypeSimpleDto()
            {
                OpenETSyncResultTypeID = openETSyncResultType.OpenETSyncResultTypeID,
                OpenETSyncResultTypeName = openETSyncResultType.OpenETSyncResultTypeName,
                OpenETSyncResultTypeDisplayName = openETSyncResultType.OpenETSyncResultTypeDisplayName
            };
            return dto;
        }
    }
}