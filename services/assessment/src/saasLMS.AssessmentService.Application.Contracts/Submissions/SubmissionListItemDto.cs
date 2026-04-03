using System;
using saasLMS.AssessmentService.Shared;
using saasLMS.AssessmentService.Submissions;

namespace saasLMS.AssessmentService.Submissions;

public class SubmissionListItemDto
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public ContentType ContentType { get; set; }
    public string? FileName { get; set; }
    public SubmissionStatus Status { get; set; }
    public decimal? Score { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? GradedAt { get; set; }
}