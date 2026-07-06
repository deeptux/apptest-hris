namespace Hris.Demo.Api.Services;

public static class ApplicantFileNameSanitizer
{
    public static string Sanitize(string? originalName)
    {
        var name = Path.GetFileName(originalName ?? "file");
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "file";
        }

        const int maxLen = 120;
        return name.Length <= maxLen ? name : name[..maxLen];
    }
}
