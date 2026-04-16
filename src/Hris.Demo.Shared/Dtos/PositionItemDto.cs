namespace Hris.Demo.Shared.Dtos;

public sealed record PositionItemDto(
    Guid Id,
    string ItemNumber,
    string PlantillaNumber,
    string Title,
    string SalaryGrade,
    Guid OrganizationUnitId,
    Guid QualificationStandardRefId,
    bool IsActive,
    DateTimeOffset? DeactivatedAt);
