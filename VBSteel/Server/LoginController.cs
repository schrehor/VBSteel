using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VBSteel.Shared;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class LoginController : Controller
{
    private readonly DatabaseContext _databaseContext;

    public LoginController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginViewModel login)
    {
        // Lookup user by email
        var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
        if (user == null)
        {
            return Unauthorized("User does not exist.");
        }

        bool isPasswordVerified = VerifyPassword(login.Password, user.PasswordHash);
        if (!isPasswordVerified)
        {
            return Unauthorized("Invalid password.");
        }

        string token = TokenGenerator.GenerateToken(user);

        return Ok(token);
    }
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}