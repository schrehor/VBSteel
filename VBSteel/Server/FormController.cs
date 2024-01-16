using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
	public async Task<IActionResult> SubmitForm(FormModel formData)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest("Invalid form data.");
		}

		Form newForm = new Form()
		{
			Email = formData.Email,
			Message = formData.Message,
			FormId = Guid.NewGuid()
		};
		
		_databaseContext.Forms.Add(newForm);
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
	
	[HttpGet("getUserMessages")]
	public ActionResult<IEnumerable<Form>> GetUserMessages()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
		{
			return Unauthorized("Invalid user ID.");
		}

		var userMessages = _databaseContext.Forms.Where(m => m.UserId == userId).ToList();
		return Ok(userMessages);
	}

	
	[HttpGet("getAllMessages")]
	[Authorize(Roles = "Admin")]
	public ActionResult<IEnumerable<Form>> GetAllMessages()
	{
	
		var allMessages = _databaseContext.Forms.ToList();
		return Ok(allMessages);
	}
}
