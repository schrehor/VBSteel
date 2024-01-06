using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VBSteel.Shared;

namespace VBSteel.Server;
//Todo: Prerobit podla novej DB architektury
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
	public async Task<IActionResult> SubmitForm(Form formData)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest("Invalid form data.");
		}

		formData.FormId = Guid.NewGuid();
		_databaseContext.Forms.Add(formData);
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
		var formData = await _databaseContext.Forms.FirstOrDefaultAsync();

		if (formData == null)
		{
			return NotFound();
		}

		return Ok(formData);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteFormData(Guid id)
	{
		
		var formData = await _databaseContext.Forms.FindAsync(id);

		if (formData == null)
		{
			return NotFound();
		}

		_databaseContext.Forms.Remove(formData);
		await _databaseContext.SaveChangesAsync();

		return NoContent();
	}

	[HttpPut("updateForm/{id}")]
	public async Task<IActionResult> UpdateFormData(Guid id, Form updatedFormData)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest("Invalid form data.");
		}
		
		if (id != updatedFormData.FormId)
		{
			return BadRequest();
		}

		var existingFormData = await _databaseContext.Forms.FindAsync(id);

		if (existingFormData == null)
		{
			return NotFound();
		}

		existingFormData.Email = updatedFormData.Email;
		existingFormData.Message = updatedFormData.Message;

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
