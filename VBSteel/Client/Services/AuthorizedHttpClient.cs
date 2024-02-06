using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace VBSteel.Client.Services;

public class AuthorizedHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationProvider _authenticationProvider;

    public AuthorizedHttpClient(HttpClient httpClient, AuthenticationProvider authenticationProvider)
    {
		_httpClient = httpClient;
		_authenticationProvider = authenticationProvider;

        SetAuthorizationHeaderAsync();
        _authenticationProvider.OnTokenChanged += SetAuthorizationHeaderAsync;
    }

    private async void SetAuthorizationHeaderAsync()
    {
        var token = await _authenticationProvider.GetJwtTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(token) ?
            new AuthenticationHeaderValue("Bearer", token) : null;
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        if (_httpClient.DefaultRequestHeaders.Authorization is null)
        {
            return CreateUnauthorizedResponse();
        }

        return await _httpClient.GetAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync(string requestUri, object content)
    {
        if (_httpClient.DefaultRequestHeaders.Authorization is null)
        {
            return CreateUnauthorizedResponse();
        }

        return await _httpClient.PostAsJsonAsync(requestUri, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        if (_httpClient.DefaultRequestHeaders.Authorization is null)
        {
            return CreateUnauthorizedResponse();
        }

        return await _httpClient.DeleteAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync(string requestUri, object content)
    {
        if (_httpClient.DefaultRequestHeaders.Authorization is null)
        {
            return CreateUnauthorizedResponse();
        }

        return await _httpClient.PutAsJsonAsync(requestUri, content);
    }

    private HttpResponseMessage CreateUnauthorizedResponse()
    {
        return new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("User is not authenticated.")
        };
    }
}
