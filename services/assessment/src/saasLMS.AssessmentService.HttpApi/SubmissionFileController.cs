using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using saasLMS.AssessmentService.Submissions;
using Volo.Abp;
using Volo.Abp.Content;

namespace saasLMS.AssessmentService;

[RemoteService(Name = AssessmentServiceRemoteServiceConsts.RemoteServiceName)]
[Area(AssessmentServiceRemoteServiceConsts.RemoteServiceName)]
[Route("api/assessment/submission")]
public class SubmissionFileController : AssessmentServiceController
{
    private readonly SubmissionAppService _submissionAppService;

    public SubmissionFileController(SubmissionAppService submissionAppService)
    {
        _submissionAppService = submissionAppService;
    }

    [HttpPost("upload-submission-file")]
    [Consumes("multipart/form-data")]
    public async Task<SubmissionDto> UploadSubmissionFileAsync([FromForm] UploadSubmissionFileRequest input)
    {
        if (input.File == null || input.File.Length <= 0)
            throw new BusinessException("Assessment:FileEmpty");

        using var stream = input.File.OpenReadStream();
        var remoteStream = new RemoteStreamContent(stream, input.File.FileName, input.File.ContentType);
        var uploadInput = new UploadSubmissionFileInput
        {
            SubmissionId = input.SubmissionId
        };
        return await _submissionAppService.UploadSubmissionFileAsync(uploadInput, remoteStream);
    }

    [HttpPost("submit-submission-file")]
    [Consumes("multipart/form-data")]
    public async Task<SubmissionDto> SubmitSubmissionFileAsync([FromForm] SubmitSubmissionFileRequest input)
    {
        if (input.File == null || input.File.Length <= 0)
            throw new BusinessException("Assessment:FileEmpty");

        using var stream = input.File.OpenReadStream();
        var remoteStream = new RemoteStreamContent(stream, input.File.FileName, input.File.ContentType);
        return await _submissionAppService.SubmitFileAsync(input.AssignmentId, remoteStream);
    }

    /// <summary>
    /// Download a submission file as Instructor.
    /// Requires Submissions.View permission (checked inside DownloadSubmissionFileAsync).
    /// </summary>
    [HttpGet("{submissionId}/download-file")]
    public async Task<IActionResult> DownloadSubmissionFileAsync([FromRoute] Guid submissionId)
    {
        var input = new DownloadSubmissionFileInput { SubmissionId = submissionId };
        var file = await _submissionAppService.DownloadSubmissionFileAsync(input);
        return File(file.GetStream(), file.ContentType ?? "application/octet-stream", file.FileName ?? "submission");
    }
}
