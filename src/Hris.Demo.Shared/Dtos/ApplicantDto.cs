namespace Hris.Demo.Shared.Dtos;

public sealed record ApplicantDto(
    Guid Id,
    string FullName,
    string AppliedFor,
    Guid? PersonProfileId,
    string EligibilityFlag,
    int ScreenScore);
