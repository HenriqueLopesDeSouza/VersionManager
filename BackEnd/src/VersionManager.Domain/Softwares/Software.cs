using VersionManager.Domain.Common;

namespace VersionManager.Domain.Softwares;

public sealed class Software
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<SoftwareVersion> _versions = new();
    public IReadOnlyCollection<SoftwareVersion> Versions => _versions;


    private Software() { } // EF

    public Software(string name, string? description)
    {
        SetName(name);
        SetDescription(description);
    }

    public void Update(string name, string? description)
    {
        SetName(name);
        SetDescription(description);
    }

    public void Patch(string? name, string? description)
    {
        if (name is not null)
            SetName(name);

        if (description is not null)
            SetDescription(description);
    }


    public void RemoveVersion(Guid versionId)
    {
        var version = GetVersionOrThrow(versionId);
        _versions.Remove(version);
    }


    public SoftwareVersion AddVersion(string version, DateOnly releaseDate, bool isDeprecated)
    {
        version = Normalize(version);

        if (_versions.Any(v => v.Version == version))
            throw new DomainException("A versão já existe para este software.");

        var entity = new SoftwareVersion(Id, version, releaseDate, isDeprecated);
        _versions.Add(entity);
        return entity;
    }

    public SoftwareVersion GetVersionOrThrow(Guid versionId)
        => _versions.FirstOrDefault(v => v.Id == versionId)
           ?? throw new DomainException("Versão não encontrada para este software.");

    private void SetName(string name)
    {
        name = Normalize(name);
        if (name.Length < 2) throw new DomainException("Nome do software inválido.");
        Name = name;
    }

    private void SetDescription(string? description)
        => Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

    private static string Normalize(string value)
        => string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
}
