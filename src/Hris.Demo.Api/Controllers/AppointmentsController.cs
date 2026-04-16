using Hris.Demo.Api.Services;
using Hris.Demo.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AppointmentsController(MockRspStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<AppointmentPackageDto>> GetAll() => Ok(store.Appointments);

    [HttpPost("{id:guid}/mark-appointed")]
    public ActionResult<AppointmentPackageDto> MarkAsAppointed(Guid id, [FromBody] AppointmentMarkAsAppointedDto body)
    {
        var updated = store.MarkAsAppointed(id, body.ActorRole, body.Effectivity);
        return updated is null ? NotFound() : Ok(updated);
    }
}
