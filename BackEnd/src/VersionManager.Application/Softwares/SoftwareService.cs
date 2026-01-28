using System.Xml.Linq;
using VersionManager.Application.Abstractions;
using VersionManager.Application.Common;
using VersionManager.Application.Softwares.Dtos;
using VersionManager.Domain.Common;
using VersionManager.Domain.Softwares;

namespace VersionManager.Application.Softwares;

public sealed class SoftwareService : ISoftwareService
{
    private readonly ISoftwareRepository _repo;
    private readonly IUnitOfWork _uow;

    public SoftwareService(ISoftwareRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<IReadOnlyList<SoftwareListItemDto>> ListAsync(CancellationToken ct)
        => (await _repo.ListPagedAsync(1, 50, ct)).Items;

    public Task<PagedResult<SoftwareListItemDto>> ListPagedAsync(int page, int size, CancellationToken ct)
        => _repo.ListPagedAsync(page, size, ct);

    public async Task<SoftwareDetailsDto?> GetAsync(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null
            ? null
            : new SoftwareDetailsDto(entity.Id, entity.Name, entity.Description, entity.CreatedAt);
    }

    public async Task<Guid> CreateAsync(CreateSoftwareDto dto, CancellationToken ct)
    {
        var name = (dto.Name ?? "").Trim();

        if (await _repo.ExistsByNameAsync(name, excludeId: null, ct))
            throw new DomainException("Já existe um software com este nome.");

        var entity = new Software(name, dto.Description);
        await _repo.AddAsync(entity, ct);

        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<bool> PatchAsync(Guid id, PatchSoftwareDto dto, CancellationToken ct)
    {
        if (dto.Name is null && dto.Description is null)
            throw new DomainException("Informe ao menos um campo para atualização.");

        var software = await _repo.GetByIdAsync(id, ct);
        if (software is null) return false;

        if (dto.Name is not null)
        {
            var name = dto.Name.Trim();

            if (await _repo.ExistsByNameAsync(name, excludeId: id, ct))
                throw new DomainException("Já existe um software com este nome.");
        }

        software.Patch(dto.Name, dto.Description);

        await _uow.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> UpdateAsync(Guid id, UpdateSoftwareDto dto, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return false;

        var name = dto.Name.Trim();

        if (await _repo.ExistsByNameAsync(name, excludeId: id, ct))
            throw new DomainException("Já existe um software com este nome.");

        entity.Update(dto.Name, dto.Description);

        await _uow.SaveChangesAsync(ct);
        return true;
    }


    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return false;

        _repo.Remove(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
