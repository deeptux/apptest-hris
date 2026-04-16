using Hris.Demo.Api.Services;
using Hris.Demo.Shared.Ai;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AiController(
    AiJobDescriptionService jobDescription,
    MockRspStore store) : ControllerBase
{
    /// <summary>GET /api/Ai/quota-status</summary>
    [HttpGet("quota-status")]
    public ActionResult<AiQuotaStatusDto> GetQuotaStatus() => Ok(jobDescription.GetQuotaStatus());

    /// <summary>POST /api/Ai/job-description — rate-limited (global + per-IP chained policy).</summary>
    [HttpPost("job-description")]
    [EnableRateLimiting("AiJobDescription")]
    public async Task<IActionResult> GenerateJobDescriptionAsync(
        [FromBody] JobDescriptionGenerateRequest request,
        CancellationToken cancellationToken = default)
    {
        var (ok, statusCode, err) = await jobDescription.GenerateAsync(request, cancellationToken).ConfigureAwait(false);
        if (ok is not null)
        {
            // Metadata only — no generated body in audit (spec §11).
            store.AddAudit(
                actorRole: "Demo",
                action: "AI_JOB_DESCRIPTION_GENERATED",
                entityType: "QualificationStandardRef",
                entityId: Guid.Empty,
                summary: $"fromCache={ok.FromCache}");
            return Ok(ok);
        }

        return StatusCode(statusCode, err);
    }
}
