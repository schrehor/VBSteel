using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class FormController : ControllerBase
{
	private readonly DatabaseContext _databaseContext;

	public FormController(DatabaseContext databaseContext)
	{
		_databaseContext = databaseContext;
	}
	
	[HttpPost("submitForm")]
	public async Task<IActionResult> SubmitForm(FormData formData)
	{
		if (!IsValidName(formData.Name) || !IsValidText(formData.Text))
		{
			return BadRequest("Invalid form data.");
		}

		formData.Id = Guid.NewGuid();
		_databaseContext.FormData.Add(formData);
		try
		{
			await _databaseContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
		
		return Ok("Form data submitted successfully!");
	}

	[HttpGet("first")]
	public async Task<IActionResult> GetFormDataById(Guid id)
	{
		var formData = await _databaseContext.FormData.FirstOrDefaultAsync();

		if (formData == null)
		{
			return NotFound();
		}

		return Ok(formData);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteFormData(Guid id)
	{
		var formData = await _databaseContext.FormData.FindAsync(id);

		if (formData == null)
		{
			return NotFound();
		}

		_databaseContext.FormData.Remove(formData);
		await _databaseContext.SaveChangesAsync();

		return NoContent();
	}

	[HttpPut("updateForm/{id}")]
	public async Task<IActionResult> UpdateFormData(Guid id, FormData updatedFormData)
	{
		if (id != updatedFormData.Id)
		{
			return BadRequest();
		}

		var existingFormData = await _databaseContext.FormData.FindAsync(id);

		if (existingFormData == null)
		{
			return NotFound();
		}

		existingFormData.Name = updatedFormData.Name;
		existingFormData.Text = updatedFormData.Text;

		_databaseContext.Entry(existingFormData).State = EntityState.Modified;

		await _databaseContext.SaveChangesAsync();

		return NoContent();
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
