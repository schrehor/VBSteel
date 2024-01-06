﻿using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;

public class Form
{
	[Key]
	public Guid FormId { get; set; }

	public Guid? UserId { get; set; }

	[Required(ErrorMessage = "Emailová adresa je povinná")]
	[EmailAddress(ErrorMessage = "Neplatná emailová adresa")]
	public string Email { get; set; }

	[Required(ErrorMessage = "Telo správy nemôže byť prázdne")]
	[StringLength(2000, ErrorMessage = "Maximálna dĺžka správy je 2000 znakov")]
	public string Message { get; set; }

	public User User { get; set; }
}
