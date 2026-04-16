namespace Hris.Demo.Shared.Dtos;

public sealed record QueueActionRequestDto(string ActorRole);

public sealed record ApplicantScreeningUpdateDto(
    string ActorRole,
    string Outcome);

public sealed record AppointmentMarkAsAppointedDto(
    string ActorRole,
    DateOnly? Effectivity);
