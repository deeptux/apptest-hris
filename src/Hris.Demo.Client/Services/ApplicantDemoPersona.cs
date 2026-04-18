namespace Hris.Demo.Client.Services;

/// <summary>
/// Pinned demo applicant for Applicant mode (v1). Seed row lives in
/// <c>MockRspStore</c> in the API project — keep this id aligned with that seed.
/// </summary>
public static class ApplicantDemoPersona
{
    public static readonly Guid ApplicantId = Guid.Parse("c3000000-0000-4000-8000-000000000001");
}
