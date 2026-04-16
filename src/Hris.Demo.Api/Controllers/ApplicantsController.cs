using Hris.Demo.Api.Services;
using Hris.Demo.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ApplicantsController(MockRspStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<ApplicantDto>> GetAll() => Ok(store.Applicants);

    [HttpPost("{id:guid}/screening-outcome")]
    public ActionResult<ApplicantDto> SetScreeningOutcome(Guid id, [FromBody] ApplicantScreeningUpdateDto body)
    {
        var updated = store.SetApplicantOutcome(id, body.ActorRole, body.Outcome);
        return updated is null ? NotFound() : Ok(updated);
    }
}
