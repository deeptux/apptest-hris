using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hris.Demo.Shared;
using Hris.Demo.Shared.ApplicantFiles;
using Hris.Demo.Shared.Dtos;

namespace Hris.Demo.Client.Services;

public sealed class RspDemoApiService(HttpClient http)
{
    private static readonly JsonSerializerOptions ApplicantFileJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };
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

    public async Task<(ApplicantFileUploadUrlResponse? Body, string? Error)> RequestApplicantFileUploadUrlAsync(
        Guid applicantId,
        ApplicantFileUploadUrlRequest body,
        CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"api/applicants/{applicantId}/files/upload-url");
        req.Content = JsonContent.Create(body, options: ApplicantFileJson);
        var response = await http.SendAsync(req, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return (null, await ReadApiErrorAsync(response, cancellationToken).ConfigureAwait(false));
        }

        var parsed = await response.Content.ReadFromJsonAsync<ApplicantFileUploadUrlResponse>(ApplicantFileJson, cancellationToken)
            .ConfigureAwait(false);
        return (parsed, null);
    }

    public async Task<(ApplicantFileMetadataDto? Body, string? Error)> CompleteApplicantFileUploadAsync(
        Guid applicantId,
        ApplicantFileCompleteRequest body,
        CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"api/applicants/{applicantId}/files/complete");
        req.Content = JsonContent.Create(body, options: ApplicantFileJson);
        var response = await http.SendAsync(req, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return (null, await ReadApiErrorAsync(response, cancellationToken).ConfigureAwait(false));
        }

        var parsed = await response.Content.ReadFromJsonAsync<ApplicantFileMetadataDto>(ApplicantFileJson, cancellationToken)
            .ConfigureAwait(false);
        return (parsed, null);
    }

    public async Task<(IReadOnlyList<ApplicantFileMetadataDto>? Body, string? Error)> GetApplicantFilesAsync(
        Guid applicantId,
        CancellationToken cancellationToken = default)
    {
        var response = await http.GetAsync($"api/applicants/{applicantId}/files", cancellationToken).ConfigureAwait(false);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return (null, "Applicant not found.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return (null, await ReadApiErrorAsync(response, cancellationToken).ConfigureAwait(false));
        }

        var parsed = await response.Content.ReadFromJsonAsync<List<ApplicantFileMetadataDto>>(ApplicantFileJson, cancellationToken)
            .ConfigureAwait(false);
        return (parsed, null);
    }

    public async Task<(ApplicantFileDownloadUrlResponse? Body, string? Error)> GetApplicantFileDownloadUrlAsync(
        Guid applicantId,
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        var response = await http.GetAsync($"api/applicants/{applicantId}/files/{fileId}/download-url", cancellationToken)
            .ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return (null, await ReadApiErrorAsync(response, cancellationToken).ConfigureAwait(false));
        }

        var parsed = await response.Content.ReadFromJsonAsync<ApplicantFileDownloadUrlResponse>(ApplicantFileJson, cancellationToken)
            .ConfigureAwait(false);
        return (parsed, null);
    }

    public async Task<(bool Ok, string? Error)> DeleteApplicantFileAsync(
        Guid applicantId,
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        var response = await http.DeleteAsync($"api/applicants/{applicantId}/files/{fileId}", cancellationToken)
            .ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            return (true, null);
        }

        return (false, await ReadApiErrorAsync(response, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>PUT bytes to S3 (or compatible) using a pre-signed URL; uses a separate <see cref="HttpClient"/> so API base address does not apply.</summary>
    public static async Task<string?> PutToPresignedUrlAsync(
        string uploadUrl,
        byte[] body,
        IReadOnlyDictionary<string, string> requiredHeaders,
        CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var content = new ByteArrayContent(body);
        foreach (var pair in requiredHeaders)
        {
            if (string.Equals(pair.Key, "Content-Type", StringComparison.OrdinalIgnoreCase))
            {
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(pair.Value);
            }
            else
            {
                content.Headers.TryAddWithoutValidation(pair.Key, pair.Value);
            }
        }

        var response = await client.PutAsync(uploadUrl, content, cancellationToken).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            return null;
        }

        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return $"Direct upload failed ({(int)response.StatusCode}): {text}";
    }

    private static async Task<string?> ReadApiErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (doc.RootElement.TryGetProperty("message", out var m))
            {
                return m.GetString() ?? response.ReasonPhrase;
            }
        }
        catch
        {
            /* fall through */
        }

        return response.ReasonPhrase ?? $"HTTP {(int)response.StatusCode}";
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
