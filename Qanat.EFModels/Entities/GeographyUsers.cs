namespace Qanat.EFModels.Entities;

public static class GeographyUsers
{
    public static void AddGeographyNormalUserIfAbsent(QanatDbContext dbContext, int userID, int geographyID)
    {
        AddGeographyNormalUsersIfAbsent(dbContext, new List<int>(){ userID }, geographyID);
    }

    public static void AddGeographyNormalUsersIfAbsent(QanatDbContext dbContext, List<int> userIDs, int geographyID)
    {
        var geographyUserIDs = dbContext.GeographyUsers
            .Where(x => x.GeographyID == geographyID && userIDs.Contains(x.UserID))
            .Select(x => x.UserID).ToList();

        var newGeographyUsers = userIDs.Where(x => !geographyUserIDs.Contains(x))
            .Select(x => new GeographyUser()
            {
                UserID = x,
                GeographyID = geographyID,
                GeographyRoleID = (int)GeographyRoleEnum.Normal
            });

        dbContext.GeographyUsers.AddRange(newGeographyUsers);
        dbContext.SaveChanges();
    }
}