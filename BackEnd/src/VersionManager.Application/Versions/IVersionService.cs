using VersionManager.Application.Common;
using VersionManager.Application.Versions.Dtos;

namespace VersionManager.Application.Versions;

public interface IVersionService
{
    Task<PagedResult<VersionItemDto>> ListAsync(Guid softwareId, int page, int size, CancellationToken ct);
    Task<VersionItemDto?> GetAsync(Guid softwareId, Guid versionId, CancellationToken ct);
    Task<VersionItemDto?> GetLatestAsync(Guid softwareId, CancellationToken ct);

    Task<Guid> CreateAsync(Guid softwareId, CreateVersionDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(Guid softwareId, Guid versionId, UpdateVersionDto dto, CancellationToken ct);
    Task<bool> PatchAsync(Guid softwareId, Guid versionId, PatchVersionDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(Guid softwareId, Guid versionId, CancellationToken ct);
}
