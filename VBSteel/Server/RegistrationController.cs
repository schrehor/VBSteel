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
    public async Task<IActionResult> RegisterUser([FromBody] RegistrationViewModel registrationModel)
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
                Role = UserRole.RegularUser
            };

            _databaseContext.Users.Add(newUser);
            await _databaseContext.SaveChangesAsync();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = "NajtajnejsiKlucKtoryNiktoNikdyNeuhadne"u8.ToArray();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, newUser.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            
            // todo: pouzit CreatedAtAction?
            return Ok(new { Token = tokenString });
        }

        return BadRequest(ModelState);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}