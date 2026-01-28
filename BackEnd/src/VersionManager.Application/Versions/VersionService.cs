using VersionManager.Application.Abstractions;
using VersionManager.Application.Common;
using VersionManager.Application.Versions.Dtos;
using VersionManager.Domain.Common;

namespace VersionManager.Application.Versions;

public sealed class VersionService : IVersionService
{
    private readonly ISoftwareRepository _repo;
    private readonly IUnitOfWork _uow;

    public VersionService(ISoftwareRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public Task<PagedResult<VersionItemDto>> ListAsync(Guid softwareId, int page, int size, CancellationToken ct)
        => _repo.ListVersionsPagedAsync(softwareId, page, size, ct);

    public Task<VersionItemDto?> GetAsync(Guid softwareId, Guid versionId, CancellationToken ct)
        => _repo.GetVersionAsync(softwareId, versionId, ct);

    public Task<VersionItemDto?> GetLatestAsync(Guid softwareId, CancellationToken ct)
        => _repo.GetLatestVersionAsync(softwareId, ct);

    public async Task<Guid> CreateAsync(Guid softwareId, CreateVersionDto dto, CancellationToken ct)
    {
        var software = await _repo.GetWithVersionsAsync(softwareId, ct);
        if (software is null) throw new DomainException("Software não encontrado.");

        var created = software.AddVersion(dto.Version, dto.ReleaseDate, dto.IsDeprecated);
        await _uow.SaveChangesAsync(ct);
        return created.Id;
    }

    public async Task<bool> PatchAsync(Guid softwareId, Guid versionId, PatchVersionDto dto, CancellationToken ct)
    {
        if (dto.Version is null && dto.ReleaseDate is null && dto.IsDeprecated is null)
            throw new DomainException("Informe ao menos um campo para atualização.");

        var software = await _repo.GetWithVersionsAsync(softwareId, ct);
        if (software is null) return false;

        var version = software.GetVersionOrThrow(versionId);

        if (dto.Version is not null)
        {
            var normalized = dto.Version.Trim();

            var exists = software.Versions.Any(v => v.Id != versionId && v.Version == normalized);
            if (exists)
                throw new DomainException("A versão já existe para este software.");
        }

        var newVersion = dto.Version ?? version.Version;
        var newReleaseDate = dto.ReleaseDate ?? version.ReleaseDate;
        var newDeprecated = dto.IsDeprecated ?? version.IsDeprecated;

        version.Update(newVersion, newReleaseDate, newDeprecated);

        await _uow.SaveChangesAsync(ct);
        return true;
    }


    public async Task<bool> UpdateAsync(Guid softwareId, Guid versionId, UpdateVersionDto dto, CancellationToken ct)
    {
        var software = await _repo.GetWithVersionsAsync(softwareId, ct);
        if (software is null) return false;

        var exists = software.Versions.Any(v => v.Id != versionId && v.Version == dto.Version.Trim());
        if (exists)
            throw new DomainException("A versão já existe para este software.");

        var version = software.GetVersionOrThrow(versionId);
        version.Update(dto.Version, dto.ReleaseDate, dto.IsDeprecated);

        await _uow.SaveChangesAsync(ct);
        return true;
    }


    public async Task<bool> DeleteAsync(Guid softwareId, Guid versionId, CancellationToken ct)
    {
        var software = await _repo.GetWithVersionsAsync(softwareId, ct);
        if (software is null) return false;

        software.RemoveVersion(versionId);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
