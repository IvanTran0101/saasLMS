using System;

namespace saasLMS.AssessmentService.Assignments.Events;

public class AssignmentUpdatedDomainEvent
{
    public Guid AssignmentId { get; }
    public Guid TenantId { get; }
    public Guid CourseId { get; }
    public Guid LessonId { get; }
    public string Title { get; }
    public DateTime? Deadline { get; }
    public decimal MaxScore { get; }

    public AssignmentUpdatedDomainEvent(Guid assignmentId, Guid tenantId, Guid courseId, Guid lessonId, string title, DateTime? deadline, decimal maxScore)
    {
        AssignmentId = assignmentId;
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        Title = title;
        Deadline = deadline;
        MaxScore = maxScore;
    }
}