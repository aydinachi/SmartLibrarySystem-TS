
namespace SmartLibrarySystem.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }

    public User(int id, string fullName, string email, UserRole role)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        Role = role;
    }
}
