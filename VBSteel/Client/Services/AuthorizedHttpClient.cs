using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace VBSteel.Client.Services;

public class AuthorizedHttpClient(HttpClient httpClient, AuthenticationProvider authenticationProvider)
{
    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        var token = await authenticationProvider.GetJwtTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        return await httpClient.GetAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync(string requestUri, object content)
    {
        var token = await authenticationProvider.GetJwtTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await httpClient.PostAsJsonAsync(requestUri, content);
    }

    // Todo: PutAsync, DeleteAsync
}
