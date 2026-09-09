using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Hris.Demo.Api.Configuration;
using Hris.Demo.Shared.Ai;
using Microsoft.Extensions.Options;

namespace Hris.Demo.Api.Services;

/// <summary>
/// Gemini REST v1beta: POST https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key=API_KEY
/// </summary>
public sealed class GeminiJobDescriptionGenerator(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<AiOptions> options,
    ILogger<GeminiJobDescriptionGenerator> logger) : IJobDescriptionGenerator
{
    public async Task<string> GenerateAsync(JobDescriptionGenerateRequest request, string prompt, CancellationToken cancellationToken = default)
    {
        var ai = options.CurrentValue;
        var g = ai.Gemini;
        var jd = ai.JobDescription;
        if (string.IsNullOrWhiteSpace(g.ApiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured.");
        }

        var maxTokens = Math.Clamp(jd.MaxOutputTokens, 256, 8192);
        var temperature = Math.Clamp(jd.Temperature, 0.0, 2.0);
        var modelSegment = NormalizeGeminiModelId(g.Model);
        var model = Uri.EscapeDataString(modelSegment);
        var url = $"v1beta/models/{model}:generateContent?key={Uri.EscapeDataString(g.ApiKey)}";

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = prompt } },
                },
            },
            generationConfig = new
            {
                maxOutputTokens = maxTokens,
                temperature,
                topP = 0.95,
            },
        };

        var client = httpClientFactory.CreateClient("Gemini");
        using var response = await client.PostAsJsonAsync(url, payload, cancellationToken).ConfigureAwait(false);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Gemini HTTP {Status}: {Body}", (int)response.StatusCode, raw.Length > 200 ? raw[..200] : raw);
            response.EnsureSuccessStatusCode();
        }

        using var doc = JsonDocument.Parse(raw);
        if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("Gemini response missing candidates.");
        }

        var c0 = candidates[0];
        var finishReason = c0.TryGetProperty("finishReason", out var frEl) ? frEl.GetString() : null;
        if (!c0.TryGetProperty("content", out var content) ||
            !content.TryGetProperty("parts", out var parts) ||
            parts.GetArrayLength() == 0)
        {
            logger.LogWarning("Gemini missing content.parts. finishReason={FinishReason}", finishReason);
            throw new InvalidOperationException("Gemini response missing content.parts.");
        }

        var text = ConcatenateTextParts(parts);
        if (string.IsNullOrWhiteSpace(text))
        {
            logger.LogWarning("Gemini returned empty text. finishReason={FinishReason}", finishReason);
            return string.Empty;
        }

        if (text.Length < 400)
        {
            logger.LogWarning(
                "Gemini output is short ({Length} chars). finishReason={FinishReason}. Consider prompt/model if this persists.",
                text.Length,
                finishReason);
        }
        else
        {
            logger.LogDebug("Gemini finishReason={FinishReason}, length={Length}", finishReason, text.Length);
        }

        return text;
    }

    private static string ConcatenateTextParts(JsonElement partsArray)
    {
        var sb = new StringBuilder();
        foreach (var part in partsArray.EnumerateArray())
        {
            if (part.TryGetProperty("text", out var t))
            {
                var s = t.GetString();
                if (!string.IsNullOrEmpty(s))
                {
                    if (sb.Length > 0)
                    {
                        sb.AppendLine();
                    }

                    sb.Append(s);
                }
            }
        }

        return sb.ToString();
    }

    /// <summary>Strip accidental "models/" prefix; Google path is v1beta/models/{id}:generateContent.</summary>
    private static string NormalizeGeminiModelId(string? configured)
    {
        var m = (configured ?? string.Empty).Trim();
        if (m.StartsWith("models/", StringComparison.OrdinalIgnoreCase))
        {
            m = m["models/".Length..];
        }

        return string.IsNullOrEmpty(m) ? "gemini-3.6-flash" : m;
    }
}
