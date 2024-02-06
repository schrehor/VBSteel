using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using VBSteel.Shared;

namespace VBSteel.Client.Services;

public class AuthenticationProvider : AuthenticationStateProvider
{
    private const string AuthTokenKey = "authToken";
    private ClaimsPrincipal? _user;
	private readonly IJSRuntime _jsRuntime;

    public AuthenticationProvider(IJSRuntime jsRuntime)
    {
	    this._jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var authToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AuthTokenKey);

        var identity = new ClaimsIdentity();
        if (!string.IsNullOrEmpty(authToken))
        {
            try
            {
                var userClaims = ParseClaimsFromJwt(authToken);
                identity = new ClaimsIdentity(userClaims, "jwt");
            }
            catch (Exception)
            {
                // todo: exception handling
            }
        }

        _user = new ClaimsPrincipal(identity);
		return new AuthenticationState(_user);
    }

    public async Task<string> GetJwtTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AuthTokenKey);
    }

    public UserRole DetermineUserRole()
    {
	    if (_user is null)
	    {
		    return UserRole.NotLoggedIn;
	    }

	    var isAdmin = _user.IsInRole("Admin");
	    return isAdmin ? UserRole.Admin : UserRole.RegularUser;
    }

	public async Task MarkUserAsAuthenticated(string authToken)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AuthTokenKey, authToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    
    public async Task MarkUserAsLoggedOut()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AuthTokenKey);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        keyValuePairs.TryGetValue("nameid", out var nameIdentifier);
        if (nameIdentifier != null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier.ToString()));
        }
        
        keyValuePairs.TryGetValue("role", out var role);
        if (role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }

        return claims;
    }
    
    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

}