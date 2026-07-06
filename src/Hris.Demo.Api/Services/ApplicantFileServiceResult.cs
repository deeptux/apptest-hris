using Microsoft.AspNetCore.Http;

namespace Hris.Demo.Api.Services;

public sealed record ApplicantFileServiceResult<T>(T? Value, string? Error, int StatusCode)
{
    public static ApplicantFileServiceResult<T> Ok(T value, int statusCode = StatusCodes.Status200OK) =>
        new(value, null, statusCode);

    public static ApplicantFileServiceResult<T> Fail(string message, int statusCode) =>
        new(default, message, statusCode);
}
