using System.Text.Json;
using System.Text.Json.Serialization;
using Hris.Demo.Api.Configuration;
using Hris.Demo.Api.Data;
using Hris.Demo.Api.Services;
using Hris.Demo.Shared;
using Hris.Demo.Shared.Ai;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var portEnv = Environment.GetEnvironmentVariable("PORT")?.Trim();
if (!string.IsNullOrWhiteSpace(portEnv) && int.TryParse(portEnv, out var renderPort) && renderPort > 0)
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{renderPort}");
}

builder.Services.Configure<BrandingOptions>(builder.Configuration.GetSection(BrandingOptions.SectionName));
builder.Services.Configure<AiOptions>(builder.Configuration.GetSection(AiOptions.SectionName));

builder.Services.AddSingleton<MockRspStore>();

var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDir);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AppDb")));

StorageRegistrar.AddObjectStorage(builder);
builder.Services.AddScoped<ApplicantProfileFilesService>();
builder.Services.AddSingleton<AiDailyQuotaTracker>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<OllamaJobDescriptionGenerator>();
builder.Services.AddSingleton<GeminiJobDescriptionGenerator>();
builder.Services.AddSingleton<IJobDescriptionGenerator>(sp =>
{
    var ai = sp.GetRequiredService<IOptionsMonitor<AiOptions>>().CurrentValue;
    return string.Equals(ai.Provider, "Gemini", StringComparison.OrdinalIgnoreCase)
        ? sp.GetRequiredService<GeminiJobDescriptionGenerator>()
        : sp.GetRequiredService<OllamaJobDescriptionGenerator>();
});
builder.Services.AddSingleton<AiJobDescriptionService>();

var aiLimiterSnapshot = builder.Configuration.GetSection(AiOptions.SectionName).Get<AiOptions>() ?? new AiOptions();
var aiJobDescriptionRateLimiterPolicy = new AiJobDescriptionRateLimiterPolicy(Microsoft.Extensions.Options.Options.Create(aiLimiterSnapshot));
builder.Services.AddSingleton(aiJobDescriptionRateLimiterPolicy);

builder.Services.AddHttpClient("Ollama", (sp, client) =>
{
    var ai = sp.GetRequiredService<IOptionsMonitor<AiOptions>>().CurrentValue;
    client.Timeout = TimeSpan.FromSeconds(Math.Clamp(ai.JobDescription.RequestTimeoutSeconds, 5, 120));
});
builder.Services.AddHttpClient("Gemini", (sp, client) =>
{
    var ai = sp.GetRequiredService<IOptionsMonitor<AiOptions>>().CurrentValue;
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
    client.Timeout = TimeSpan.FromSeconds(Math.Clamp(ai.JobDescription.RequestTimeoutSeconds, 5, 120));
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new AiErrorResponse(AiErrorCodes.RateLimit));
        await context.HttpContext.Response.WriteAsync(body, cancellationToken).ConfigureAwait(false);
    };

    options.AddPolicy("AiJobDescription", aiJobDescriptionRateLimiterPolicy);
});

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });
builder.Services.AddOpenApi();

var corsOrigins = ResolveCorsOrigins(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins.Length > 0 ? corsOrigins : ["https://localhost:7117", "http://localhost:5027"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

static string[] ResolveCorsOrigins(IConfiguration configuration)
{
    var fromEnv = Environment.GetEnvironmentVariable("CORS_ORIGINS");
    if (!string.IsNullOrWhiteSpace(fromEnv))
    {
        return fromEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    return configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync().ConfigureAwait(false);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseRateLimiter();
app.MapControllers();

app.Run();
