namespace Hris.Demo.Api.Configuration;

public sealed class AiOptions
{
    public const string SectionName = "Ai";

    public bool Enabled { get; set; } = true;

    /// <summary>Ollama | Gemini</summary>
    public string Provider { get; set; } = "Ollama";

    public AiOllamaOptions Ollama { get; set; } = new();

    public AiGeminiOptions Gemini { get; set; } = new();

    public AiJobDescriptionOptions JobDescription { get; set; } = new();

    public AiQuotaOptions Quota { get; set; } = new();

    public AiCacheOptions Cache { get; set; } = new();
}

public sealed class AiOllamaOptions
{
    /// <summary>Base URL only, e.g. http://localhost:11434 — generator POSTs to /api/generate.</summary>
    public string BaseUrl { get; set; } = "http://localhost:11434";

    public string Model { get; set; } = "gemma2:2b";
}

public sealed class AiGeminiOptions
{
    public string ApiKey { get; set; } = "";

    /// <summary>Model id segment for v1beta generateContent (no "models/" prefix), e.g. gemini-2.0-flash</summary>
    public string Model { get; set; } = "gemini-3.6-flash";
}

public sealed class AiJobDescriptionOptions
{
    /// <summary>Max tokens for provider output (Gemini generationConfig; Ollama num_predict).</summary>
    public int MaxOutputTokens { get; set; } = 1024;

    public int RequestTimeoutSeconds { get; set; } = 30;

    /// <summary>Gemini <c>generationConfig.temperature</c> (0–2). Higher can reduce abrupt one-line stops.</summary>
    public double Temperature { get; set; } = 0.75;
}

public sealed class AiQuotaOptions
{
    public int GlobalRpm { get; set; } = 10;

    public int PerIpRpm { get; set; } = 4;

    public int DailyHardCap { get; set; } = 1500;

    public int DailySoftWarning { get; set; } = 1200;
}

public sealed class AiCacheOptions
{
    public bool Enabled { get; set; } = true;

    public int TtlHours { get; set; } = 24;
}
