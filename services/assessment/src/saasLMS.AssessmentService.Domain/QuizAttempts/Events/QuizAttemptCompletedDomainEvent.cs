using System;

namespace saasLMS.AssessmentService.QuizAttempts.Events;

public class QuizAttemptCompletedDomainEvent
{
    public Guid QuizAttemptId { get; }
    public Guid TenantId { get; }
    public Guid QuizId { get; }
    public Guid StudentId { get; }
    public decimal Score { get; }
    public DateTime CompletedAt { get; }

    public QuizAttemptCompletedDomainEvent(Guid quizAttemptId, Guid tenantId, Guid quizId, Guid studentId, decimal score, DateTime completedAt)
    {
        QuizAttemptId = quizAttemptId;
        TenantId = tenantId;
        QuizId = quizId;
        StudentId = studentId;
        Score = score;
        CompletedAt = completedAt;
    }
}