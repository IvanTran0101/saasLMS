using System;

namespace saasLMS.AssessmentService.QuizAttempts.Events;

public class QuizAttemptExpiredDomainEvent
{
    public Guid QuizAttemptId { get; }
    public Guid TenantId { get; }
    public Guid QuizId { get; }
    public Guid StudentId { get; }
    public DateTime ExpiredAt { get; }

    public QuizAttemptExpiredDomainEvent(Guid quizAttemptId, Guid tenantId, Guid quizId, Guid studentId, DateTime expiredAt)
    {
        QuizAttemptId = quizAttemptId;
        TenantId = tenantId;
        QuizId = quizId;
        StudentId = studentId;
        ExpiredAt = expiredAt;
    }
}