using System;

namespace saasLMS.AssessmentService.QuizAttempts.Etos;

public class QuizAttemptStartedEto : IntegrationEventEtoBase
{
    public Guid QuizAttemptId { get; set; }
    public Guid QuizId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime StartedAt { get; set; }
}