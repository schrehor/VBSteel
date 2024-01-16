using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;

public class LoginViewModel
{
    [Required(ErrorMessage = "Emailová adresa je povinná")]
    [EmailAddress(ErrorMessage = "Neplatná emailová adresa")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Heslo je povinný údaj")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
