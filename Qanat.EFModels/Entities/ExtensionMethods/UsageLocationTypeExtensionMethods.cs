using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities.ExtensionMethods;
public static class UsageLocationTypeExtensionMethods
{
    public static UsageLocationTypeDto AsDto(this UsageLocationType usageLocationType)
    {
        var usageLocationTypeDto = new UsageLocationTypeDto
        {
            UsageLocationTypeID = usageLocationType.UsageLocationTypeID,
            Geography = usageLocationType.Geography?.AsDisplayDto(),
            Name = usageLocationType.Name,
            Definition = usageLocationType.Definition,
            CanBeRemoteSensed = usageLocationType.CanBeRemoteSensed,
            IsIncludedInUsageCalculation = usageLocationType.IsIncludedInUsageCalculation,
            IsDefault = usageLocationType.IsDefault,
            ColorHex = usageLocationType.ColorHex,
            SortOrder = usageLocationType.SortOrder,

            CanBeSelectedInCoverCropForm = usageLocationType.CanBeSelectedInCoverCropForm,
            CountsAsCoverCropped = usageLocationType.CountsAsCoverCropped,
            CanBeSelectedInFallowForm = usageLocationType.CanBeSelectedInFallowForm,
            CountsAsFallowed = usageLocationType.CountsAsFallowed,

            CreateDate = usageLocationType.CreateDate,
            CreateUser = usageLocationType.CreateUser.AsDisplayDto(),
            UpdateDate = usageLocationType.UpdateDate,
            UpdateUser = usageLocationType.UpdateUser?.AsDisplayDto()
        };

        return usageLocationTypeDto;
    }

    public static UsageLocationTypeSimpleDto AsSimpleDto(this UsageLocationType usageLocationType)
    {
        var usageLocationTypeSimpleDto = new UsageLocationTypeSimpleDto
        {
            UsageLocationTypeID = usageLocationType.UsageLocationTypeID,
            Name = usageLocationType.Name,
            Definition = usageLocationType.Definition,
            CanBeRemoteSensed = usageLocationType.CanBeRemoteSensed,
            IsIncludedInUsageCalculation = usageLocationType.IsIncludedInUsageCalculation,
            IsDefault = usageLocationType.IsDefault,
            ColorHex = usageLocationType.ColorHex,
            SortOrder = usageLocationType.SortOrder,

            CanBeSelectedInCoverCropForm = usageLocationType.CanBeSelectedInCoverCropForm,
            CountsAsCoverCropped = usageLocationType.CountsAsCoverCropped,
            CanBeSelectedInFallowForm = usageLocationType.CanBeSelectedInFallowForm,
            CountsAsFallowed = usageLocationType.CountsAsFallowed,
        };

        return usageLocationTypeSimpleDto;
    }
}