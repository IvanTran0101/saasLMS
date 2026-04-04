using System;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Submissions.Events;

public class SubmissionCreatedDomainEvent
{
    public Guid SubmissionId { get; }
    public Guid TenantId { get; }
    public Guid AssignmentId { get; }
    public Guid StudentId { get; }
    public ContentType ContentType { get; }
    public DateTime SubmittedAt { get; }

    public SubmissionCreatedDomainEvent(Guid submissionId, Guid tenantId, Guid assignmentId,  Guid studentId,ContentType contentType, DateTime submittedAt)
    {
        SubmissionId = submissionId;
        TenantId = tenantId;
        AssignmentId = assignmentId;
        StudentId = studentId;
        ContentType = contentType;
        SubmittedAt = submittedAt;
    }
}