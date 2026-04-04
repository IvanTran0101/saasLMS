using System;

namespace saasLMS.AssessmentService.Assignments.Events;

public class AssignmentClosedDomainEvent
{
    public Guid AssignmentId { get; }
    public Guid TenantId { get; }
    public Guid CourseId { get; }
    public Guid LessonId { get; }
    public DateTime ClosedAt { get; }

    public AssignmentClosedDomainEvent(Guid assignmentId, Guid tenantId, Guid courseId, Guid lessonId, DateTime closedAt)
    {
        AssignmentId = assignmentId;
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        ClosedAt = closedAt;
    }
}