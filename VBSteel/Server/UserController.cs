using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class UserController(DatabaseContext databaseContext) : Controller
{
    [HttpGet("GetUserRole")]
    public IActionResult GetUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        if (roleClaim == null)
        {
            return Unauthorized();
        }

        return Ok(roleClaim);
    }
    
    [HttpGet("getUserEmail")]
    public async Task<IActionResult> GetUserEmail()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized("Invalid user ID.");
        }

        var user = await databaseContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        if (user == null)
        {
            return BadRequest("Internal error.");
        }
        
        return Ok(user.Email);
    }
}