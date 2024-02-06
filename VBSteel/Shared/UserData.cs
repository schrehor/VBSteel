using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;
public class User
{
    [Key]
    public Guid UserId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    [Required, StringLength(100)]
    public string Surname { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public UserRole Role { get; set; }

    public List<Favorite> Favorites { get; set; }
}

public enum UserRole
{
    RegularUser,
    Admin,
    NotLoggedIn
}