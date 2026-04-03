using System;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Submissions.Events;

public class SubmissionGradedDomainEvent
{
    public Guid SubmissionId { get; }
    public Guid TenantId { get; }
    public Guid AssignmentId { get; }
    public Guid StudentId { get; }
    public ContentType ContentType { get; }
    public decimal Score { get; }
    public DateTime GradedAt { get; }

    public SubmissionGradedDomainEvent(Guid submissionId, Guid tenantId, Guid assignmentId, Guid studentId, ContentType contentType, decimal score, DateTime gradedAt)
    {
        SubmissionId = submissionId;
        TenantId = tenantId;
        AssignmentId = assignmentId;
        StudentId = studentId;
        ContentType = contentType;
        Score = score;
        GradedAt = gradedAt;
    }
}