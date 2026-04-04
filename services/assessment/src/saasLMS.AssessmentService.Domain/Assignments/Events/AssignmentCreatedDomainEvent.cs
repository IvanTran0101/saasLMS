using System;

namespace saasLMS.AssessmentService.Assignments.Events;

public class AssignmentCreatedDomainEvent
{
    public Guid AssignmentId { get; }
    public Guid TenantId { get; }
    public Guid CourseId { get; }
    public Guid LessonId { get; }
    public string Title { get; }
    public DateTime? Deadline { get; }
    public decimal MaxScore { get; }
    public DateTime CreatedAt { get; }

    public AssignmentCreatedDomainEvent(Guid assignmentId, Guid tenantId, Guid courseId, Guid lessonId, string title, DateTime? deadline, decimal maxScore, DateTime createdAt)
    {
        AssignmentId = assignmentId;
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        Title = title;
        Deadline = deadline;
        MaxScore = maxScore;
        CreatedAt = createdAt;
    }
}
