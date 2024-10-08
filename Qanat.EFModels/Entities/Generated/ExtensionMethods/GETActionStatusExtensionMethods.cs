//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GETActionStatus]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GETActionStatusExtensionMethods
    {
        public static GETActionStatusSimpleDto AsSimpleDto(this GETActionStatus gETActionStatus)
        {
            var dto = new GETActionStatusSimpleDto()
            {
                GETActionStatusID = gETActionStatus.GETActionStatusID,
                GETActionStatusName = gETActionStatus.GETActionStatusName,
                GETActionStatusDisplayName = gETActionStatus.GETActionStatusDisplayName,
                GETRunStatusID = gETActionStatus.GETRunStatusID,
                IsTerminal = gETActionStatus.IsTerminal
            };
            return dto;
        }
    }
}