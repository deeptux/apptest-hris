using System.Text;
using Hris.Demo.Shared.Ai;

namespace Hris.Demo.Api.Services;

public static class JobDescriptionPromptBuilder
{
    public static string Build(JobDescriptionGenerateRequest request)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are drafting a formal job description for HR records.");
        sb.AppendLine();
        sb.AppendLine("OUTPUT RULES (critical):");
        sb.AppendLine("- Use plain text only. Do not use markdown (no **, #, or __). Do not output only headings or labels.");
        sb.AppendLine("- You MUST write a complete draft in one response. Do not stop after the job title or section titles.");
        sb.AppendLine("- Minimum length: about 350–600 words total, with real sentences and bullets where listed below.");
        sb.AppendLine("- Include ALL of the following sections with substantive content under each:");
        sb.AppendLine("  1) Summary — at least one paragraph (roughly 80–120 words) describing the role and its purpose.");
        sb.AppendLine("  2) Key Duties — at least 4 bullet lines, each starting with \"- \" and describing a concrete duty.");
        sb.AppendLine("  3) Minimum Qualifications — bullet list of education, experience, and skills.");
        sb.AppendLine("  4) Competencies — bullet list of behaviors or skills needed for success.");
        sb.AppendLine("- Do not invent specific salary, union contract terms, or legal guarantees.");
        sb.AppendLine();
        sb.Append("Job position title: ").AppendLine(request.PositionTitle);
        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            sb.Append("Department (context): ").AppendLine(request.Department);
        }

        if (!string.IsNullOrWhiteSpace(request.EmploymentType))
        {
            sb.Append("Employment type (context): ").AppendLine(request.EmploymentType);
        }

        sb.Append("Language: ").AppendLine(string.IsNullOrWhiteSpace(request.Language) ? "en" : request.Language);
        sb.AppendLine();
        sb.AppendLine("Write the complete job description now, following every rule above:");
        return sb.ToString();
    }
}
