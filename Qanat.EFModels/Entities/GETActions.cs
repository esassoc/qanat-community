using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class GETActions
{
    public static async Task<GETAction> CreateNew(QanatDbContext dbContext, int userID, int modelID, int scenarioID,
        string scenarioRunName = null)
    {
        var getAction = new GETAction()
        {
            ModelID = modelID,
            ScenarioID = scenarioID,
            UserID = userID,
            CreateDate = DateTime.UtcNow,
            LastUpdateDate = DateTime.UtcNow,
            GETActionStatusID = GETActionStatus.Created.GETActionStatusID,
            ActionName = scenarioRunName
        };
        dbContext.GETActions.Add(getAction);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(getAction).ReloadAsync();
        return getAction;
    }

    public static List<GETActionDto> ListByModelID(QanatDbContext dbContext, int modelID)
    {
        return dbContext.GETActions.AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.ModelID == modelID)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => x.AsGETActionDto()).ToList();
    }

    public static List<GETActionDto> ListByModelIDAndGETRunCustomerID(QanatDbContext dbContext, int modelID, int getRunCustomerID)
    {
        var userIDList = dbContext.Users.AsNoTracking().Where(x => x.GETRunCustomerID == getRunCustomerID).Select(x => x.UserID)
            .ToList();
        return dbContext.GETActions.AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.ModelID == modelID && userIDList.Contains(x.UserID))
            .OrderByDescending(x => x.CreateDate)
            .Select(x => x.AsGETActionDto()).ToList();
    }

    public static List<GETActionDto> List(QanatDbContext dbContext)
    {
        return dbContext.GETActions.AsNoTracking()
            .Include(x => x.User)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => x.AsGETActionDto()).ToList();
    }

    public static List<GETActionDto> ListByGETRunCustomerID(QanatDbContext dbContext, int getRunCustomerID)
    {
        var userIDList = dbContext.Users.AsNoTracking().Where(x => x.GETRunCustomerID == getRunCustomerID).Select(x => x.UserID)
            .ToList();
        var getActionDtos = dbContext.GETActions.AsNoTracking()
            .Include(x => x.User)
            .Where(x => userIDList.Contains(x.UserID))
            .OrderByDescending(x => x.CreateDate)
            .Select(x => x.AsGETActionDto()).ToList();
        return getActionDtos;
    }

    public static GETAction GetByID(QanatDbContext dbContext, int getActionID)
    {
        return dbContext.GETActions
            .Include(x => x.User)
            .Include(x => x.GETActionFileResources)
                .ThenInclude(x => x.FileResource)
            .Include(x => x.GETActionOutputFiles)
                .ThenInclude(x => x.FileResource)
            .SingleOrDefault(x => x.GETActionID == getActionID);
    }

    public static void MarkAsTerminalWithIntegrationFailure(
        QanatDbContext dbContext, GETAction getAction)
    {
        getAction.LastUpdateDate = DateTime.UtcNow;
        getAction.GETActionStatusID = GETActionStatus.GETIntegrationFailure.GETActionStatusID;
        dbContext.SaveChanges();
    }

    public static async Task AddFileResource(QanatDbContext dbContext, int getActionID, FileResource fileResource)
    {
        var getAction = dbContext.GETActions
            .Single(x => x.GETActionID == getActionID);

        getAction.GETActionFileResources.Add(new GETActionFileResource()
        {
            FileResourceID = fileResource.FileResourceID
        });
        await dbContext.SaveChangesAsync();
    }
}