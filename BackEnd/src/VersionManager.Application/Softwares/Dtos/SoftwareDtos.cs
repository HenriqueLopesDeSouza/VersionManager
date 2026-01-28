namespace VersionManager.Application.Softwares.Dtos;

public sealed record SoftwareListItemDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt
);

public sealed record SoftwareDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt
);

public sealed record CreateSoftwareDto(
    string Name,
    string? Description
);

public sealed record UpdateSoftwareDto(
    string Name,
    string? Description
);

public sealed record PatchSoftwareDto(
    string? Name,
    string? Description
);