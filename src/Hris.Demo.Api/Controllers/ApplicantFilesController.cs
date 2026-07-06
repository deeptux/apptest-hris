using Hris.Demo.Api.Services;
using Hris.Demo.Shared.ApplicantFiles;
using Microsoft.AspNetCore.Mvc;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/applicants/{applicantId:guid}/files")]
public sealed class ApplicantFilesController(ApplicantProfileFilesService files) : ControllerBase
{
    [HttpPost("upload-url")]
    public ActionResult<ApplicantFileUploadUrlResponse> RequestUploadUrl(
        Guid applicantId,
        [FromBody] ApplicantFileUploadUrlRequest body)
    {
        var result = files.TryCreateUploadUrl(applicantId, body, out var error, out var code);
        if (result is null)
        {
            return StatusCode(code, new { message = error });
        }

        return Ok(result);
    }

    [HttpPost("complete")]
    public async Task<ActionResult<ApplicantFileMetadataDto>> CompleteAsync(
        Guid applicantId,
        [FromBody] ApplicantFileCompleteRequest body,
        CancellationToken cancellationToken)
    {
        var result = await files.CompleteAsync(applicantId, body, cancellationToken).ConfigureAwait(false);
        if (result.Error is not null)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ApplicantFileMetadataDto>>> ListAsync(
        Guid applicantId,
        CancellationToken cancellationToken)
    {
        var list = await files.ListAsync(applicantId, cancellationToken).ConfigureAwait(false);
        if (list is null)
        {
            return NotFound();
        }

        return Ok(list);
    }

    [HttpGet("{fileId:guid}/download-url")]
    public async Task<ActionResult<ApplicantFileDownloadUrlResponse>> DownloadUrlAsync(
        Guid applicantId,
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var result = await files.GetDownloadUrlAsync(applicantId, fileId, cancellationToken).ConfigureAwait(false);
        if (result.Error is not null)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{fileId:guid}")]
    public async Task<IActionResult> DeleteAsync(
        Guid applicantId,
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var result = await files.DeleteAsync(applicantId, fileId, cancellationToken).ConfigureAwait(false);
        if (result.Error is not null)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return NoContent();
    }
}
