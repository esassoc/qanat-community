using Qanat.EFModels.Workflows;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public partial class WellRegistration
{
    public double? Longitude => LocationPoint4326?.Coordinate.X;
    public double? Latitude => LocationPoint4326?.Coordinate.Y;

    public WellRegistryWorkflow GetWorkflow(QanatDbContext dbContext, UserDto currentUser)
    {
        return new WellRegistryWorkflow(dbContext, this, currentUser);
    }
}