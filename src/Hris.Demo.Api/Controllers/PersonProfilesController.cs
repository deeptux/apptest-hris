using Hris.Demo.Api.Services;
using Hris.Demo.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PersonProfilesController(MockRspStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<PersonProfileDto>> GetAll([FromQuery] bool includeInactive = false) =>
        Ok(store.GetPersonProfiles(includeInactive));

    [HttpPost]
    public ActionResult<PersonProfileDto> Create([FromBody] PersonProfileUpsertDto body)
    {
        var created = store.CreatePersonProfile(body);
        return Ok(created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<PersonProfileDto> Update(Guid id, [FromBody] PersonProfileUpsertDto body)
    {
        var updated = store.UpdatePersonProfile(id, body);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/deactivate")]
    public ActionResult<PersonProfileDto> Deactivate(Guid id)
    {
        var updated = store.DeactivatePersonProfile(id);
        return updated is null ? NotFound() : Ok(updated);
    }
}
