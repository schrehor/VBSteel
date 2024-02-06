using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace VBSteel.Client.Services;

public class AuthorizedHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationProvider _authenticationProvider;

    public AuthorizedHttpClient(HttpClient httpClient, AuthenticationProvider authenticationProvider)
    {
		this._httpClient = httpClient;
		this._authenticationProvider = authenticationProvider;
	}

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        var token = await _authenticationProvider.GetJwtTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        return await _httpClient.GetAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync(string requestUri, object content)
    {
        var token = await _authenticationProvider.GetJwtTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await _httpClient.PostAsJsonAsync(requestUri, content);
    }

    // Todo: PutAsync, DeleteAsync
}
