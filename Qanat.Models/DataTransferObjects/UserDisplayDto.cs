namespace Qanat.Models.DataTransferObjects;

public class UserDisplayDto
{
    public int UserID { get; set; }
    public string FullName { get; set; }

    public UserDisplayDto(int userID, string firstName, string lastName)
    {
        UserID = userID;
        FullName = $"{firstName} {lastName}";
    }
}