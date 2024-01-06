using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;

public class Form
{
	[Key]
	public Guid FormId { get; set; }

	public Guid? UserId { get; set; }

	[Required, EmailAddress]
	public string Email { get; set; }

	[Required, StringLength(2000)]
	public string Message { get; set; }

	public User User { get; set; }
}
