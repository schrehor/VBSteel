using Microsoft.AspNetCore.Mvc;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class FormController : ControllerBase
{
	
	[HttpPost("submitForm")]
	public async Task<IActionResult> SubmitForm(FormData formData)
	{
		if (!IsValidName(formData.Name) || !IsValidText(formData.Text))
		{
			return BadRequest("Invalid form data.");
		}

		return Ok("Form data submitted successfully!");
	}

	private bool IsValidName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return false;
		}

		var words = name.Trim().Split(' ');
		return words.Length is >= 1 and <= 2 && words.All(IsAlphaCharacters);
	}

	private bool IsAlphaCharacters(string value)
	{
		return !string.IsNullOrEmpty(value) && value.All(char.IsLetter);
	}

	private bool IsValidText(string text)
	{
		return !string.IsNullOrWhiteSpace(text);
	}
}
