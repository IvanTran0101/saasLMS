using System;
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
    public decimal? Score { get; protected set; }
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

        if (attemptNumber <= 0)
        {
            throw new ArgumentException("The attempt number must be greater than 0.", nameof(attemptNumber));        }
        TenantId = tenantId;
        QuizId = quizId;
        StudentId = studentId;
        AttemptNumber = attemptNumber;
        StartedAt = startedAt;
        Status = QuizAttemptStatus.InProgress;
    }

    public void Complete(decimal score, DateTime completedAt)
    {
        if (score < 0)
        {
            throw new ArgumentException("The score cannot be negative.", nameof(score));
        }

        if (Status == QuizAttemptStatus.Completed)
        {
            throw new BusinessException("The quiz attempt is already completed.");
        }

        if (Status == QuizAttemptStatus.Expired)
        {
            throw new BusinessException("Expired quiz attempt cannot be completed.");        }
        Score = score;
        CompletedAt = completedAt;
        Status = QuizAttemptStatus.Completed;
    }

    public void Expire(DateTime expiredAt)
    {
        if (Status == QuizAttemptStatus.Expired)
        {
            throw new BusinessException("The quiz attempt is already expired.");
        }
        if (Status == QuizAttemptStatus.Completed)
        {
            throw new BusinessException("Completed quiz attempt cannot be expired.");        }
        CompletedAt = expiredAt;
        Status = QuizAttemptStatus.Expired;
    }
}