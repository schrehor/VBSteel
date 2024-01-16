using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VBSteel.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("VBSteel.ServerAPI", client => 
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddScoped<AuthenticationProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<AuthenticationProvider>());
builder.Services.AddScoped<AuthorizedHttpClient>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("VBSteel.ServerAPI"));

await builder.Build().RunAsync();
