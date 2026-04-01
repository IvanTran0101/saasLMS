using System;

namespace saasLMS.AssessmentService.Quizzes.Events;

public class QuizPublishedDomainEvent
{
    public Guid QuizId { get; }
    public Guid TenantId { get; }
    public Guid CourseId { get; }
    public Guid LessonId { get; }
    public DateTime PublishedAt { get; }

    public QuizPublishedDomainEvent(Guid quizId, Guid tenantId, Guid courseId, Guid lessonId, DateTime publishedAt)
    {
        QuizId = quizId;
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        PublishedAt = publishedAt;
    }
}