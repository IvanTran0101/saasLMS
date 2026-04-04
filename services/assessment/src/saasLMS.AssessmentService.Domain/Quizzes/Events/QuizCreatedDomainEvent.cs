using System;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Quizzes.Events;

public class QuizCreatedDomainEvent
{
    public Guid QuizId { get; }
    public Guid TenantId { get; }
    public Guid CourseId { get; }
    public Guid LessonId { get; }
    public string Title { get; }
    public int? TimeLimitMinutes { get; }
    public decimal MaxScore { get; }
    public AttemptPolicy AttemptPolicy { get; }
    public DateTime CreatedAt { get; }

    public QuizCreatedDomainEvent(
        Guid quizId,
        Guid tenantId,
        Guid courseId,
        Guid lessonId,
        string title,
        int? timeLimitMinutes,
        decimal maxScore,
        AttemptPolicy attemptPolicy,
        DateTime createdAt)
    {
        QuizId = quizId;
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        Title = title;
        TimeLimitMinutes = timeLimitMinutes;
        MaxScore = maxScore;
        AttemptPolicy = attemptPolicy;
        CreatedAt = createdAt;
    }
}