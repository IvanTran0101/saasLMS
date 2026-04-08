using System;
using Microsoft.AspNetCore.Http;

namespace saasLMS.AssessmentService.Submissions;

public class SubmitSubmissionFileRequest
{
    public Guid AssignmentId { get; set; }
    public IFormFile? File { get; set; }
}
