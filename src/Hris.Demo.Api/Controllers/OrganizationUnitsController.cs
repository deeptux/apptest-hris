using Hris.Demo.Api.Services;
using Hris.Demo.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrganizationUnitsController(MockRspStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<OrganizationUnitDto>> GetAll([FromQuery] bool includeInactive = false) =>
        Ok(store.GetOrganizationUnits(includeInactive));

    [HttpPost]
    public ActionResult<OrganizationUnitDto> Create([FromBody] OrganizationUnitUpsertDto body)
    {
        var created = store.CreateOrganizationUnit(body);
        return Ok(created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<OrganizationUnitDto> Update(Guid id, [FromBody] OrganizationUnitUpsertDto body)
    {
        var updated = store.UpdateOrganizationUnit(id, body);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/deactivate")]
    public ActionResult<OrganizationUnitDto> Deactivate(Guid id)
    {
        var updated = store.DeactivateOrganizationUnit(id);
        return updated is null ? NotFound() : Ok(updated);
    }
}
