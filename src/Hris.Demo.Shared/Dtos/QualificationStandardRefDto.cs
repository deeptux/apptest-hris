namespace Hris.Demo.Shared.Dtos;

public sealed record QualificationStandardRefDto(
    Guid Id,
    string Code,
    string PositionTitle,
    string Description,
    string Education,
    string Training,
    string Experience,
    string Eligibility,
    bool IsActive,
    DateTimeOffset? DeactivatedAt);
