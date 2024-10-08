//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Flag]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class FlagExtensionMethods
    {
        public static FlagSimpleDto AsSimpleDto(this Flag flag)
        {
            var dto = new FlagSimpleDto()
            {
                FlagID = flag.FlagID,
                FlagName = flag.FlagName,
                FlagDisplayName = flag.FlagDisplayName
            };
            return dto;
        }
    }
}