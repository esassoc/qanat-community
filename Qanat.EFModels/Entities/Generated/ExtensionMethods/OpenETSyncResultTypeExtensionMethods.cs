//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETSyncResultType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class OpenETSyncResultTypeExtensionMethods
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