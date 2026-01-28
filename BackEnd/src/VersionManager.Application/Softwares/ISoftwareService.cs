using VersionManager.Application.Common;
using VersionManager.Application.Softwares.Dtos;

namespace VersionManager.Application.Softwares;

public interface ISoftwareService
{
    Task<PagedResult<SoftwareListItemDto>> ListPagedAsync(int page, int size, CancellationToken ct);

    Task<SoftwareDetailsDto?> GetAsync(Guid id, CancellationToken ct);
    Task<Guid> CreateAsync(CreateSoftwareDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(Guid id, UpdateSoftwareDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> PatchAsync(Guid id, PatchSoftwareDto dto, CancellationToken ct);

}
