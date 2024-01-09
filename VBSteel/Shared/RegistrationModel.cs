using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;

public class RegistrationViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Surname { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Heslá sa nezhodujú.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}
