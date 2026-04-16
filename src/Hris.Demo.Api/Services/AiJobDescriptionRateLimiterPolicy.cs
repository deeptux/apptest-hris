using System.Threading.RateLimiting;
using Hris.Demo.Api.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Hris.Demo.Api.Services;

/// <summary>
/// Chains a shared global fixed-window limiter with a per-IP fixed-window limiter (spec §5).
/// </summary>
internal sealed class AiJobDescriptionRateLimiterPolicy(IOptions<AiOptions> options) : IRateLimiterPolicy<string>, IDisposable
{
    private readonly FixedWindowRateLimiter _globalLimiter = new(new FixedWindowRateLimiterOptions
    {
        AutoReplenishment = true,
        PermitLimit = Math.Max(1, options.Value.Quota.GlobalRpm),
        Window = TimeSpan.FromMinutes(1),
        QueueLimit = 0,
    });

    public void Dispose() => _globalLimiter.Dispose();

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        // RemoteIpAddress: best-effort behind reverse proxy unless forwarded headers are configured (spec §5).
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var perIp = Math.Max(1, options.Value.Quota.PerIpRpm);
        return RateLimitPartition.Get(
            ip,
            _ => new ChainedTwoRateLimiter(
                _globalLimiter,
                new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = perIp,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                })));
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => null;
}

/// <summary>Acquires from <paramref name="first"/> then <paramref name="second"/>; releases first if second fails.</summary>
internal sealed class ChainedTwoRateLimiter : RateLimiter
{
    private readonly RateLimiter _first;
    private readonly RateLimiter _second;

    public ChainedTwoRateLimiter(RateLimiter first, RateLimiter second)
    {
        _first = first;
        _second = second;
    }

    public override TimeSpan? IdleDuration => _second.IdleDuration;

    protected override RateLimitLease AttemptAcquireCore(int permitCount)
    {
        var a = _first.AttemptAcquire(permitCount);
        if (!a.IsAcquired)
        {
            return a;
        }

        var b = _second.AttemptAcquire(permitCount);
        if (!b.IsAcquired)
        {
            a.Dispose();
            return b;
        }

        return new DualRateLimitLease(a, b);
    }

    protected override async ValueTask<RateLimitLease> AcquireAsyncCore(int permitCount, CancellationToken cancellationToken)
    {
        var a = await _first.AcquireAsync(permitCount, cancellationToken).ConfigureAwait(false);
        if (!a.IsAcquired)
        {
            return a;
        }

        var b = await _second.AcquireAsync(permitCount, cancellationToken).ConfigureAwait(false);
        if (!b.IsAcquired)
        {
            a.Dispose();
            return b;
        }

        return new DualRateLimitLease(a, b);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _second.Dispose(); // per-IP limiter owned by this chain instance; _first is shared and not disposed here.
        }

        base.Dispose(disposing);
    }

    public override RateLimiterStatistics? GetStatistics() => _second.GetStatistics();
}

internal sealed class DualRateLimitLease : RateLimitLease
{
    private readonly RateLimitLease _a;
    private readonly RateLimitLease _b;
    private bool _disposed;

    public DualRateLimitLease(RateLimitLease a, RateLimitLease b)
    {
        _a = a;
        _b = b;
    }

    public override bool IsAcquired => _a.IsAcquired && _b.IsAcquired;

    public override IEnumerable<string> MetadataNames => Array.Empty<string>();

    public override bool TryGetMetadata(string metadataName, out object? metadata)
    {
        metadata = null;
        return false;
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (disposing)
        {
            _b.Dispose();
            _a.Dispose();
        }

        base.Dispose(disposing);
    }
}
