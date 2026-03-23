using System;
using saasLMS.AssessmentService.Shared;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.AssessmentService.Quizzes;

public class Quiz : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid CourseId { get; protected set; }
    public Guid LessonId { get; protected set; }
    public string Title { get; protected set; }
    public int? TimeLimitMinutes { get; protected set; }
    public decimal MaxScore { get; protected set; }
    public AttemptPolicy AttemptPolicy { get; protected set; }
    public string QuestionsJson { get; protected set; }
    public QuizStatus Status { get; protected set; }
    public DateTime? PublishedAt { get; protected set; }
    public DateTime? ClosedAt { get; protected set; }
    
    protected Quiz()
    {
    }

    public Quiz(Guid id, Guid tenantId, Guid courseId, Guid lessonId, string title, int? timeLimitMinutes, decimal maxScore, AttemptPolicy attemptPolicy, string questionJson) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("The tenant id cannot be empty.", nameof(tenantId));
        }

        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("The course id cannot be empty.", nameof(courseId));
        }

        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("The lesson id cannot be empty.", nameof(lessonId));
        }

        if (timeLimitMinutes.HasValue && timeLimitMinutes.Value <= 0)        {
            throw new ArgumentException("The timeLimitMinutes must be greater than zero.", nameof(timeLimitMinutes));
        }

        if (maxScore <= 0)
        {
            throw new ArgumentException("The maxScore must be greater than zero.", nameof(maxScore));
        }
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        TimeLimitMinutes = timeLimitMinutes;
        MaxScore = maxScore;
        AttemptPolicy = attemptPolicy;
        QuestionsJson = Check.NotNullOrWhiteSpace(questionJson, nameof(questionJson));
        Status = QuizStatus.Draft;
    }

    public void UpdateInfo(
        string title,
        int? timeLimitMinutes,
        decimal maxScore,
        AttemptPolicy attemptPolicy,
        string questionsJson)
    {
        if (timeLimitMinutes.HasValue && timeLimitMinutes.Value <= 0)        {
            throw new ArgumentException("The timeLimitMinutes must be greater than zero.", nameof(timeLimitMinutes));
        }

        if (maxScore <= 0)
        {
            throw new ArgumentException("The maxScore must be greater than zero.", nameof(maxScore));
        }
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        TimeLimitMinutes = timeLimitMinutes;
        MaxScore = maxScore;
        AttemptPolicy = attemptPolicy;
        QuestionsJson = Check.NotNullOrWhiteSpace(questionsJson, nameof(questionsJson));
    }

    public void Publish(DateTime publishedAt)
    {
        if (Status == QuizStatus.Published)
        {
            throw new BusinessException("Quiz is already published."); 
        }

        if (Status == QuizStatus.Closed)
        {
            throw new BusinessException("Quiz is already closed.");
        }
        Status = QuizStatus.Published;
        PublishedAt = publishedAt;
        ClosedAt = null;
    }

    public void Close(DateTime closedAt)
    {
        if (Status == QuizStatus.Closed)
        {
            throw new BusinessException("Quiz is already closed.");
        }
        Status = QuizStatus.Closed;
        ClosedAt = closedAt;
    }
}