namespace VersionManager.Application.Versions.Dtos;

public sealed record VersionItemDto(
    Guid Id,
    string Version,
    DateOnly ReleaseDate,
    bool IsDeprecated,
    DateTime? DeprecatedAt
);

public sealed record CreateVersionDto(
    string Version,
    DateOnly ReleaseDate,
    bool IsDeprecated
);

public sealed record UpdateVersionDto(
    string Version,
    DateOnly ReleaseDate,
    bool IsDeprecated
);

public sealed record PatchVersionDto(
    string? Version,
    DateOnly? ReleaseDate,
    bool? IsDeprecated
);
