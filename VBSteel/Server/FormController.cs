using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VBSteel.Shared;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class FormController : ControllerBase
{
	private readonly DatabaseContext _databaseContext;

	public FormController(DatabaseContext databaseContext)
	{
		this._databaseContext = databaseContext;
	}

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
	public async Task<IActionResult> UpdateFormData(Guid id, FormModel updatedFormData)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest("Invalid form data.");
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
	public async Task<ActionResult<IEnumerable<Form>>> GetUserMessages()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
		{
			return Unauthorized("Invalid user ID.");
		}

		var userMessages = await _databaseContext.Forms
			.Where(m => m.UserId == userId)
			.Include(f => f.User)
			.ToListAsync();

		return Ok(userMessages);
	}

	
	[HttpGet("getAllMessages")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<IEnumerable<Form>>> GetAllMessages()
	{
		var allMessages = await _databaseContext.Forms
			.Include(f => f.User)
			.ToListAsync();

		return Ok(allMessages);
	}
}
