namespace Hris.Demo.Shared.Dtos;

public sealed record OrganizationUnitDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTimeOffset? DeactivatedAt);
