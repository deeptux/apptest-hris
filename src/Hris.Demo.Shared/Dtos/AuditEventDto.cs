namespace Hris.Demo.Shared.Dtos;

public sealed record AuditEventDto(
    Guid Id,
    DateTimeOffset OccurredAt,
    string ActorRole,
    string Action,
    string EntityType,
    Guid EntityId,
    string EntitySummary);
