using System.Collections.Concurrent;

namespace Hris.Demo.Api.Services;

/// <summary>
/// Process-local daily counter (UTC day key yyyyMMdd). Uses Interlocked on per-day boxes — cache hits must not call reserve/release.
/// </summary>
public sealed class AiDailyQuotaTracker
{
    private sealed class Counter
    {
        public long Value;
    }

    private readonly ConcurrentDictionary<string, Counter> _byDay = new(StringComparer.Ordinal);

    public static string UtcDayKey() => DateTime.UtcNow.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

    public long Peek(string dayKey) =>
        _byDay.TryGetValue(dayKey, out var c) ? Volatile.Read(ref c.Value) : 0;

    /// <summary>Atomically increment if current count is below hard cap; otherwise return false.</summary>
    public bool TryReserve(string dayKey, long hardCap, out long countAfter)
    {
        var box = _byDay.GetOrAdd(dayKey, static _ => new Counter());
        while (true)
        {
            var cur = Volatile.Read(ref box.Value);
            if (cur >= hardCap)
            {
                countAfter = cur;
                return false;
            }

            if (Interlocked.CompareExchange(ref box.Value, cur + 1, cur) == cur)
            {
                countAfter = cur + 1;
                return true;
            }
        }
    }

    /// <summary>Refund a reserved slot after a failed live generation (not used on cache hit).</summary>
    public void Release(string dayKey)
    {
        if (!_byDay.TryGetValue(dayKey, out var c))
        {
            return;
        }

        while (true)
        {
            var cur = Volatile.Read(ref c.Value);
            var next = Math.Max(0, cur - 1);
            if (Interlocked.CompareExchange(ref c.Value, next, cur) == cur)
            {
                return;
            }
        }
    }
}
