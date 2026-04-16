namespace Hris.Demo.Shared.Dtos;

public sealed record ManpowerRequestDto(
    Guid Id,
    string ReferenceCode,
    string Department,
    string PositionTitle,
    Guid OrganizationUnitId,
    Guid PositionItemId,
    string Status,
    DateOnly RequestedOn);
