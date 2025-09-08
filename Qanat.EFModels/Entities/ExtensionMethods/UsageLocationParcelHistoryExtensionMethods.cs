using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities.ExtensionMethods;
public static class UsageLocationParcelHistoryExtensionMethods
{
    public static UsageLocationParcelHistoryDto AsDto(this UsageLocationParcelHistory usageLocationParcelHistory)
    {
        var dto = new UsageLocationParcelHistoryDto
        {
            UsageLocationParcelHistoryID = usageLocationParcelHistory.UsageLocationParcelHistoryID,
            GeographyID = usageLocationParcelHistory.GeographyID,
            UsageLocationID = usageLocationParcelHistory.UsageLocationID,
            UsageLocationName = usageLocationParcelHistory.UsageLocation.Name,
            ReportingPeriodID = usageLocationParcelHistory.ReportingPeriodID,
            ReportingPeriodName = usageLocationParcelHistory.ReportingPeriod.Name,
            FromParcelID = usageLocationParcelHistory.FromParcelID,
            FromParcelNumber = usageLocationParcelHistory.FromParcelNumber,
            ToParcelID = usageLocationParcelHistory.ToParcelID,
            ToParcelNumber = usageLocationParcelHistory.ToParcelNumber,
            CreateDate = usageLocationParcelHistory.CreateDate,
            CreateUserFullName = usageLocationParcelHistory.CreateUser?.FullName
        };

        return dto;
    }
}