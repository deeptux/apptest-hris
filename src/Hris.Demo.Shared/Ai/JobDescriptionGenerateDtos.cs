using System.Text.Json.Serialization;

namespace Hris.Demo.Shared.Ai;

/// <summary>Request body for POST /api/Ai/job-description. Only <see cref="PositionTitle"/> is required; omit null optional fields from the client when not collected in UI.</summary>
public sealed record JobDescriptionGenerateRequest(
    [property: JsonPropertyName("positionTitle")] string PositionTitle,
    [property: JsonPropertyName("department")] string? Department = null,
    [property: JsonPropertyName("employmentType")] string? EmploymentType = null,
    [property: JsonPropertyName("language")] string Language = "en");

public sealed record JobDescriptionGenerateResponse(
    [property: JsonPropertyName("generatedDescription")] string GeneratedDescription,
    [property: JsonPropertyName("fromCache")] bool FromCache);

/// <summary>Quota snapshot for Generate AI button state. Server owns semantics per implementation spec.</summary>
public sealed record AiQuotaStatusDto(
    [property: JsonPropertyName("canGenerate")] bool CanGenerate,
    [property: JsonPropertyName("softWarning")] bool SoftWarning,
    [property: JsonPropertyName("dailyLimitReached")] bool DailyLimitReached,
    [property: JsonPropertyName("resetsAtUtc")] DateTimeOffset? ResetsAtUtc);

/// <summary>Canonical error JSON for AI POST (and aligned errors).</summary>
public sealed record AiErrorResponse(
    [property: JsonPropertyName("errorCode")] string ErrorCode,
    [property: JsonPropertyName("message")] string? Message = null);
