using ETL.Domain.Common;

namespace ETL.Domain.Entities;
public class User : BaseEntity
{

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }

    private User() { }


    public User(string firstName, string lastName, string username, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Username = username;
        Email = email;
    }

}
