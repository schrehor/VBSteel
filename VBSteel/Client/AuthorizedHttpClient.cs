using System.Net.Http.Headers;

namespace VBSteel.Client;

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

    // Todo: PostAsync, PutAsync, DeleteAsync
}
