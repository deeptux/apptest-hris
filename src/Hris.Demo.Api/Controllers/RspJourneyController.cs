using Hris.Demo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RspJourneyController(MockRspStore store) : ControllerBase
{
    [HttpGet("steps")]
    public ActionResult<IReadOnlyList<Shared.RspJourneyStepDto>> GetSteps() => Ok(store.JourneySteps);
}
