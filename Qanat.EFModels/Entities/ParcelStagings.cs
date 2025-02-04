namespace Qanat.EFModels.Entities;

public static class ParcelStagings
{
    public const string OwnerNameToUseIfNull = "<owner name not provided>";
    public const string OwnerAddressToUseIfNull = "<owner address not provided>";

    public static ParcelUpdateExpectedResultsDto GetExpectedResultsDto(QanatDbContext dbContext, int geographyID)
    {
        var fParcelStagingChanges = dbContext.fParcelStagingChanges(geographyID);

        var expectedChanges = new ParcelUpdateExpectedResultsDto()
        {
            NumParcelsInGdb = fParcelStagingChanges.Count(),
            NumParcelsUnchanged = fParcelStagingChanges.Count(x => !x.HasGeometryChange && !x.HasOwnerNameChange && !x.HasOwnerAddressChange && !x.HasAcresChange),
            NumParcelsToBeInactivated = fParcelStagingChanges.Count(x => x.ParcelStatusID == (int)ParcelStatusEnum.Inactive),
            NumParcelsWithUpdatedGeometries = fParcelStagingChanges.Count(x => x.HasGeometryChange),
            NumParcelsAdded = fParcelStagingChanges.Count(x => x.IsNew),
            NumParcelsWithOwnerOrAddressChange = fParcelStagingChanges.Count(x => x.HasOwnerNameChange || x.HasOwnerAddressChange),
            NumParcelsWithAcresChange = fParcelStagingChanges.Count(x => x.HasAcresChange)
        };
        expectedChanges.NumParcelsToBeUpdated = expectedChanges.NumParcelsInGdb - expectedChanges.NumParcelsUnchanged;

        return expectedChanges;
    }
}

public class ParcelUpdateExpectedResultsDto
{
    public int NumParcelsInGdb { get; set; }
    public int NumParcelsAdded { get; set; }
    public int NumParcelsUnchanged { get; set; }
    public int NumParcelsToBeInactivated { get; set; }
    public int NumParcelsToBeUpdated { get; set; }
    public int NumParcelsWithUpdatedGeometries { get; set; }
    public int NumParcelsWithOwnerOrAddressChange { get; set; }
    public int NumParcelsWithAcresChange { get; set; }
}