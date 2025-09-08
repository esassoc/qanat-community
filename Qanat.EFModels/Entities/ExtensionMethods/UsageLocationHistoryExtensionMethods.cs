using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities.ExtensionMethods;

public static class UsageLocationHistoryExtensionMethods
{
    public static UsageLocationHistoryDto AsDto(this UsageLocationHistory usageLocationHistory)
    {
        var dto = new UsageLocationHistoryDto
        {
            UsageLocationHistoryID = usageLocationHistory.UsageLocationHistoryID,
            UsageLocationID = usageLocationHistory.UsageLocationID,
            UsageLocationName = usageLocationHistory.UsageLocation.Name,
            UsageLocationTypeName = usageLocationHistory.UsageLocationType.Name,
            ReportingPeriodName = usageLocationHistory.UsageLocation.ReportingPeriod.Name,
            Note = usageLocationHistory.Note,
            CreateDate = usageLocationHistory.CreateDate,
            CreateUserFullName = usageLocationHistory.CreateUser.FullName,
        };

        return dto;
    }
}
