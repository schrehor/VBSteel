using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VBSteel.Shared;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class FormController(DatabaseContext databaseContext) : ControllerBase
{
	[HttpPost("submitForm")]
	public async Task<IActionResult> SubmitForm(FormModel formData)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest("Invalid form data.");
		}

		Form newForm;
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
		{
			newForm = new Form()
			{
				Email = formData.Email,
				Message = formData.Message,
				FormId = Guid.NewGuid()
			};
		}
		else
		{
			newForm = new Form()
			{
				Email = formData.Email,
				Message = formData.Message,
				FormId = Guid.NewGuid(),
				UserId = userId
			};
		}
		
		databaseContext.Forms.Add(newForm);
		try
		{
			await databaseContext.SaveChangesAsync();
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
		var formData = await databaseContext.Forms.FirstOrDefaultAsync();

		if (formData == null)
		{
			return NotFound();
		}

		return Ok(formData);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteFormData(Guid id)
	{
		
		var formData = await databaseContext.Forms.FindAsync(id);

		if (formData == null)
		{
			return NotFound();
		}

		databaseContext.Forms.Remove(formData);
		await databaseContext.SaveChangesAsync();

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

		var existingFormData = await databaseContext.Forms.FindAsync(id);

		if (existingFormData == null)
		{
			return NotFound();
		}

		existingFormData.Email = updatedFormData.Email;
		existingFormData.Message = updatedFormData.Message;

		databaseContext.Entry(existingFormData).State = EntityState.Modified;

		await databaseContext.SaveChangesAsync();

		return NoContent();
	}
	
	[HttpGet("getUserMessages")]
	public async Task<ActionResult<IEnumerable<Form>>> GetUserMessages()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
		{
			return Unauthorized("Invalid user ID.");
		}

		var userMessages = await databaseContext.Forms.Where(m => m.UserId == userId).ToListAsync();
		return Ok(userMessages);
	}

	
	[HttpGet("getAllMessages")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<IEnumerable<Form>>> GetAllMessages()
	{
		var allMessages = await databaseContext.Forms.ToListAsync();
		return Ok(allMessages);
	}
}
