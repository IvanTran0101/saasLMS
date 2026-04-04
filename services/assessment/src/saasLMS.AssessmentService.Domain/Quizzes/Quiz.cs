using System;
using saasLMS.AssessmentService.Quizzes.Events;
using saasLMS.AssessmentService.Quizzes.Models;
using saasLMS.AssessmentService.Shared;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.AssessmentService.Quizzes;

public class Quiz : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid CourseId { get; protected set; }
    public Guid LessonId { get; protected set; }
    public string Title { get; protected set; } = string.Empty;
    public int? TimeLimitMinutes { get; protected set; }
    public decimal MaxScore { get; protected set; }
    public AttemptPolicy AttemptPolicy { get; protected set; }
    public string QuestionsJson { get; protected set; } = string.Empty;
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

        
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        SetTitle(title);
        SetTimeLimit(timeLimitMinutes);
        SetMaxScore(maxScore);
        AttemptPolicy = attemptPolicy;
        SetQuestions(questionJson);
        Status = QuizStatus.Draft;
        
    }
    public static Quiz Create(
        Guid id,
        Guid tenantId,
        Guid courseId,
        Guid lessonId,
        string title,
        int? timeLimitMinutes,
        decimal maxScore,
        AttemptPolicy attemptPolicy,
        string questionJson,
        DateTime createdAt)
    {
        var quiz = new Quiz(
            id,
            tenantId,
            courseId,
            lessonId,
            title,
            timeLimitMinutes,
            maxScore,
            attemptPolicy,
            questionJson);

        quiz.AddLocalEvent(new QuizCreatedDomainEvent(
            quiz.Id,
            quiz.TenantId,
            quiz.CourseId,
            quiz.LessonId,
            quiz.Title,
            quiz.TimeLimitMinutes,
            quiz.MaxScore,
            quiz.AttemptPolicy,
            createdAt));

        return quiz;
    }

    public void UpdateInfo(
        string title,
        int? timeLimitMinutes,
        decimal maxScore,
        AttemptPolicy attemptPolicy,
        string questionsJson,
        DateTime updatedAt)
    {
        if (Status != QuizStatus.Draft)
        {
            throw new BusinessException("Only draft quiz can be updated.");
        }
        SetTitle(title);
        SetTimeLimit(timeLimitMinutes);
        SetMaxScore(maxScore);
        AttemptPolicy = attemptPolicy;
        SetQuestions(questionsJson);
        // AddLocalEvent(new QuizUpdatedEvent(...));
        AddLocalEvent(new QuizUpdatedDomainEvent(
            Id,
            TenantId,
            CourseId,
            LessonId,
            Title,
            TimeLimitMinutes,
            MaxScore,
            AttemptPolicy,
            updatedAt));

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
        EnsureReadyToPublish();
        Status = QuizStatus.Published;
        PublishedAt = publishedAt;
        ClosedAt = null;
        // AddLocalEvent(new QuizPublishedEvent(...));
        AddLocalEvent(new QuizPublishedDomainEvent(
            Id,
            TenantId,
            CourseId,
            LessonId,
            publishedAt));
    }

    public void Close(DateTime closedAt)
    {
        if (Status == QuizStatus.Closed)
        {
            throw new BusinessException("Quiz is already closed.");
        }

        if (Status != QuizStatus.Published)
        {
            throw new BusinessException("Only published quiz can be closed.");
        }
        if (PublishedAt.HasValue && closedAt < PublishedAt.Value)
        {
            throw new BusinessException("Closed date cannot be earlier than published date.");
        }
        Status = QuizStatus.Closed;
        ClosedAt = closedAt;
        // AddLocalEvent(new QuizClosedEvent(...));
        AddLocalEvent(new QuizClosedDomainEvent(
            Id,
            TenantId,
            CourseId,
            LessonId,
            closedAt));
    }
    
    // helpers (by minh)
    private void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
    }

    private void SetTimeLimit(int? timeLimitMinutes)
    {
        if (timeLimitMinutes.HasValue && timeLimitMinutes.Value <= 0)
        {
            throw new ArgumentException("The timeLimitMinutes must be greater than zero.", nameof(timeLimitMinutes));
        }
        TimeLimitMinutes = timeLimitMinutes;
    }

    private void SetMaxScore(decimal maxScore)
    {
        if (maxScore <= 0)
        {
            throw new ArgumentException("The maxScore must be greater than zero.", nameof(maxScore));
        }
        MaxScore = maxScore;
    }

    private void SetQuestions(string questionsJson)
    {
        QuizQuestionsJsonValidator.ValidateAndParse(questionsJson);
        QuestionsJson = questionsJson;
    }

    private void EnsureReadyToPublish()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new BusinessException("Quiz title cannot be empty.");
        }

        if (MaxScore <= 0)
        {
            throw new BusinessException("Quiz maxScore must be greater than zero.");
        }
        QuizQuestionsJsonValidator.ValidateAndParse(QuestionsJson);
    }
}