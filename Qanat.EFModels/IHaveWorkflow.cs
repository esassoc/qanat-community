using Qanat.EFModels.Entities;
using Qanat.EFModels.Workflows;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels;

public interface IHaveWorkflow<out T> where T : IWorkflow
{
    public T GetWorkflow(QanatDbContext dbContext, UserDto currentUser);
}