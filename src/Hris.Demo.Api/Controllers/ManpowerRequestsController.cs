using Hris.Demo.Api.Services;
using Hris.Demo.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ManpowerRequestsController(MockRspStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<ManpowerRequestDto>> GetAll() => Ok(store.ManpowerRequests);

    [HttpPost("{id:guid}/toggle-submit-approve")]
    public ActionResult<ManpowerRequestDto> ToggleSubmitApprove(Guid id, [FromBody] QueueActionRequestDto body)
    {
        var updated = store.ToggleManpowerSubmitApprove(id, body.ActorRole);
        return updated is null ? NotFound() : Ok(updated);
    }
}
