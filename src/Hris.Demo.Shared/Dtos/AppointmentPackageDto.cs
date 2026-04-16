namespace Hris.Demo.Shared.Dtos;

public sealed record AppointmentPackageDto(
    Guid Id,
    string ApplicantName,
    Guid SelectedApplicantId,
    string PositionTitle,
    Guid PositionItemId,
    Guid? AppointeePersonProfileId,
    string PackageStatus,
    DateOnly? Effectivity);
