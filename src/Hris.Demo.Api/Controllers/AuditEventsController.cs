using Hris.Demo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuditEventsController(MockRspStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<Shared.Dtos.AuditEventDto>> GetAll() => Ok(store.AuditEvents);
}
