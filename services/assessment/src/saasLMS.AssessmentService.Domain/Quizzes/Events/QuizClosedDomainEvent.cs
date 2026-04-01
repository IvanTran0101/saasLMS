using System;

namespace saasLMS.AssessmentService.Quizzes.Events;

public class QuizClosedDomainEvent
{
    public Guid QuizId { get; }
    public Guid TenantId { get; }
    public Guid CourseId { get; }
    public Guid LessonId { get; }
    public DateTime ClosedAt { get; }

    public QuizClosedDomainEvent(Guid quizId, Guid tenantId, Guid courseId, Guid lessonId, DateTime closedAt)
    {
        QuizId = quizId;
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        ClosedAt = closedAt;
    }
}