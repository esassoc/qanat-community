using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ModelUsers
{ 
    public static async Task<List<ModelSimpleDto>> ListModelsByUserID(QanatDbContext dbContext, UserDto user)
    {
        var userIsAdmin = (user.Flags?.FirstOrDefault(x => x.Key == Flag.IsSystemAdmin.FlagName))?.Value ?? false;
        if (userIsAdmin)
        {
            return Model.All.Select(x => x.AsSimpleDto()).ToList();
        }

        var modelsIDsForUser = await dbContext.ModelUsers.AsNoTracking()
            .Where(x => x.UserID == user.UserID)
            .Select(x => x.ModelID)
            .ToListAsync();

        var modelsForUser = Model.All
            .Where(x => modelsIDsForUser.Contains(x.ModelID))
            .Select(x => x.AsSimpleDto())
            .ToList();

        return modelsForUser;
    }

    public static async Task<bool> CheckIfUserHasModelAccessAsync(QanatDbContext dbContext, int modelID, UserDto user)
    {
        var userIsAdmin = (user.Flags?.FirstOrDefault(x => x.Key == Flag.IsSystemAdmin.FlagName))?.Value ?? false;
        if (userIsAdmin)
        {
            return true;
        }

        var modelUser = await dbContext.ModelUsers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ModelID == modelID && x.UserID == user.UserID);

        return modelUser != null;
    }
}
