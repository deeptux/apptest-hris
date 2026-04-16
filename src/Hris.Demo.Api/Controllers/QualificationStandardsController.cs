using Hris.Demo.Api.Services;
using Hris.Demo.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class QualificationStandardsController(MockRspStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<QualificationStandardRefDto>> GetAll([FromQuery] bool includeInactive = false) =>
        Ok(store.GetQualificationStandards(includeInactive));

    [HttpPost]
    public ActionResult<QualificationStandardRefDto> Create([FromBody] QualificationStandardRefUpsertDto body)
    {
        var created = store.CreateQualificationStandard(body);
        return Ok(created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<QualificationStandardRefDto> Update(Guid id, [FromBody] QualificationStandardRefUpsertDto body)
    {
        var updated = store.UpdateQualificationStandard(id, body);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/deactivate")]
    public ActionResult<QualificationStandardRefDto> Deactivate(Guid id)
    {
        var updated = store.DeactivateQualificationStandard(id);
        return updated is null ? NotFound() : Ok(updated);
    }
}
