namespace Qanat.Models.DataTransferObjects;

public class UserDisplayDto
{
    public int UserID { get; set; }
    public string FullName { get; set; }

    public UserDisplayDto() { } //MK 6/18/2025: Needed for deserialization.

    public UserDisplayDto(int userID, string firstName, string lastName)
    {
        UserID = userID;
        FullName = $"{firstName} {lastName}";
    }
}