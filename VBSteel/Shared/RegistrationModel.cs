using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;

public class RegistrationModel
{
    [Required(ErrorMessage = "Emailová adresa je povinná")]
    [EmailAddress(ErrorMessage = "Neplatná emailová adresa")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Meno je povinný údaj")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Priezvisko je povinný údaj")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Heslo je povinný údaj")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Overenie hesla je povinné")]
    [Compare("Password", ErrorMessage = "Heslá sa nezhodujú.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}
