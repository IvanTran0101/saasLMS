using System;
using saasLMS.AssessmentService.QuizAttempts.Events;
using saasLMS.AssessmentService.QuizAttempts.Models;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.AssessmentService.QuizAttempts;

public class QuizAttempt : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid QuizId { get; protected set; }
    public Guid StudentId { get; protected set; }
    public int AttemptNumber { get; protected set; }
    public DateTime StartedAt { get; protected set; }
    public DateTime? CompletedAt { get; protected set; }
    public decimal Score { get; protected set; }
    public string? SubmittedAnswersJson { get; protected set; }
    public QuizAttemptStatus Status { get; protected set; }

    protected QuizAttempt()
    {
        
    }
    
    public QuizAttempt(Guid id, Guid tenantId, Guid quizId, Guid studentId, int attemptNumber, DateTime startedAt) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("The tenant id cannot be empty.", nameof(tenantId));
        }

        if (quizId == Guid.Empty)
        {
            throw new ArgumentException("The quiz id cannot be empty.", nameof(quizId));
        }

        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("The student id cannot be empty.", nameof(studentId));
        }

        if (attemptNumber != 1)
        {
            throw new ArgumentException("The attempt number must be exactly 1 for one-time attempt policy.", nameof(attemptNumber));
        }
        TenantId = tenantId;
        QuizId = quizId;
        StudentId = studentId;
        AttemptNumber = attemptNumber;
        StartedAt = startedAt;
        Status = QuizAttemptStatus.InProgress;
    }

    public static QuizAttempt Create(
        Guid id,
        Guid tenantId,
        Guid quizId,
        Guid studentId,
        int attemptNumber,
        DateTime startedAt)
    {
        var quizAttempt = new QuizAttempt(
            id,
            tenantId,
            quizId,
            studentId,
            attemptNumber,
            startedAt);
        
        quizAttempt.AddLocalEvent(new QuizAttemptStartedDomainEvent(
            quizAttempt.Id,
            quizAttempt.TenantId,
            quizAttempt.QuizId,
            quizAttempt.StudentId,
            quizAttempt.StartedAt));
        
        return quizAttempt;
        
    }

    public void Complete(string submittedAnswersJson, decimal score, DateTime completedAt)
    {
        if (Status == QuizAttemptStatus.Completed)
        {
            throw new BusinessException("The quiz attempt is already completed.");
        }

        if (Status == QuizAttemptStatus.Expired)
        {
            throw new BusinessException("Expired quiz attempt cannot be completed.");        }
        if (completedAt < StartedAt)
        {
            throw new ArgumentException("The completedAt cannot be earlier than StartedAt.", nameof(completedAt));
        }
        SetSubmittedAnswers(submittedAnswersJson);
        SetScore(score);
        CompletedAt = completedAt;
        Status = QuizAttemptStatus.Completed;
        
        AddLocalEvent(new QuizAttemptCompletedDomainEvent(
            Id,
            TenantId,
            QuizId,
            StudentId,
            Score,
            completedAt));
    }

    public void Expire(DateTime expiredAt)
    {
        if (expiredAt < StartedAt)
        {
            throw new ArgumentException("The expiredAt cannot be earlier than StartedAt.", nameof(expiredAt));
        }
        if (Status == QuizAttemptStatus.Expired)
        {
            throw new BusinessException("The quiz attempt is already expired.");
        }
        if (Status == QuizAttemptStatus.Completed)
        {
            throw new BusinessException("Completed quiz attempt cannot be expired.");        }
        CompletedAt = expiredAt;
        Status = QuizAttemptStatus.Expired;
        
        AddLocalEvent(new QuizAttemptExpiredDomainEvent(
            Id,
            TenantId,
            QuizId,
            StudentId,
            expiredAt));
    }

    private void SetSubmittedAnswers(string submittedAnswersJson)
    {
        if (string.IsNullOrWhiteSpace(submittedAnswersJson))
        {
            throw new ArgumentException("The submittedAnswersJson cannot be empty.", nameof(submittedAnswersJson));
        }
        QuizSubmittedAnswerJsonValidator.ValidateAndParse(submittedAnswersJson);
        SubmittedAnswersJson = submittedAnswersJson;
    }

    private void SetScore(decimal score)
    {
        if (score < 0)
        {
            throw new ArgumentException("The score cannot be negative.", nameof(score));
        }
        Score = score;
    }

    public void CompleteByTimeout(DateTime completedAt)
    {
        if (Status == QuizAttemptStatus.Completed)
        {
            throw new BusinessException("The quiz attempt is already completed.");
        }

        if (Status == QuizAttemptStatus.Expired)
        {
            throw new BusinessException("Expired quiz attempt cannot be completed.");
        }

        if (completedAt < StartedAt)
        {
            throw new ArgumentException("The completedAt cannot be earlier than StartedAt.", nameof(completedAt));
        }

        SubmittedAnswersJson = null;
        Score = 0;
        CompletedAt = completedAt;
        Status = QuizAttemptStatus.Completed;
        
        AddLocalEvent(new QuizAttemptCompletedDomainEvent(
            Id,
            TenantId,
            QuizId,
            StudentId,
            Score,
            completedAt));
    }
}