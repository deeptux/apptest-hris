namespace Hris.Demo.Shared.Dtos;

public sealed record PersonProfileDto(
    Guid Id,
    string FullName,
    string? EmailAddress,
    string? EmployeeNumber,
    bool IsEmployee,
    bool IsActive,
    DateTimeOffset? DeactivatedAt);
