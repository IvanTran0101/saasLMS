using System;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Submissions.Events;

public class SubmissionUpdatedDomainEvent
{
    public Guid SubmissionId { get; }
    public Guid TenantId { get; }
    public Guid AssignmentId { get; }
    public Guid StudentId { get; }
    public ContentType ContentType { get; }
    public DateTime UpdatedAt { get; }

    public SubmissionUpdatedDomainEvent(Guid submissionId, Guid tenantId, Guid assignmentId, Guid studentId, ContentType contentType, DateTime updatedAt)
    {
        SubmissionId = submissionId;
        TenantId = tenantId;
        AssignmentId = assignmentId;
        StudentId = studentId;
        ContentType = contentType;
        UpdatedAt = updatedAt;
    }
}