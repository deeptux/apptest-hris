using Hris.Demo.Api.Services;
using Hris.Demo.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VacanciesController(MockRspStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<VacancyDto>> GetAll() => Ok(store.Vacancies);

    [HttpPost("{id:guid}/toggle-publish")]
    public ActionResult<VacancyDto> TogglePublish(Guid id, [FromBody] QueueActionRequestDto body)
    {
        var updated = store.ToggleVacancyPublish(id, body.ActorRole);
        return updated is null ? NotFound() : Ok(updated);
    }
}
