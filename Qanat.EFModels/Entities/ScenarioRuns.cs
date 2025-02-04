using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ScenarioRuns
{
    public static async Task<ScenarioRun> CreateNew(QanatDbContext dbContext, int userID, int modelID, int scenarioID, string scenarioRunName = null)
    {
        var getAction = new ScenarioRun()
        {
            ModelID = modelID,
            ScenarioID = scenarioID,
            UserID = userID,
            CreateDate = DateTime.UtcNow,
            LastUpdateDate = DateTime.UtcNow,
            ScenarioRunStatusID = ScenarioRunStatus.Created.ScenarioRunStatusID,
            ActionName = scenarioRunName
        };

        dbContext.ScenarioRuns.Add(getAction);

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(getAction).ReloadAsync();

        return getAction;
    }

    public static List<ScenarioRunDto> ListByModelID(QanatDbContext dbContext, int modelID)
    {
        var scenarioRuns = dbContext.ScenarioRuns.AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.ModelID == modelID)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => x.AsDto()).ToList();

        return scenarioRuns;
    }

    public static List<ScenarioRunDto> ListByModelIDAndGETRunCustomerID(QanatDbContext dbContext, int modelID, int getRunCustomerID)
    {
        var userIDList = dbContext.Users.AsNoTracking()
            .Where(x => x.GETRunCustomerID == getRunCustomerID)
            .Select(x => x.UserID)
            .ToList();


        var scenarioRuns = dbContext.ScenarioRuns.AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.ModelID == modelID && userIDList.Contains(x.UserID))
            .OrderByDescending(x => x.CreateDate)
            .Select(x => x.AsDto()).ToList();

        return scenarioRuns;
    }

    public static List<ScenarioRunDto> List(QanatDbContext dbContext)
    {
        var scenarioRuns = dbContext.ScenarioRuns.AsNoTracking()
            .Include(x => x.User)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => x.AsDto()).ToList();

        return scenarioRuns;
    }

    public static List<ScenarioRunDto> ListByGETRunCustomerID(QanatDbContext dbContext, int getRunCustomerID)
    {
        var userIDList = dbContext.Users.AsNoTracking()
            .Where(x => x.GETRunCustomerID == getRunCustomerID)
            .Select(x => x.UserID)
            .ToList();

        var scenarioRuns = dbContext.ScenarioRuns.AsNoTracking()
            .Include(x => x.User)
            .Where(x => userIDList.Contains(x.UserID))
            .OrderByDescending(x => x.CreateDate)
            .Select(x => x.AsDto()).ToList();

        return scenarioRuns;
    }

    public static ScenarioRun GetByID(QanatDbContext dbContext, int scenarioRunID)
    {
        var scenarioRun = dbContext.ScenarioRuns
            .Include(x => x.User)
            .Include(x => x.ScenarioRunFileResources).ThenInclude(x => x.FileResource)
            .Include(x => x.ScenarioRunOutputFiles).ThenInclude(x => x.FileResource)
            .SingleOrDefault(x => x.ScenarioRunID == scenarioRunID);

        return scenarioRun;
    }

    public static void MarkAsTerminalWithIntegrationFailure(QanatDbContext dbContext, ScenarioRun scenarioRun)
    {
        scenarioRun.LastUpdateDate = DateTime.UtcNow;
        scenarioRun.ScenarioRunStatusID = ScenarioRunStatus.GETIntegrationFailure.ScenarioRunStatusID;
        dbContext.SaveChanges();
    }

    public static async Task AddFileResource(QanatDbContext dbContext, int scenarioRunID, FileResource fileResource)
    {
        var getAction = dbContext.ScenarioRuns.Single(x => x.ScenarioRunID == scenarioRunID);

        getAction.ScenarioRunFileResources.Add(new ScenarioRunFileResource()
        {
            FileResourceID = fileResource.FileResourceID
        });

        await dbContext.SaveChangesAsync();
    }
}