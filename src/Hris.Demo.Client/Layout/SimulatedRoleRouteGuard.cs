namespace Hris.Demo.Client.Layout;

/// <summary>
/// Keeps the browser URL aligned with simulated role (Applicant vs HRIS).
/// Returns a relative navigation target, or <c>null</c> if no redirect is needed.
/// </summary>
public static class SimulatedRoleRouteGuard
{
    public static string? GetRedirectTarget(string simulatedRole, string baseRelativePath)
    {
        var rel = baseRelativePath.Trim('/').ToLowerInvariant();
        var applicantMode = string.Equals(simulatedRole, "Applicant", StringComparison.OrdinalIgnoreCase);
        var underApplicant = UnderApplicantRoutes(rel);

        if (applicantMode && !underApplicant)
            return "applicant";

        if (!applicantMode && underApplicant)
            return string.Empty;

        return null;
    }

    private static bool UnderApplicantRoutes(string relativeTrimmedLower)
    {
        if (string.Equals(relativeTrimmedLower, "applicant", StringComparison.Ordinal))
            return true;
        return relativeTrimmedLower.StartsWith("applicant/", StringComparison.Ordinal);
    }
}
