using Microsoft.AspNetCore.Mvc;

namespace VBSteel.Server;

public class LoginController : Controller
{
    
    
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}