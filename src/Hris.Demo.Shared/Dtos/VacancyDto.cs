namespace Hris.Demo.Shared.Dtos;

public sealed record VacancyDto(
    Guid Id,
    string Title,
    string SalaryGrade,
    Guid PositionItemId,
    DateOnly OpenFrom,
    DateOnly OpenUntil,
    string PublicationStatus);
