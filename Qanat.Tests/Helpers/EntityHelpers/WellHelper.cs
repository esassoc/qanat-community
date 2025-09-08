using Qanat.EFModels.Entities;
using System.Threading.Tasks;

namespace Qanat.Tests.Helpers.EntityHelpers;
public static class WellHelper
{
    public static async Task<Well> AddWellAsync(int geographyID)
    {
        var addWellEntry = await AssemblySteps.QanatDbContext.Wells.AddAsync(new Well()
        {
            GeographyID = geographyID,
            WellName = "Test Well"
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var well = addWellEntry.Entity;
        await AssemblySteps.QanatDbContext.Entry(well).ReloadAsync();
        return well;
    }

    public static async Task<WellIrrigatedParcel> AddWellIrrigatedParcel(int wellID, int parcelID)
    {
        var addWellIrrigatedParcelEntry = await AssemblySteps.QanatDbContext.WellIrrigatedParcels.AddAsync(new WellIrrigatedParcel()
        {
            WellID = wellID,
            ParcelID = parcelID
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var wellIrrigatedParcel = addWellIrrigatedParcelEntry.Entity;
        await AssemblySteps.QanatDbContext.Entry(wellIrrigatedParcel).ReloadAsync();
        return wellIrrigatedParcel;
    }
}
