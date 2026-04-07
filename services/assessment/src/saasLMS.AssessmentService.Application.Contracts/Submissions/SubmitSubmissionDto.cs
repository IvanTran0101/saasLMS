using System;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Submissions;

public class SubmitSubmissionDto
{
    public Guid AssignmentId { get; set; }
    public ContentType ContentType { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public long? FileSize { get; set; }
}
