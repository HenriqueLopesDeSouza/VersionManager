using VersionManager.Application.Common;
using VersionManager.Application.Softwares.Dtos;
using VersionManager.Application.Versions.Dtos;
using VersionManager.Domain.Softwares;

namespace VersionManager.Application.Abstractions;

public interface ISoftwareRepository
{
    Task<Software?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Software?> GetWithVersionsAsync(Guid id, CancellationToken ct);
    Task AddAsync(Software software, CancellationToken ct);
    void Remove(Software software);

    Task<PagedResult<SoftwareListItemDto>> ListPagedAsync(int page, int size, CancellationToken ct);
    Task<PagedResult<VersionItemDto>> ListVersionsPagedAsync(Guid softwareId, int page, int size, CancellationToken ct);
    Task<VersionItemDto?> GetVersionAsync(Guid softwareId, Guid versionId, CancellationToken ct);
    Task<VersionItemDto?> GetLatestVersionAsync(Guid softwareId, CancellationToken ct);

    Task<bool> ExistsByNameAsync(string name, Guid? excludeId, CancellationToken ct);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
}
