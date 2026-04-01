using System;

namespace saasLMS.AssessmentService.QuizAttempts.Events;

public class QuizAttemptStartedDomainEvent
{
    public Guid QuizAttemptId { get;}
    public Guid TenantId { get;}
    public Guid QuizId { get;}
    public Guid StudentId { get;}
    public DateTime StartedAt { get;}

    public QuizAttemptStartedDomainEvent(Guid quizAttemptId, Guid tenantId, Guid quizId, Guid studentId, DateTime startedAt)
    {
        QuizAttemptId = quizAttemptId;
        TenantId = tenantId;
        QuizId = quizId;
        StudentId = studentId;
        StartedAt = startedAt;
    }
}