using Hris.Demo.Api.Services;
using Hris.Demo.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PositionItemsController(MockRspStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<PositionItemDto>> GetAll([FromQuery] bool includeInactive = false) =>
        Ok(store.GetPositionItems(includeInactive));

    [HttpPost]
    public ActionResult<PositionItemDto> Create([FromBody] PositionItemUpsertDto body)
    {
        var created = store.CreatePositionItem(body);
        return Ok(created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<PositionItemDto> Update(Guid id, [FromBody] PositionItemUpsertDto body)
    {
        var updated = store.UpdatePositionItem(id, body);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/deactivate")]
    public ActionResult<PositionItemDto> Deactivate(Guid id)
    {
        var updated = store.DeactivatePositionItem(id);
        return updated is null ? NotFound() : Ok(updated);
    }
}
