using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VBSteel.Shared;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController : Controller
{
    private readonly DatabaseContext _databaseContext;

    public RegistrationController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmailExists(string email)
    {
        var emailExists = await _databaseContext.Users.AnyAsync(u => u.Email == email);
        return Ok(emailExists);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegistrationModel registrationModel)
    {
        if (ModelState.IsValid)
        {
            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = registrationModel.Email,
                Name = registrationModel.Name,
                Surname = registrationModel.Surname,
                PasswordHash = HashPassword(registrationModel.Password),
                Role = registrationModel.Email.Contains("vbsteel") ? UserRole.Admin : UserRole.RegularUser
            };

            _databaseContext.Users.Add(newUser);
            await _databaseContext.SaveChangesAsync();

            var tokenString = TokenGenerator.GenerateToken(newUser);
            
            // todo: pouzit CreatedAtAction?
            return Ok(tokenString);
        }

        return BadRequest(ModelState);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}