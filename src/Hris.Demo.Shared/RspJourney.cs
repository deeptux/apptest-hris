namespace Hris.Demo.Shared;

public static class RspJourney
{
    public static IReadOnlyList<RspJourneyStepDto> Steps { get; } =
    [
        new(1, "manpower_signal", "Manpower planning signal", "Validated staffing need aligned with position control."),
        new(2, "manpower_request", "Manpower request & approval", "Formal requisition with routing and approvals."),
        new(3, "vacancy_definition", "Vacancy definition", "Job description, qualification standards, classification."),
        new(4, "publication", "Publication & application window", "Posted vacancy with deadlines and eligibility rules."),
        new(5, "screening", "Screening & evaluation", "Completeness checks, comparative assessment, shortlist."),
        new(6, "selection_offer", "Selection & job offer", "Deliberation, offer, and acceptance."),
        new(7, "appointment", "Appointment & onboarding", "Appointment processing, effectivity, onboarding tasks.")
    ];
}
