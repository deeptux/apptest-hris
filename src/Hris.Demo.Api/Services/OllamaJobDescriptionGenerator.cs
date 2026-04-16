using System.Net.Http.Json;
using System.Text.Json;
using Hris.Demo.Api.Configuration;
using Hris.Demo.Shared.Ai;
using Microsoft.Extensions.Options;

namespace Hris.Demo.Api.Services;

/// <summary>
/// Ollama REST: POST {BaseUrl}/api/generate with JSON body model, prompt, stream=false.
/// See https://github.com/ollama/ollama/blob/main/docs/api.md#generate-a-completion
/// </summary>
public sealed class OllamaJobDescriptionGenerator(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<AiOptions> options,
    ILogger<OllamaJobDescriptionGenerator> logger) : IJobDescriptionGenerator
{
    public async Task<string> GenerateAsync(JobDescriptionGenerateRequest request, string prompt, CancellationToken cancellationToken = default)
    {
        var ai = options.CurrentValue;
        var o = ai.Ollama;
        var maxTokens = Math.Clamp(ai.JobDescription.MaxOutputTokens, 64, 8192);
        var baseUrl = o.BaseUrl.TrimEnd('/');

        // Contract: /api/generate, non-streaming JSON.
        var payload = new
        {
            model = o.Model,
            prompt,
            stream = false,
            options = new { num_predict = maxTokens },
        };

        var client = httpClientFactory.CreateClient("Ollama");
        using var response = await client.PostAsJsonAsync($"{baseUrl}/api/generate", payload, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            logger.LogWarning("Ollama HTTP {Status}: {Body}", (int)response.StatusCode, body.Length > 200 ? body[..200] : body);
            response.EnsureSuccessStatusCode();
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!doc.RootElement.TryGetProperty("response", out var responseText))
        {
            throw new InvalidOperationException("Ollama response JSON missing 'response' field.");
        }

        return responseText.GetString() ?? string.Empty;
    }
}
