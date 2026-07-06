using System.Text.Json.Serialization;

namespace Hris.Demo.Shared.ApplicantFiles;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApplicantFileCategory
{
    Avatar = 0,
    Cover = 1,
    ResumePdf = 2,
    CvPdf = 3
}
