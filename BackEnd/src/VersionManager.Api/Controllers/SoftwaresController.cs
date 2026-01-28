using Microsoft.AspNetCore.Mvc;
using VersionManager.Application.Softwares;
using VersionManager.Application.Softwares.Dtos;

namespace VersionManager.Api.Controllers;

[ApiController]
[Route("api/softwares")]
public sealed class SoftwaresController : ControllerBase
{
    private readonly ISoftwareService _service;

    public SoftwaresController(ISoftwareService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
        => Ok(await _service.ListPagedAsync(page, size, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await _service.GetAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSoftwareDto dto, CancellationToken ct)
    {
        var id = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSoftwareDto dto, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, dto, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] PatchSoftwareDto dto, CancellationToken ct)
    {
        var updated = await _service.PatchAsync(id, dto, ct);
        return updated ? NoContent() : NotFound();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
