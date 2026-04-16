using Hris.Demo.Shared.Ai;

namespace Hris.Demo.Api.Services;

public interface IJobDescriptionGenerator
{
    Task<string> GenerateAsync(JobDescriptionGenerateRequest request, string prompt, CancellationToken cancellationToken = default);
}
