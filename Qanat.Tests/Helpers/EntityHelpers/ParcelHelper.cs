using System;
using System.Threading.Tasks;
using Qanat.EFModels.Entities;

namespace Qanat.Tests.Helpers.EntityHelpers;

public static class ParcelHelper
{
    public static async Task<Parcel> AddParcelAsync(int geographyID, double area)
    {
        var parcelNumber = Guid.NewGuid().ToString().Substring(0, 20);
        var addParcelResult = await AssemblySteps.QanatDbContext.Parcels.AddAsync(new Parcel()
        {
            GeographyID = geographyID,
            ParcelNumber = parcelNumber,
            ParcelArea = area,
            ParcelStatusID = ParcelStatus.Unassigned.ParcelStatusID,
            OwnerAddress = Guid.NewGuid().ToString(),
            OwnerName = Guid.NewGuid().ToString(),
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var parcel = addParcelResult.Entity;
        await AssemblySteps.QanatDbContext.Entry(parcel).ReloadAsync();
        return parcel;
    }
}