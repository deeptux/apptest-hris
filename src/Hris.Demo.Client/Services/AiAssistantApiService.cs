using System.Net.Http.Json;
using Hris.Demo.Shared.Ai;

namespace Hris.Demo.Client.Services;

public sealed class AiAssistantApiService(HttpClient http)
{
    public Task<AiQuotaStatusDto?> GetQuotaStatusAsync(CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsync<AiQuotaStatusDto>("api/Ai/quota-status", cancellationToken);

    public async Task<(JobDescriptionGenerateResponse? Data, int StatusCode, AiErrorResponse? Error)> GenerateJobDescriptionAsync(
        JobDescriptionGenerateRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await http.PostAsJsonAsync("api/Ai/job-description", request, cancellationToken).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<JobDescriptionGenerateResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return (data, (int)response.StatusCode, null);
        }

        AiErrorResponse? err = null;
        try
        {
            err = await response.Content.ReadFromJsonAsync<AiErrorResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // Non-JSON body — ignore; caller uses status only.
        }

        return (null, (int)response.StatusCode, err);
    }
}
