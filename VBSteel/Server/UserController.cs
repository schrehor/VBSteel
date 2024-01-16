using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
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
}