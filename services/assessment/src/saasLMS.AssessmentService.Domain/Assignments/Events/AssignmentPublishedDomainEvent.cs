using System;

namespace saasLMS.AssessmentService.Assignments.Events;

public class AssignmentPublishedDomainEvent
{
    public Guid AssignmentId { get; }
    public Guid TenantId { get; }
    public Guid CourseId { get; }
    public Guid LessonId { get; }
    public DateTime PublishedAt { get; }

    public AssignmentPublishedDomainEvent(Guid assignmentId, Guid tenantId, Guid courseId, Guid lessonId, DateTime publishedAt)
    {
        AssignmentId = assignmentId;
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        PublishedAt = publishedAt;
    }
}