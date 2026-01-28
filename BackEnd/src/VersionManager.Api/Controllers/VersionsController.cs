using Microsoft.AspNetCore.Mvc;
using VersionManager.Application.Versions;
using VersionManager.Application.Versions.Dtos;

namespace VersionManager.Api.Controllers;

[ApiController]
[Route("api/softwares/{softwareId:guid}/versions")]
public sealed class VersionsController : ControllerBase
{
    private readonly IVersionService _service;

    public VersionsController(IVersionService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List(Guid softwareId, [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
        => Ok(await _service.ListAsync(softwareId, page, size, ct));

    [HttpGet("latest")]
    public async Task<IActionResult> Latest(Guid softwareId, CancellationToken ct)
    {
        var item = await _service.GetLatestAsync(softwareId, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("{versionId:guid}")]
    public async Task<IActionResult> Get(Guid softwareId, Guid versionId, CancellationToken ct)
    {
        var item = await _service.GetAsync(softwareId, versionId, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid softwareId, [FromBody] CreateVersionDto dto, CancellationToken ct)
    {
        var id = await _service.CreateAsync(softwareId, dto, ct);
        return CreatedAtAction(nameof(Get), new { softwareId, versionId = id }, new { id });
    }

    [HttpPut("{versionId:guid}")]
    public async Task<IActionResult> Update(Guid softwareId, Guid versionId, [FromBody] UpdateVersionDto dto, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(softwareId, versionId, dto, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPatch("{versionId:guid}")]
    public async Task<IActionResult> Patch(Guid softwareId, Guid versionId, [FromBody] PatchVersionDto dto, CancellationToken ct)
    {
        await _service.PatchAsync(softwareId, versionId, dto, ct);
        return NoContent();
    }


    [HttpDelete("{versionId:guid}")]
    public async Task<IActionResult> Delete(Guid softwareId, Guid versionId, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(softwareId, versionId, ct);
        return ok ? NoContent() : NotFound();
    }

}
