using Microsoft.EntityFrameworkCore;
using VersionManager.Application.Abstractions;
using VersionManager.Application.Common;
using VersionManager.Application.Softwares.Dtos;
using VersionManager.Application.Versions.Dtos;
using VersionManager.Domain.Softwares;
using VersionManager.Infrastructure.Persistence;

namespace VersionManager.Infrastructure.Repositories;

public sealed class SoftwareRepository : ISoftwareRepository
{
    private readonly AppDbContext _db;

    public SoftwareRepository(AppDbContext db) => _db = db;

    public Task AddAsync(Software software, CancellationToken ct)
        => _db.Softwares.AddAsync(software, ct).AsTask();

    public Task<Software?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Softwares.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Software?> GetWithVersionsAsync(Guid id, CancellationToken ct)
        => _db.Softwares.Include(x => x.Versions).FirstOrDefaultAsync(x => x.Id == id, ct);

    public void Remove(Software software) => _db.Softwares.Remove(software);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        => _db.Softwares.AsNoTracking().AnyAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeId, CancellationToken ct)
    {
        name = name.Trim();

        return _db.Softwares.AnyAsync(s => s.Name == name && (excludeId == null || s.Id != excludeId.Value), ct);
    }



    public async Task<PagedResult<SoftwareListItemDto>> ListPagedAsync(int page, int size, CancellationToken ct)
    {
        page = ClampPage(page);
        size = ClampSize(size);

        var query = _db.Softwares.AsNoTracking().OrderBy(x => x.Name);

        var total = await query.LongCountAsync(ct);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new SoftwareListItemDto(x.Id, x.Name, x.Description, x.CreatedAt))
            .ToListAsync(ct);

        return new PagedResult<SoftwareListItemDto>(items, page, size, total);
    }

    public async Task<PagedResult<VersionItemDto>> ListVersionsPagedAsync(Guid softwareId, int page, int size, CancellationToken ct)
    {
        page = ClampPage(page);
        size = ClampSize(size);

        var query = _db.SoftwareVersions
            .AsNoTracking()
            .Where(v => v.SoftwareId == softwareId)
            .OrderByDescending(v => v.ReleaseDate)
            .ThenByDescending(v => v.Version);

        var total = await query.LongCountAsync(ct);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(v => new VersionItemDto(v.Id, v.Version, v.ReleaseDate, v.IsDeprecated, v.DeprecatedAt))
            .ToListAsync(ct);

        return new PagedResult<VersionItemDto>(items, page, size, total);
    }

    public Task<VersionItemDto?> GetVersionAsync(Guid softwareId, Guid versionId, CancellationToken ct)
        => _db.SoftwareVersions
            .AsNoTracking()
            .Where(v => v.SoftwareId == softwareId && v.Id == versionId)
            .Select(v => new VersionItemDto(v.Id, v.Version, v.ReleaseDate, v.IsDeprecated, v.DeprecatedAt))
            .FirstOrDefaultAsync(ct);

    public Task<VersionItemDto?> GetLatestVersionAsync(Guid softwareId, CancellationToken ct)
        => _db.SoftwareVersions
            .AsNoTracking()
            .Where(v => v.SoftwareId == softwareId)
            .OrderByDescending(v => v.ReleaseDate)
            .ThenByDescending(v => v.Version)
            .Select(v => new VersionItemDto(v.Id, v.Version, v.ReleaseDate, v.IsDeprecated, v.DeprecatedAt))
            .FirstOrDefaultAsync(ct);

    private static int ClampPage(int page) => page < 1 ? 1 : page;
    private static int ClampSize(int size) => size switch
    {
        < 1 => 10,
        > 100 => 100,
        _ => size
    };
}
