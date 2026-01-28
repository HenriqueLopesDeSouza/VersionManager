using VersionManager.Domain.Common;

namespace VersionManager.Domain.Softwares;

public sealed class SoftwareVersion
{
    public Guid Id { get; private set; }
    public Guid SoftwareId { get; private set; }
    public string Version { get; private set; } = default!;
    public DateOnly ReleaseDate { get; private set; }
    public bool IsDeprecated { get; private set; }
    public DateTime? DeprecatedAt { get; private set; }

    private SoftwareVersion() { } // EF

    public SoftwareVersion(Guid softwareId, string version, DateOnly releaseDate, bool isDeprecated)
    {
        SoftwareId = softwareId;
        Version = version;
        ReleaseDate = releaseDate;
        IsDeprecated = isDeprecated;
        SetDeprecated(isDeprecated);
    }

    public void Update(string version, DateOnly releaseDate, bool isDeprecated)
    {
        version = version?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(version))
            throw new DomainException("Versão inválida.");

        Version = version;
        ReleaseDate = releaseDate;
        SetDeprecated(isDeprecated);
    }


    private void SetDeprecated(bool deprecated)
    {
        IsDeprecated = deprecated;
        DeprecatedAt = deprecated ? DateTime.UtcNow : null;
    }
}
