using System.Security.Cryptography;
using System.Text;
using Hris.Demo.Api.Configuration;
using Hris.Demo.Shared.Ai;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Hris.Demo.Api.Services;

public sealed class AiJobDescriptionService(
    IOptionsMonitor<AiOptions> options,
    AiDailyQuotaTracker quotaTracker,
    IMemoryCache cache,
    IJobDescriptionGenerator generator,
    ILogger<AiJobDescriptionService> logger)
{
    private static DateTimeOffset NextUtcMidnight()
    {
        var now = DateTime.UtcNow;
        return new DateTimeOffset(now.Date.AddDays(1), TimeSpan.Zero);
    }

    public AiQuotaStatusDto GetQuotaStatus()
    {
        var ai = options.CurrentValue;
        if (!ai.Enabled)
        {
            return new AiQuotaStatusDto(
                CanGenerate: false,
                SoftWarning: false,
                DailyLimitReached: false,
                ResetsAtUtc: null);
        }

        var dayKey = AiDailyQuotaTracker.UtcDayKey();
        var count = quotaTracker.Peek(dayKey);
        var hard = ai.Quota.DailyHardCap;
        var soft = ai.Quota.DailySoftWarning;

        var dailyReached = count >= hard;
        var softWarn = count >= soft && !dailyReached;
        var canGenerate = !dailyReached;

        return new AiQuotaStatusDto(
            CanGenerate: canGenerate,
            SoftWarning: softWarn,
            DailyLimitReached: dailyReached,
            ResetsAtUtc: NextUtcMidnight());
    }

    /// <summary>POST job-description: success body or HTTP status + canonical error.</summary>
    public async Task<(JobDescriptionGenerateResponse? Ok, int StatusCode, AiErrorResponse? Error)> GenerateAsync(
        JobDescriptionGenerateRequest request,
        CancellationToken cancellationToken = default)
    {
        var ai = options.CurrentValue;
        if (!ai.Enabled)
        {
            return (null, StatusCodes.Status503ServiceUnavailable, new AiErrorResponse(AiErrorCodes.AiDisabled, "AI assistance is turned off."));
        }

        if (string.IsNullOrWhiteSpace(request.PositionTitle))
        {
            return (null, StatusCodes.Status400BadRequest, new AiErrorResponse("INVALID_REQUEST", "Position title is required."));
        }

        var memKey = "jd_ai_" + BuildCacheKey(request);
        if (ai.Cache.Enabled &&
            cache.TryGetValue(memKey, out string? cached) &&
            !string.IsNullOrEmpty(cached))
        {
            // Cache hits do not count toward daily cap (spec).
            return (new JobDescriptionGenerateResponse(cached, FromCache: true), StatusCodes.Status200OK, null);
        }

        var dayKey = AiDailyQuotaTracker.UtcDayKey();
        var hard = ai.Quota.DailyHardCap;
        if (quotaTracker.Peek(dayKey) >= hard)
        {
            return (null, StatusCodes.Status429TooManyRequests, new AiErrorResponse(AiErrorCodes.DailyLimitReached));
        }

        if (!quotaTracker.TryReserve(dayKey, hard, out _))
        {
            return (null, StatusCodes.Status429TooManyRequests, new AiErrorResponse(AiErrorCodes.DailyLimitReached));
        }

        var prompt = JobDescriptionPromptBuilder.Build(request);
        string text;
        try
        {
            text = await generator.GenerateAsync(request, prompt, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            quotaTracker.Release(dayKey);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Job description generation failed.");
            quotaTracker.Release(dayKey);
            return (null, StatusCodes.Status503ServiceUnavailable,
                new AiErrorResponse(AiErrorCodes.ProviderUnavailable, "The writing service is temporarily unavailable."));
        }

        text = text.Trim();
        if (ai.Cache.Enabled && !string.IsNullOrEmpty(text))
        {
            cache.Set(
                memKey,
                text,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(Math.Clamp(ai.Cache.TtlHours, 1, 168)),
                });
        }

        return (new JobDescriptionGenerateResponse(text, FromCache: false), StatusCodes.Status200OK, null);
    }

    private static string BuildCacheKey(JobDescriptionGenerateRequest r)
    {
        var lang = string.IsNullOrWhiteSpace(r.Language) ? "en" : r.Language.Trim();
        var s = $"{r.PositionTitle.Trim()}|{r.Department?.Trim() ?? ""}|{r.EmploymentType?.Trim() ?? ""}|{lang}".ToLowerInvariant();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(s));
        return Convert.ToHexString(hash);
    }
}
