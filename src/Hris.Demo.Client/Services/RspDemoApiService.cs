using System.Net.Http.Json;
using Hris.Demo.Shared;
using Hris.Demo.Shared.Dtos;

namespace Hris.Demo.Client.Services;

public sealed class RspDemoApiService(HttpClient http)
{
    public Task<IReadOnlyList<RspJourneyStepDto>?> GetJourneyStepsAsync(CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<RspJourneyStepDto>>("api/RspJourney/steps", cancellationToken);

    public Task<IReadOnlyList<ManpowerRequestDto>?> GetManpowerRequestsAsync(CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<ManpowerRequestDto>>("api/ManpowerRequests", cancellationToken);

    public Task<IReadOnlyList<VacancyDto>?> GetVacanciesAsync(CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<VacancyDto>>("api/Vacancies", cancellationToken);

    public Task<IReadOnlyList<ApplicantDto>?> GetApplicantsAsync(CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<ApplicantDto>>("api/Applicants", cancellationToken);

    public Task<IReadOnlyList<AppointmentPackageDto>?> GetAppointmentsAsync(CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<AppointmentPackageDto>>("api/Appointments", cancellationToken);

    public Task<IReadOnlyList<AuditEventDto>?> GetAuditEventsAsync(CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<AuditEventDto>>("api/AuditEvents", cancellationToken);

    public Task<IReadOnlyList<OrganizationUnitDto>?> GetOrganizationUnitsAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<OrganizationUnitDto>>($"api/OrganizationUnits?includeInactive={includeInactive}", cancellationToken);

    public async Task<OrganizationUnitDto?> CreateOrganizationUnitAsync(OrganizationUnitUpsertDto model, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("api/OrganizationUnits", model, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<OrganizationUnitDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<OrganizationUnitDto?> UpdateOrganizationUnitAsync(Guid id, OrganizationUnitUpsertDto model, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsJsonAsync($"api/OrganizationUnits/{id}", model, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<OrganizationUnitDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<OrganizationUnitDto?> DeactivateOrganizationUnitAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"api/OrganizationUnits/{id}/deactivate", new { }, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<OrganizationUnitDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public Task<IReadOnlyList<QualificationStandardRefDto>?> GetQualificationStandardsAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<QualificationStandardRefDto>>($"api/QualificationStandards?includeInactive={includeInactive}", cancellationToken);

    public async Task<QualificationStandardRefDto?> CreateQualificationStandardAsync(QualificationStandardRefUpsertDto model, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("api/QualificationStandards", model, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<QualificationStandardRefDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<QualificationStandardRefDto?> UpdateQualificationStandardAsync(Guid id, QualificationStandardRefUpsertDto model, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsJsonAsync($"api/QualificationStandards/{id}", model, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<QualificationStandardRefDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<QualificationStandardRefDto?> DeactivateQualificationStandardAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"api/QualificationStandards/{id}/deactivate", new { }, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<QualificationStandardRefDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public Task<IReadOnlyList<PositionItemDto>?> GetPositionItemsAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<PositionItemDto>>($"api/PositionItems?includeInactive={includeInactive}", cancellationToken);

    public async Task<PositionItemDto?> CreatePositionItemAsync(PositionItemUpsertDto model, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("api/PositionItems", model, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<PositionItemDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PositionItemDto?> UpdatePositionItemAsync(Guid id, PositionItemUpsertDto model, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsJsonAsync($"api/PositionItems/{id}", model, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<PositionItemDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PositionItemDto?> DeactivatePositionItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"api/PositionItems/{id}/deactivate", new { }, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<PositionItemDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public Task<IReadOnlyList<PersonProfileDto>?> GetPersonProfilesAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<IReadOnlyList<PersonProfileDto>>($"api/PersonProfiles?includeInactive={includeInactive}", cancellationToken);

    public async Task<PersonProfileDto?> CreatePersonProfileAsync(PersonProfileUpsertDto model, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("api/PersonProfiles", model, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<PersonProfileDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PersonProfileDto?> UpdatePersonProfileAsync(Guid id, PersonProfileUpsertDto model, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsJsonAsync($"api/PersonProfiles/{id}", model, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<PersonProfileDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PersonProfileDto?> DeactivatePersonProfileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"api/PersonProfiles/{id}/deactivate", new { }, cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<PersonProfileDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ManpowerRequestDto?> ToggleManpowerSubmitApproveAsync(Guid id, string actorRole, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"api/ManpowerRequests/{id}/toggle-submit-approve", new QueueActionRequestDto(actorRole), cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<ManpowerRequestDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<VacancyDto?> ToggleVacancyPublishAsync(Guid id, string actorRole, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"api/Vacancies/{id}/toggle-publish", new QueueActionRequestDto(actorRole), cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<VacancyDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ApplicantDto?> SetApplicantOutcomeAsync(Guid id, string actorRole, string outcome, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"api/Applicants/{id}/screening-outcome", new ApplicantScreeningUpdateDto(actorRole, outcome), cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<ApplicantDto>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AppointmentPackageDto?> MarkAppointmentAsAppointedAsync(Guid id, string actorRole, DateOnly? effectivity, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"api/Appointments/{id}/mark-appointed", new AppointmentMarkAsAppointedDto(actorRole, effectivity), cancellationToken).ConfigureAwait(false);
        return await ReadOrDefaultAsync<AppointmentPackageDto>(response, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<T?> ReadOrDefaultAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
