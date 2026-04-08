using System;
using Microsoft.AspNetCore.Http;

namespace saasLMS.AssessmentService.Submissions;

public class UploadSubmissionFileRequest
{
    public Guid SubmissionId { get; set; }
    public IFormFile? File { get; set; }
}
