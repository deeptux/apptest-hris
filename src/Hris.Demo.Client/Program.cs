using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Hris.Demo.Client;
using Hris.Demo.Client.Services;
using Hris.Demo.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

using (var configHttp = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
{
    await using (var stream = await configHttp.GetStreamAsync("appsettings.json").ConfigureAwait(false))
    {
        builder.Configuration.AddJsonStream(stream);
    }

    var envName = builder.HostEnvironment.Environment;
    var envFile = $"appsettings.{envName}.json";
    try
    {
        using var resp = await configHttp.GetAsync(envFile).ConfigureAwait(false);
        if (resp.IsSuccessStatusCode)
        {
            await using var envStream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
            builder.Configuration.AddJsonStream(envStream);
        }
    }
    catch
    {
        /* Optional environment file (e.g. not present in dev) */
    }
}

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBase = builder.Configuration["ApiBaseUrl"]?.TrimEnd('/');
if (string.IsNullOrEmpty(apiBase))
    apiBase = "https://localhost:7209";

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBase + "/") });
builder.Services.AddScoped<RspDemoApiService>();
builder.Services.AddScoped<AiAssistantApiService>();

var branding = builder.Configuration.GetSection(BrandingOptions.SectionName).Get<BrandingOptions>() ?? new BrandingOptions();
builder.Services.AddSingleton(branding);

await builder.Build().RunAsync().ConfigureAwait(false);
